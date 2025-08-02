using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Codefarts.DependencyInjection;
using Codefarts.DependencyInjection.CodefartsIoc;
using Codefarts.IoC;

namespace Codefarts.BuildHelper.PowerShell
{
    [Cmdlet(VerbsLifecycle.Invoke, "BuildHelper")]
    [OutputType(typeof(FavoriteStuff))]
    public class BuildHelperCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0)]
        public string BuildFile { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1)]
        public string ProjectFile { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 2)]
        public bool SilentMode { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 3)]
        public string TargetFramework { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 3)]
        public string ConfigFile { get; set; }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            // validate files exist
            if (IsFileMissing(this.BuildFile, true, this.SilentMode))
            {
                return;
            }

            if (IsFileMissing(this.ProjectFile, false, this.SilentMode))
            {
                return;
            }

            // do initialization
            var ioc = new DependencyInjectorShim(new Container());
            ioc.Register<IDependencyInjectionProvider>(() => ioc);
            ioc.Register<IStatusReporter, ConsoleStatusReporter>();
            var xmlConfigProvider = ioc.Resolve<XmlFileConfigProvider>();

            xmlConfigProvider.Load(this.ConfigFile);
            // store values
            xmlConfigProvider.SetValue("filename", buildFile);
            xmlConfigProvider.SetValue("projectfile", projectFile);
            xmlConfigProvider.SetValue("targetframework", targetFramework);
            xmlConfigProvider.SetValue("applicationpath", appPath);
            xmlConfigProvider.SetValue("configfile", configFile);

            ioc.Register<IConfigurationProvider>(() => xmlConfigProvider);
            ioc.Register<ICommandImporter>(() => new XmlCommandFileReader(ioc));
            var plugManager = ioc.Resolve<PluginManager>();
            ioc.Register<IPluginManager>(() => plugManager);

            // create app and run
            var app = ioc.Resolve<Application>();
            var result = app.Run();
            if (result.Error != null)
            {
                // Environment.ExitCode = 1;
                if (!silentMode)
                {
                    Console.Write(result.Error.ToString());
                    Console.WriteLine();
                }
            }
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }

        private static bool IsFileMissing(string filename, bool buildFile, bool silentMode)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                if (!silentMode)
                {
                    var text = buildFile ? "Build" : "Project";
                    Console.WriteLine($"{text} file not specified!");
                }

                return true;
            }

            // if file exists we are good to exit
            var buildFileInfo = new FileInfo(filename);
            if (buildFileInfo.Exists)
            {
                return false;
            }

            // file does not seem to exist                                                        
            // Environment.ExitCode = 1;
            if (!silentMode)
            {
                var text = buildFile ? "Build" : "Project";
                Console.WriteLine($"{text} file not found!");
                Console.WriteLine($"File: {filename}");
            }

            return true;
        }
    }
}