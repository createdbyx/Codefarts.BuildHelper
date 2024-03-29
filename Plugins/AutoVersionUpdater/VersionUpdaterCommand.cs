﻿// <copyright file="VersionUpdaterCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.IO;
using System.Threading;
using Codefarts.BuildHelper.Exceptions;

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
    [NamedParameter("version", typeof(bool), false, "If true will increment the version. Default is true.")]
    [NamedParameter("retry", typeof(bool), false, "If true will retry to save the project if it is locked by another process. Default is false.")]
    [NamedParameter("retrycount", typeof(bool), false, "Specifies the number of times to retry. Default is 3.")]
    [NamedParameter("retrydelay", typeof(int), false, "Species the retry delay time in milliseconds before retrying. Default is 50ms.")]
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

            // validate command node name is expected
            if (!args.Command.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                args.Result = RunResult.Errored(new ArgumentException($"Command name passed in args is invalid. Command name: {args.Command.Name}"));
                return;
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
            var updateVersion = args.GetParameter("version", true);
            var useCurrentDate = args.GetParameter("usecurrentdate", true);
            var retry = args.GetParameter("retry", false);
            var retryCount = retry ? args.GetParameter("retrycount", 3) : 1;
            var retryDelay = args.GetParameter("retrydelay", 50);

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
            var version = propGroups.SelectMany(x => x.Elements().Where(y => y.Name == "Version")).FirstOrDefault();

            // change file version info
            if (updateFile && fileVersion != null)
            {
                string result;
                if (!this.UpdateVersion(fileVersion.Value, useCurrentDate, out result))
                {
                    args.Result = RunResult.Errored(new BuildException("FileVersion could not be understood."));
                    return;
                }

                fileVersion.Value = result;
            }

            // change assembly version info
            if (updateAssembly && assemblyVersion != null)
            {
                string result;
                if (!this.UpdateVersion(assemblyVersion.Value, useCurrentDate, out result))
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
                if (!this.UpdateVersion(packageVersion.Value, useCurrentDate, out result))
                {
                    args.Result = RunResult.Errored(new BuildException("PackageVersion could not be understood."));
                    return;
                }

                packageVersion.Value = result;
            }

            // change version info
            if (updateVersion && version != null)
            {
                string result;
                if (!this.UpdateVersion(version.Value, useCurrentDate, out result))
                {
                    args.Result = RunResult.Errored(new BuildException("Version could not be understood."));
                    return;
                }

                version.Value = result;
            }

            // save project file

            // retry x number of times waiting x number os milli seconds between each time
            var wasSaved = false;
            Exception lastError = null;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    using (var fs = new FileStream(projectFilePath, FileMode.Truncate, FileAccess.Write, FileShare.None))
                    {
                        doc.Save(fs);
                        wasSaved = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }

                if (retry)
                {
                    Thread.Sleep(retryDelay);
                }
            }

            if (!wasSaved)
            {
                args.Result = RunResult.Errored(lastError);
                return;
            }

            args.Result = RunResult.Sucessful();
        }

        private bool UpdateVersion(string value, bool useCurrentDate, out string result)
        {
            var parts = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                result = value;
                return false;
            }

            int yearValue;
            if (!int.TryParse(parts[0], out yearValue))
            {
                result = value;
                return false;
            }

            int monthValue;
            if (!int.TryParse(parts[1], out monthValue))
            {
                result = value;
                return false;
            }

            int dayValue;
            if (!int.TryParse(parts[2], out dayValue))
            {
                result = value;
                return false;
            }

            int revisionValue;
            if (!int.TryParse(parts[3], out revisionValue))
            {
                result = value;
                return false;
            }

            // we use min/max comparison to prevent exceptions with possible bad numeric values that are to be converted into a date
            var year = Math.Max(1, yearValue);
            var month = Math.Max(1, monthValue);
            var day = Math.Max(1, dayValue);

            year = Math.Min(9999, year);
            month = Math.Min(12, month);
            day = Math.Min(31, day);

            var storedTime = new DateTime(year, month, day);

            var date = DateTime.Now;
            parts[0] = (useCurrentDate ? date.Year : storedTime.Year).ToString();
            parts[1] = (useCurrentDate ? date.Month : storedTime.Month).ToString();
            parts[2] = (useCurrentDate ? date.Day : storedTime.Day).ToString();
            parts[3] = (useCurrentDate && storedTime.Date != date.Date ? 0 : revisionValue + 1).ToString();

            result = string.Join(".", parts);
            return true;
        }
    }
}