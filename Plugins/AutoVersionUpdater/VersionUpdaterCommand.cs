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
    [NamedVariable("ProjectFileName", typeof(string), true, "Specifies the full path to the project file.")]
    public class VersionUpdaterCommand : IBuildCommand
    {
        public string Name
        {
            get
            {
                return "UpdateVersion";
            }
        }

        public void Run(ExecuteCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Variables == null)
            {
                throw new NullReferenceException("Variables dictionary is null.");
            }

            var projectFilePath = args.GetVariable<string>("ProjectFileName", null);
            projectFilePath = projectFilePath != null ? projectFilePath.ReplaceVariableStrings(args.Variables) : null;
            if (string.IsNullOrWhiteSpace(projectFilePath))
            {
                throw new MissingVariableException($"Command: {nameof(VersionUpdaterCommand)} value: ProjectFileName  - Value not found");
            }

            var updateFile = args.GetParameter("file", true);
            var updateAssembly = args.GetParameter("assembly", true);

            // read project file
            var doc = XDocument.Load(projectFilePath);

            // find version info
            var propGroups = Enumerable.Where(doc.Root.Elements(), x => x.Name == "PropertyGroup").ToArray();
            var fileVersion = propGroups.SelectMany(x => x.Elements().Where(y => y.Name == "FileVersion")).FirstOrDefault();
            var assemblyVersion = propGroups.SelectMany(x => x.Elements().Where(y => y.Name == "AssemblyVersion")).FirstOrDefault();

            // change version info
            if (updateFile && fileVersion != null)
            {
                fileVersion.Value = this.UpdateVersion(fileVersion.Value);
            }

            if (updateAssembly && assemblyVersion != null)
            {
                assemblyVersion.Value = this.UpdateVersion(assemblyVersion.Value);
            }

            // save project file
            doc.Save(projectFilePath);
        }

        private string UpdateVersion(string value)
        {
            var parts = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var date = DateTime.Now;
            parts[0] = date.Year.ToString();
            parts[1] = date.Month.ToString();
            parts[2] = date.Day.ToString();
            parts[3] = (int.Parse(parts[3]) + 1).ToString();
            return string.Join(".", parts);
        }
    }
}