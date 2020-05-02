// <copyright file="RestoreReferencesCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    public class RestoreReferencesCommand : BuildCommandBase
    {
        public RestoreReferencesCommand(BuildHelper buildHelper)
            : base(buildHelper)
        {
            this.BuildHelper = buildHelper;
        }

        public BuildHelper BuildHelper
        {
            get;
        }

        public override string Name => "restorereferences";

        public override void Execute(IDictionary<string, string> variables, XElement data)
        {
            //  Debugger.Launch();
            var path = data.GetValue("path");
            var destPath = path != null ? path.ReplaceBuildVariableStrings(variables) : null;
            if (destPath == null)
            {
                throw new XmlException($"Command: {nameof(RestoreReferencesCommand)} value: path  - Value not found");
            }

            // check if references file exists
            if (!File.Exists(destPath))
            {
                throw new XmlException($"Command: {nameof(RestoreReferencesCommand)} value: path  - File missing");
            }

            // open project file
            var projectFile = variables["ProjectPath"];
            var proj = XDocument.Load(projectFile);

            // open references file
            var refFile = XDocument.Load(destPath);

            // restore references
            proj.Root.Add(refFile.Root.Elements());

            // save project file
            proj.Save(projectFile);

            // check to delete file afterward
            var deleteValue = data.GetValue("delete");
            if (string.IsNullOrWhiteSpace(deleteValue))
            {
                throw new XmlException($"Command: {nameof(RestoreReferencesCommand)} value: delete  - not specified");
            }

            // check to cleanup reference file
            var doDelete = false;
            if (!bool.TryParse(deleteValue, out doDelete))
            {
                throw new XmlException($"Command: {nameof(RestoreReferencesCommand)} value: delete  - could not determine bool value.");
            }

            File.Delete(destPath);
        }

        private XElement CloneAttributes(XElement r)
        {
            var item = new XElement(r.Name.LocalName, r.Value);
            foreach (var element in r.Elements())
            {
                item.Add(CloneElement(element));
            }

            return item;
        }

        private XElement CloneElement(XElement r)
        {
            var item = new XElement(r.Name.LocalName, r.Value);
            foreach (var element in r.Elements())
            {
                item.Add(CloneElement(element));
            }

            return item;
        }
    }
}