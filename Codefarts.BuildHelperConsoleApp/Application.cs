// <copyright file="Application.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
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
        IStatusReporter status = null;
        try
        {
            status = this.ioc.Resolve<IStatusReporter>();
        }
        catch
        {
        }

        // load command plugins
        var pluginManager = this.ioc.Resolve<IPluginManager>();

        // get plugins
        var commandPlugins = new PluginCollection(pluginManager.Plugins);

        // Create and run a command importer 
        var importer = this.ioc.Resolve<ICommandImporter>();
        var importResults = importer.Run();

        if (importResults.Error != null)
            //if (!buildFileReader.TryReadBuildFile(buildFile, out rootCommand))
        {
            status?.Report($"ERROR: Importing commands.\r\n" + importResults.Error);
            return importResults;
        }

        var rootCommand = importResults.ReturnValue as CommandData;
        if (rootCommand == null)
        {
            return RunResult.Errored(new NullReferenceException("Import was successfull but the importer return a null or invalid return value. " +
                                                                $"Value: {importResults.ReturnValue}"));
        }

        // var variables = new VariablesDictionary();
        var args = rootCommand.Run(null, commandPlugins, status);
        return args.Result;
    }
}