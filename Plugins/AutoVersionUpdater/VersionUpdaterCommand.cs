// <copyright file="VersionUpdaterCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace AutoVersionUpdater
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;

    /// <summary>
    /// Provides a command for automatically updating a projects file or assembly version.
    /// </summary>
    [NamedParameter("ProjectFileName", typeof(string), true, "The file path to the project file.")]
    [NamedParameter("file", typeof(bool), false, "If true will increment the file version. Default is true.")]
    [NamedParameter("assembly", typeof(bool), false, "If true will increment the assembly version. Default is true.")]
    [NamedParameter("package", typeof(bool), false, "If true will increment the package version. Default is true.")]
    public class VersionUpdaterCommand : ICommandPlugin
    {
        public string Name
        {
            get
            {
                return "updateversion";
            }
        }

        public void Run(RunCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var projectFilePath = args.GetParameter<string>("ProjectFileName", null);
            projectFilePath = projectFilePath != null ? projectFilePath.ReplaceVariableStrings(args.Variables) : null;
            if (string.IsNullOrWhiteSpace(projectFilePath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("ProjectFileName"));
                return;
            }

            var updateFile = args.GetParameter("file", true);
            var updateAssembly = args.GetParameter("assembly", true);
            var updatePackage = args.GetParameter("package", true);

            // read project file
            XDocument doc;
            try
            {
                doc = XDocument.Load(projectFilePath);
            }
            catch (Exception ex)
            {
                args.Result = RunResult.Errored(ex);
                return;
            }

            // find version info
            var propGroups = Enumerable.Where(doc.Root.Elements(), x => x.Name == "PropertyGroup").ToArray();
            var fileVersion = propGroups.SelectMany(x => x.Elements().Where(y => y.Name == "FileVersion")).FirstOrDefault();
            var assemblyVersion = propGroups.SelectMany(x => x.Elements().Where(y => y.Name == "AssemblyVersion")).FirstOrDefault();
            var packageVersion = propGroups.SelectMany(x => x.Elements().Where(y => y.Name == "PackageVersion")).FirstOrDefault();

            // change version info
            if (updateFile && fileVersion != null)
            {
                string result;
                if (!this.UpdateVersion(fileVersion.Value, out result))
                {
                    args.Result = RunResult.Errored(new BuildException("FileVersion could not be understood."));
                    return;
                }

                fileVersion.Value = result;
            }

            if (updateAssembly && assemblyVersion != null)
            {
                string result;
                if (!this.UpdateVersion(assemblyVersion.Value, out result))
                {
                    args.Result = RunResult.Errored(new BuildException("AssemblyVersion could not be understood."));
                    return;
                }

                assemblyVersion.Value = result;
            }

            // change package version info
            if (updatePackage && packageVersion != null)
            {
                string result;
                if (!this.UpdateVersion(packageVersion.Value, out result))
                {
                    args.Result = RunResult.Errored(new BuildException("PackageVersion could not be understood."));
                    return;
                }

                packageVersion.Value = result;
            }

            // save project file
            try
            {
                doc.Save(projectFilePath);
            }
            catch (Exception ex)
            {
                args.Result = RunResult.Errored(ex);
                return;
            }

            args.Result = RunResult.Sucessful();
        }

        private bool UpdateVersion(string value, out string result)
        {
            var parts = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                result = value;
                return false;
            }

            int part3Value;
            if (!int.TryParse(parts[3], out part3Value))
            {
                result = value;
                return false;
            }

            var date = DateTime.Now;
            parts[0] = date.Year.ToString();
            parts[1] = date.Month.ToString();
            parts[2] = date.Day.ToString();
            parts[3] = (part3Value + 1).ToString();

            result = string.Join(".", parts);
            return true;
        }
    }
}