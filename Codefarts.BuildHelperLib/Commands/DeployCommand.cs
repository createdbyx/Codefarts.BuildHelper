// <copyright file="DeployCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

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
        public DeployCommand(Action<string> writeOutput)
            : base(writeOutput)
        {
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

            var message = data.GetValue("message");

            // check if we should clear the folder first
            value = data.GetValue("clean");
            var doClear = string.IsNullOrWhiteSpace(value) ? false : value.Trim().ToLowerInvariant() == "true";

            // check type of conditions
            var conditionsValue = data.GetValue("allconditions");
            var allConditions = true;
            if (conditionsValue != null && !bool.TryParse(conditionsValue, out allConditions))
            {
                throw new ArgumentOutOfRangeException($"'{allConditions}' attribute exists but it's value could not be parsed as a bool value.");
            }

            // check conditions
            if (!data.SatifiesConditions(variables, allConditions))
            {
                this.Output("Conditions not satisfied.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                message = message.ReplaceBuildVariableStrings(variables);
                this.Output($"Message: {message}");
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
            var allFiles = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories).Select(d => Path.GetFileName(d));
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