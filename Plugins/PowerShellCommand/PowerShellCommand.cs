// <copyright file="PowerShellCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Diagnostics;

namespace PowerShellCommand;

using System;
using Codefarts.BuildHelper;

/// <summary>
/// Provides a command for running powershell scripts.
/// </summary>
[NamedParameter("file", typeof(string), false, "Specifies the location of a powershell script.")]
public class PowerShellCommand : ICommandPlugin
{
    public string Name
    {
        get
        {
            return "powershell";
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

        var scriptFile = args.GetParameter("file", default(string)).ReplaceVariableStrings(args.Variables);
        var buildFile = args.GetVariable("BuildFile", default(string));

        var createdTempFile = false;

        if (string.IsNullOrWhiteSpace(scriptFile))
        {
            scriptFile = Path.ChangeExtension(Path.GetTempFileName(), ".ps1");
            var psScript = args.GetParameter("Value", string.Empty);
            File.WriteAllText(scriptFile, psScript);
        }

        try
        {
            var fileInfo = new FileInfo(scriptFile);
            if (!fileInfo.Exists)
            {
                args.Result = RunResult.Errored(new FileNotFoundException($"PowerShell file does not exist! {scriptFile}", scriptFile));
                return;
            }
        }
        catch
        {
        }

        var startInfo = new ProcessStartInfo();
        startInfo.FileName = @"powershell";
        startInfo.Arguments = $"\"{scriptFile}\"";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardError = true;
        startInfo.WorkingDirectory = Path.GetDirectoryName(buildFile);
        var process = Process.Start(startInfo);
        process.WaitForExit();

        // clean up script file
        if (createdTempFile)
        {
            try
            {
                File.Delete(scriptFile);
            }
            catch
            {
            }
        }

        string errorData = null; // Get error output as string
        if (process.ExitCode != 0)
        {
            using (var reader = process.StandardError)
            {
                errorData = reader.ReadToEnd();
            }
        }

        args.Result = process.ExitCode == 0
            ? RunResult.Sucessful()
            : RunResult.Errored(new Exception(errorData));

        process.Dispose();
    }
}