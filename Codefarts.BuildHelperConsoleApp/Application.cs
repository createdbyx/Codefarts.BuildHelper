// <copyright file="Application.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using Codefarts.BuildHelper;
using Codefarts.DependencyInjection;

namespace Codefarts.BuildHelperConsoleApp;

public class Application
{
    private readonly IDependencyInjectionProvider ioc;

    public Application(IDependencyInjectionProvider ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
    }

    public void Run(string buildFile)
    {
        var status = this.ioc.Resolve<IStatusReporter>();


        // load command plugins
        var pluginLoader = this.ioc.Resolve<PluginLoader>();
        var appPath = Process.GetCurrentProcess().MainModule.FileName;
        var appDir = Path.GetDirectoryName(appPath);
        var pluginFolder = Path.Combine(appDir, "Plugins");

        pluginLoader.PluginFolder = pluginFolder;
        var commandPlugins = pluginLoader.Load();

        // read build file
        var variables = new VariablesDictionary();
        variables["Application"] = Path.GetFileName(appPath);

        CommandData rootCommand;
        var buildFileReader = this.ioc.Resolve<XmlBuildFileReader>();
        if (!buildFileReader.TryReadBuildFile(buildFile, out rootCommand))
        {
            status?.Report($"ERROR: Reading Build File. {buildFile}");
            Environment.ExitCode = 1;
            return;
        }

        var buildEventValue = variables.GetValue<string>("BuildEvent", null);
        this.ReportHeader(status, $"START {buildEventValue} BUILD");
        rootCommand.Run(variables, commandPlugins, status);
        this.ReportHeader(status, $"END {buildEventValue} BUILD");
    }

    private void ReportHeader(IStatusReporter status, string message, params object[] args)
    {
        var formattedString = string.Format(message, args);
        var maxLen = Math.Max(formattedString.Length + 10, 100);
        var headPartLen = (maxLen - (formattedString.Length + 2)) / 2;
        var headerChars = new string('#', headPartLen);
        status?.Report(string.Format($"{headerChars} {message} {headerChars}", args));
    }
}