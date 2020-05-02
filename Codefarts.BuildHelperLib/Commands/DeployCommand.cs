// <copyright file="DeployCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

using System.Diagnostics;

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public class DeployCommand : BuildCommandBase
    {
        public DeployCommand(BuildHelper buildHelper)
            : base(buildHelper)
        {
            this.BuildHelper = buildHelper;
        }

        public BuildHelper BuildHelper
        {
            get;
        }

        public override string Name => "deploy";

        public override void Execute(IDictionary<string, string> variables, XElement data)
        {
            //Debugger.Launch();
            var value = data.GetValue("path");
            var destPath = value != null ? value.ReplaceBuildVariableStrings(variables) : null;
            if (destPath == null)
            {
                throw new XmlException($"Command: {nameof(DeployCommand)} value: path  - Value not found");
            }

            // check if we should clear the folder first
            value = data.GetValue("clean");
            var doClear = string.IsNullOrWhiteSpace(value) ? false : value.Trim().ToLowerInvariant() == "true";

            // check conditions
            if (!data.SatifiesConditions(variables))
            {
                this.Output("Conditions not satisfied.");
                return;
            }

            this.Output($"Clearing before deploy ({doClear}): {destPath}");
            var di = new DirectoryInfo(destPath);
            if (doClear && di.Exists)
            {
                foreach (var file in di.EnumerateFiles())
                {
                    file.Delete();
                }

                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            //Console.WriteLine($"dest: {destPath}");

            var srcPath = "$(ProjectDir)$(OutDir)".ReplaceBuildVariableStrings(variables);
            // Console.WriteLine($"srcPath: {srcPath}");
            var allFiles = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories).Select(d => d.Remove(0, srcPath.Length));
            foreach (var file in allFiles)
            {
                var src = Path.Combine(srcPath, file);
                var dest = Path.Combine(destPath, file);

                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                this.Output("Deploying: " + src + " ==> " + dest);
                File.Copy(src, dest, true);
            }

            // copy project assemblies & dependencies
            //if (!this.TryCopyProjectAssemblies(variables))
            //{
            //    return;
            //}
        }

        /*
            private bool TryCopyProjectAssemblies(IDictionary<string, string> varibles)
            {
                // setup dest path
                var asmFolder = @"P:\Codefarts Assemblies\";
                Directory.CreateDirectory(asmFolder);

                var projFolder = Path.Combine(asmFolder, varibles["ProjectName"]);
                var latestFolder = Path.Combine(projFolder, "Latest");
                var configurationFolder = Path.Combine(latestFolder, varibles["ConfigurationName"]);
                var destFile = Path.Combine(configurationFolder, varibles["TargetFileName"]);

                // copy project dll to path
                try
                {
                    Directory.CreateDirectory(configurationFolder);
                    File.Copy(varibles["TargetPath"], destFile, true);
                    Console.WriteLine("File Copied: '{0}' - > '{1}'", varibles["TargetPath"], destFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error copying to destination: " + destFile);
                    Console.WriteLine(ex.Message);
                    Environment.Exit(1);
                    return false;
                }

                // extract project file refs
                var ignorePaths = new[]
                {
                @"C:\Program Files\Unity\Hub\Editor\",
            };

                var unityFiles = new[]
                {
                "UnityEngine.dll",
                "UnityEditor.dll",
            };

                var filter = new Predicate<string>(path =>
                  {
                      var fileName = Path.GetFileName(path);
                      path = path.ToLowerInvariant();
                      var ignorePath = ignorePaths.Any(p => !path.Contains(p.ToLowerInvariant()));
                      return unityFiles.Any(p => fileName.ToLowerInvariant().Equals(p.ToLowerInvariant())) ? true : ignorePath;
                  });

                var projectReferences = ProjectHelpers.GetProjectReferences(varibles["ProjectPath"], filter);

                foreach (var reference in projectReferences)
                {
                    destFile = Path.Combine(configurationFolder, Path.GetFileName(reference));
                    // try copy project reference dll to path
                    try
                    {
                        Directory.CreateDirectory(configurationFolder);
                        File.Copy(reference, destFile, true);
                        Console.WriteLine("File Copied: '{0}' - > '{1}'", reference, destFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error copying reference to destination: " + destFile);
                        Console.WriteLine(ex.Message);
                        Environment.Exit(1);
                        return false;
                    }
                }

                return true;
            }
            */
    }
}