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

    public RunResult Run()
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

        // Create and run a command importer 
        var importer = this.ioc.Resolve<ICommandImporter>();
        var result = importer.Run();

        if (result.Error != null)
            //if (!buildFileReader.TryReadBuildFile(buildFile, out rootCommand))
        {
            status?.Report($"ERROR: Importing commands.\r\n" + result.Error);
            return result;
        }

        var rootCommand = result.ReturnValue as CommandData;
        if (rootCommand == null)
        {
            throw new NullReferenceException("Import was successfull but the importer return a null or invalid return value. " +
                                             $"Value: {result.ReturnValue}");
        }

        var buildEventValue = variables.GetValue<string>("BuildEvent", null);
        this.ReportHeader(status, $"START {buildEventValue} BUILD");
        var args = rootCommand.Run(variables, commandPlugins, status);
        this.ReportHeader(status, $"END {buildEventValue} BUILD");

        return args.Result;
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