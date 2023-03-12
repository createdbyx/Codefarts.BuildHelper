using System.Collections.ObjectModel;
using System.Xml.Linq;
using Codefarts.BuildHelper;
using Codefarts.DependencyInjection;

namespace BuildCommand;

// [NamedParameter("source", typeof(string), true, "The source folder that will be copied.")]
// [NamedParameter("destination", typeof(string), true, "The destination folder where files and folder will be copied to.")]
// [NamedParameter("clean", typeof(bool), false, "If true will delete contents from the destination before copying. Default is false.")]
// [NamedParameter("subfolders", typeof(bool), false, "If true will copy subfolders as well. Default is true.")]
// [NamedParameter("allconditions", typeof(bool), false, "Specifies weather or not all conditions must be satisfied. Default is false.")]
// [NamedParameter("ignoreconditions", typeof(bool), false, "Specifies weather to ignore conditions. Default is false.")]
// [NamedParameter("test", typeof(bool), false, "Specifies weather to run in test mode. Default is false.")]
[NamedParameter("haltonerror", typeof(bool), true, "Specifies weather to stop execution if a command returns an error. Default is true.")]
public class BuildCommand : ICommandPlugin
{
    private IStatusReporter status;
    private readonly IDependencyInjectionProvider ioc;

    public string Name
    {
        get
        {
            return "build";
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildCommand"/> class.
    /// </summary>
    public BuildCommand(IDependencyInjectionProvider ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
        try
        {
            this.status = ioc.Resolve<IStatusReporter>();
        }
        catch
        {
        }
    }

    public void Run(RunCommandArgs args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        // validate command node name is expected
        if (!args.Command.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            args.Result = RunResult.Errored(new ArgumentException($"Command name passed in args is invalid. Command name: {args.Command.Name}"));
            return;
        }

        var haltOnError = args.GetParameter("haltonerror", true);

        IConfigurationProvider config;
        try
        {
            config = this.ioc.Resolve<IConfigurationProvider>();
        }
        catch (Exception ex)
        {
            args.Result = RunResult.Errored(ex);
            return;
        }

        if (!TryGetConfigValue(args, config, "filename", out var buildFile)) return;
        if (!TryGetConfigValue(args, config, "projectfile", out var projectFile)) return;
        if (!TryGetConfigValue(args, config, "targetframework", out var targetFramework)) return;

        // read the project file once for it's info
        var projectFileRoot = XDocument.Load(projectFile).Root;

        // build filename and path info
        args.Variables["BuildFile"] = buildFile;
        args.Variables["TargetFramework"] = targetFramework;

        // post build event type
        string buildEvent = null;
        if (buildFile.EndsWith("-postbuild.xml", StringComparison.InvariantCultureIgnoreCase))
        {
            buildEvent = "Post";
        }

        // pre build event type
        if (buildFile.EndsWith("-prebuild.xml", StringComparison.InvariantCultureIgnoreCase))
        {
            buildEvent = "Pre";
        }

        // check for valid buildevent in filename
        if (string.IsNullOrWhiteSpace(buildEvent) || (buildEvent != "Pre" && buildEvent != "Post"))
        {
            args.Result = RunResult.Errored(new Exception("Filename has unrecognized or missing build event information."));
            return;
        }

        args.Variables["BuildEvent"] = buildEvent;

        // configuration name
        var justFileName = Path.GetFileName(buildFile);
        var configName = justFileName.Substring(0, justFileName.IndexOf($"-{buildEvent}", StringComparison.InvariantCultureIgnoreCase));
        args.Variables["ConfigurationName"] = configName;

        // out directory
        ParseOutDirectory(args.Variables, projectFileRoot);

        // project name
        var projectName = Path.GetFileNameWithoutExtension(projectFile);
        args.Variables["ProjectName"] = projectName;

        // the target name
        var assemblyNameElement = projectFileRoot.Descendants("AssemblyName").FirstOrDefault();
        args.Variables["TargetName"] = assemblyNameElement != null ? assemblyNameElement.Value : args.Variables["ProjectName"];

        ParseTargetExt(args.Variables, projectFileRoot);

        // the target path
        args.Variables["TargetPath"] = Path.Combine(
            Path.GetDirectoryName(projectFile),
            args.GetVariable<string>("OutDir"),
            args.GetVariable<string>("TargetName") +
            args.GetVariable<string>("TargetExt"));

        // project path
        args.Variables["ProjectPath"] = projectFile;

        // project filename
        args.Variables["ProjectFileName"] = Path.GetFileName(projectFile);

        // target filename
        args.Variables["TargetFileName"] = Path.GetFileName(args.GetVariable<string>("TargetPath"));

        // target directory
        args.Variables["TargetDir"] = Path.GetDirectoryName(args.GetVariable<string>("TargetPath")) + "\\";

        // project directory
        args.Variables["ProjectDir"] = Path.GetDirectoryName(args.GetVariable<string>("ProjectPath")) + "\\";

        // project filename extension
        args.Variables["ProjectExt"] = Path.GetExtension(args.GetVariable<string>("ProjectPath"));

        // get plugin manager
        IPluginManager pluginManager;
        try
        {
            pluginManager = this.ioc.Resolve<IPluginManager>();
        }
        catch (Exception e)
        {
            args.Result = RunResult.Errored(e);
            return;
        }

        var pluginCollection = new ReadOnlyCollection<ICommandPlugin>(pluginManager.Plugins.ToList());

        this.ReportHeader(status, $"START {projectName} - {buildEvent.ToUpper()} BUILD");

        // run children
        RunCommandArgs lastResult = null;
        foreach (var child in args.Command.Children)
        {
            var variables = new VariablesDictionary(args.Variables);
            lastResult = child.Run(variables, pluginCollection, this.status);
            if (lastResult.Result.Status == RunStatus.Errored && haltOnError)
            {
                break;
            }
        }

        this.ReportHeader(status, $"END {projectName} - {buildEvent.ToUpper()} BUILD");

        args.Result = lastResult != null ? lastResult.Result : RunResult.Sucessful();
    }

    private void ReportHeader(IStatusReporter status, string message, params object[] args)
    {
        var formattedString = string.Format(message, args);
        var maxLen = Math.Max(formattedString.Length + 10, 100);
        var headPartLen = (maxLen - (formattedString.Length + 2)) / 2;
        var headerChars = new string('#', headPartLen);
        status?.Report(string.Format($"{headerChars} {message} {headerChars}", args));
    }

    /*       
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="buildhelper -b:'$(ProjectDir)$(ConfigurationName)-PostBuild.xml' -p:'$(ProjectPath)' -tf:'$(TargetFramework)'" />
</Target>
<Target Name="PostBuild" AfterTargets="PreBuildEvent">
    <Exec Command="buildhelper -b:'$(ProjectDir)$(ConfigurationName)-PreBuild.xml' -p:'$(ProjectPath)' -tf:'$(TargetFramework)'" />
</Target>

-vs_BuildEvent:Post 
-vs_ConfigurationName:'$(ConfigurationName)' 
-vs_OutDir:'$(OutDir)' 
-vs_ProjectName:'$(ProjectName)' 
-vs_TargetName:'$(TargetName)' 
-vs_TargetExt:'$(TargetExt)' 
-vs_TargetPath:'$(TargetPath)' 
-vs_ProjectPath:'$(ProjectPath)' 
-vs_ProjectFileName:'$(ProjectFileName)' 
-vs_TargetFileName:'$(TargetFileName)' 
-vs_DevEnvDir:'$(DevEnvDir)' 
-vs_TargetDir:'$(TargetDir)' 
-vs_ProjectDir:'$(ProjectDir)' 
-vs_SolutionFileName:'$(SolutionFileName)' 
-vs_SolutionPath:'$(SolutionPath)' 
-vs_SolutionDir:'$(SolutionDir)' 
-vs_SolutionName:'$(SolutionName)' 
-vs_PlatformName:'$(PlatformName)' 
-vs_ProjectExt:'$(ProjectExt)' 
-vs_SolutionExt:'$(SolutionExt)'" 

<Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="powershell.exe -ExecutionPolicy Unrestricted -noprofile -nologo -noninteractive -Command .'P:\PowerShell\post-build.ps1' -vs_BuildEvent:Post -vs_OutDir:'$(OutDir)' -vs_ConfigurationName:'$(ConfigurationName)' -vs_ProjectName:'$(ProjectName)' -vs_TargetName:'$(TargetName)' -vs_TargetPath:'$(TargetPath)' -vs_ProjectPath:'$(ProjectPath)' -vs_ProjectFileName:'$(ProjectFileName)' -vs_TargetExt:'$(TargetExt)' -vs_TargetFileName:'$(TargetFileName)' -vs_DevEnvDir:'$(DevEnvDir)' -vs_TargetDir:'$(TargetDir)' -vs_ProjectDir:'$(ProjectDir)' -vs_SolutionFileName:'$(SolutionFileName)' -vs_SolutionPath:'$(SolutionPath)' -vs_SolutionDir:'$(SolutionDir)' -vs_SolutionName:'$(SolutionName)' -vs_PlatformName:'$(PlatformName)' -vs_ProjectExt:'$(ProjectExt)' -vs_SolutionExt:'$(SolutionExt)'" />
</Target>
<Target Name="PreBuild" BeforeTargets="PreBuildEvent">
    <Exec Command="powershell.exe -ExecutionPolicy Unrestricted -noprofile -nologo -noninteractive -Command .'P:\PowerShell\post-build.ps1' -vs_BuildEvent:Pre -vs_OutDir:'$(OutDir)' -vs_ConfigurationName:'$(ConfigurationName)' -vs_ProjectName:'$(ProjectName)' -vs_TargetName:'$(TargetName)' -vs_TargetPath:'$(TargetPath)' -vs_ProjectPath:'$(ProjectPath)' -vs_ProjectFileName:'$(ProjectFileName)' -vs_TargetExt:'$(TargetExt)' -vs_TargetFileName:'$(TargetFileName)' -vs_DevEnvDir:'$(DevEnvDir)' -vs_TargetDir:'$(TargetDir)' -vs_ProjectDir:'$(ProjectDir)' -vs_SolutionFileName:'$(SolutionFileName)' -vs_SolutionPath:'$(SolutionPath)' -vs_SolutionDir:'$(SolutionDir)' -vs_SolutionName:'$(SolutionName)' -vs_PlatformName:'$(PlatformName)' -vs_ProjectExt:'$(ProjectExt)' -vs_SolutionExt:'$(SolutionExt)'&#xD;&#xA;" />
</Target>
*/
    private static bool TryGetConfigValue(RunCommandArgs args, IConfigurationProvider config, string key, out string buildFile)
    {
        if (config.TryGetValue(key, out buildFile))
        {
            return true;
        }

        args.Result = RunResult.Errored(new ArgumentNullException($"Error fetching '{key}' from configuration."));
        return false;
    }

    private void ParseOutDirectory(VariablesDictionary variables, XElement projectFileRoot)
    {
        // extract config name from build filename
        variables.TryGetValue("ConfigurationName", out var configName);
        variables.TryGetValue("TargetFramework", out var targetFramework);

        // check if there is a condition attribute that ends with the configuration name
        var item = projectFileRoot.Descendants("OutputPath").Where(x =>
        {
            var att = x.Attribute("Condition");
            return att == null || att.Value.Trim().EndsWith($" == '{configName}'");
        }).FirstOrDefault();

        // if one was found use it
        if (item != null)
        {
            variables["OutDir"] = item.Value;
            return;
        }

        // otherwise set default value based on config name
        variables["OutDir"] = $@"bin\{configName}\{targetFramework}\";
    }

    private void ParseTargetExt(VariablesDictionary variables, XElement projectFileRoot)
    {
        // check if there is a condition attribute that ends with the configuration name
        var item = projectFileRoot.Descendants("OutputType").FirstOrDefault();

        // if one was found use it
        if (item != null)
        {
            variables["TargetExt"] = item.Value;
            return;
        }

        // otherwise set default value to dll
        variables["TargetExt"] = ".dll";
    }
}