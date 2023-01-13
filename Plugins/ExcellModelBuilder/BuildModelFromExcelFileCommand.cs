// <copyright file="CopyDirCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Data;
using System.Net;
using System.Text;
using Codefarts.BuildHelper.Exceptions;
using ExcelDataReader;

namespace Codefarts.BuildHelper
{
    using System;
    using System.IO;
    using System.Linq;

    [NamedParameter("source", typeof(string), true, "The source folder that will be copied.")]
    [NamedParameter("destination", typeof(string), true, "The destination folder where files and folder will be copied to.")]
    [NamedParameter("singlefile", typeof(bool), false,
                    "If true will generate individual code files whose filename and class name match the table names. Default is false.")]
    [NamedParameter("namespace", typeof(string), false, "Specifies the namespace where the generated classes will be located.")]
    //[NamedParameter("clean", typeof(bool), false, "If true will delete contents from the destination before copying. Default is false.")]
    //[NamedParameter("replace", typeof(bool), false, "If true will delete contents from the destination before copying. Default is false.")]
    //  [NamedParameter("subfolders", typeof(bool), false, "If true will copy subfolders as well. Default is true.")]
    // [NamedParameter("allconditions", typeof(bool), false, "Specifies weather or not all conditions must be satisfied. Default is false.")]
    // [NamedParameter("ignoreconditions", typeof(bool), false, "Specifies weather to ignore conditions. Default is false.")]
    //  [NamedParameter("relativepaths", typeof(bool), false,
    //                  "Specifies weather condition checks will compare against relative paths or full file paths. Default is false.")]
    public class BuildModelFromExcelFileCommand : ICommandPlugin
    {
        private IStatusReporter status;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildModelFromExcelFileCommand"/> class.
        /// </summary>
        public BuildModelFromExcelFileCommand(IStatusReporter status) : this()
        {
            this.status = status ?? throw new ArgumentNullException(nameof(status));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildModelFromExcelFileCommand"/> class.
        /// </summary>
        public BuildModelFromExcelFileCommand()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public string Name => "excellmodelexport";

        public void Run(RunCommandArgs args)
        {
            //  Debugger.Launch();
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var srcPath = args.GetParameter<string>("source", null);
            var destPath = args.GetParameter<string>("destination", null);

            if (string.IsNullOrWhiteSpace(destPath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("destination"));
                return;
            }

            if (string.IsNullOrWhiteSpace(srcPath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("source"));
                return;
            }

            srcPath = srcPath.ReplaceVariableStrings(args.Variables);
            destPath = destPath.ReplaceVariableStrings(args.Variables);

            // validate src and dest paths
            if (srcPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                args.Result = RunResult.Errored(
                    new MissingParameterException("source", "Contains invalid path characters after replacing variables."));
                return;
            }

            if (destPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                args.Result = RunResult.Errored(
                    new MissingParameterException("destination", "Contains invalid path characters after replacing variables."));
                return;
            }

            // check if source exists
            if (!File.Exists(srcPath))
            {
                args.Result = RunResult.Errored(new FileNotFoundException("Specified excel source file not found!", srcPath));
                return;
            }

            var singleFile = args.GetParameter("singlefile", false);
            var namespaceName = args.GetParameter("namespace", default(string));
            namespaceName = namespaceName?.ReplaceVariableStrings(args.Variables);
            //  var ignoreConditions = args.GetParameter("ignoreconditions", false);

            if (singleFile && Path.GetExtension(destPath) != ".cs")
            {
                args.Result = RunResult.Errored(
                    new Exception($"singlefile argument set to true but destination filename does not end with .cs!\r\ndestination: {destPath}"));
                return;
            }

            // check if we should clear the folder first
            //var doClear = args.GetParameter("clean", false);

            // this.status?.Report($"Clearing before copy ({doClear}): {destPath}");
            //  var classTemplate = string.Empty;
            try
            {
                //var classTemplateNode = args.Command.Children.FirstOrDefault(x=>x.Name.Equals("classtemplate", StringComparison.InvariantCultureIgnoreCase));
                // classTemplate=  classTemplateNode?.
                // {
                //     
                // }

                // read excel file
                DataSet result;
                using (var stream = File.Open(srcPath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:

                        // // 1. Use the reader methods
                        // do
                        // {
                        //     while (reader.Read())
                        //     {
                        //         // reader.GetDouble(0);
                        //     }
                        // } while (reader.NextResult());

                        // 2. Use the AsDataSet extension method
                        result = reader.AsDataSet();
                    }
                }

                var encoding = new UTF8Encoding(true);

                // The result of each spreadsheet is in result.Tables
                foreach (DataTable? table in result.Tables)
                {
                    // check if table has at least 4 rows
                    if (table.Rows.Count >= 4)
                    {
                        var className = table.TableName;
                        var namespaces = table.Rows[0].Field<string>(1);
                        var inherits = table.Rows[1].Field<string>(1);

                        var body = string.Empty;
                        // process properties
                        for (var i = 3; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            var propertyName = row[0].ToString();
                            var propertyType = row[1].ToString();
                            var propertyDefaultValue = row[2].ToString();

                            if (string.IsNullOrWhiteSpace(propertyType) && string.IsNullOrWhiteSpace(propertyName))
                            {
                                break;
                            }

                            body += "\r\n" + this.GetPropertyTemplate(propertyName, propertyType, propertyDefaultValue);
                        }

                        // validate class name
                        var nsStrings = namespaces?.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                                  .Select(x => $"using {x};");
                        var classData = this.GetClassTemplate(className, body, inherits, nsStrings);
                        var writePath = singleFile ? destPath : Path.Combine(destPath, className + ".cs");
                        var writeDir = Path.GetDirectoryName(writePath);
                        Directory.CreateDirectory(writeDir);
                        using (var writer = File.OpenWrite(writePath))
                        {
                            writer.Seek(0, SeekOrigin.End);
                            var dataToWrite = this.GetNamespaceTemplate(namespaceName, classData);
                            writer.Write(new ReadOnlySpan<byte>(encoding.GetBytes(dataToWrite)));
                            writer.SetLength(writer.Position);  // truncate file in case we opened a pre-existing file
                        }

                        //  File.WriteAllText(writePath, this.GetNamespaceTemplate(namespaceName, classData));
                    }
                }
            }
            catch (Exception ex)
            {
                args.Result = RunResult.Errored(ex);
                return;
            }

            args.Result = RunResult.Sucessful();
        }

        private string GetPropertyTemplate(string name, string type, string defaultValue)
        {
            var quotes = type.Trim().ToLowerInvariant() == "string" ? "\"" : string.Empty;
            defaultValue = string.IsNullOrWhiteSpace(defaultValue) ? string.Empty : "=" + quotes + defaultValue + quotes;
            return $"public {type} {name} {{ get; set; }} {defaultValue}\r\n";
        }

        private string GetClassTemplate(string name, string body, string inheritsFrom, IEnumerable<string>? namespaces)
        {
            //var ns = namespaces?.Select(x => $"using {x}\r\n");
            var ns = namespaces == null ? string.Empty : string.Join("\r\n", namespaces); //  namespaces?.Select(x => $"using {x}\r\n");
            inheritsFrom = string.IsNullOrWhiteSpace(inheritsFrom) ? string.Empty : " : " + inheritsFrom;
            return ns + "\r\n" +
                   $"public class {name} {inheritsFrom}\r\n" +
                   "{\r\n" +
                   $"    {body}" +
                   "}\r\n";
        }

        private string GetNamespaceTemplate(string name, string body)
        {
            return $"namespace {name}\r\n" +
                   "{\r\n" +
                   $"    {body}" +
                   "}\r\n";
        }
    }
}