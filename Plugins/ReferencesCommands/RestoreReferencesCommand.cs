// <copyright file="RestoreReferencesCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    public class RestoreReferencesCommand : IBuildCommand
    {
        public string Name => "restorereferences";

        public void Run(ExecuteCommandArgs args)
        {
            var path = args.GetParameter<string>("path");
            var destPath = path != null ? path.ReplaceVariableStrings(args.Variables) : null;
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
            var projectFile = args.Variables["ProjectPath"];
            var proj = XDocument.Load(projectFile);

            // open references file
            var refFile = XDocument.Load(destPath);

            // restore references
            proj.Root.Add(refFile.Root.Elements());

            // save project file
            proj.Save(projectFile);

            // check to delete file afterward
            var doDelete = args.GetParameter("delete", false);
            //if (string.IsNullOrWhiteSpace(deleteValue))
            //{
            //    throw new XmlException($"Command: {nameof(RestoreReferencesCommand)} value: delete  - not specified");
            //}

            // check to cleanup reference file
            //var doDelete = false;
            //if (!bool.TryParse(deleteValue, out doDelete))
            //{
            //    throw new XmlException($"Command: {nameof(RestoreReferencesCommand)} value: delete  - could not determine bool value.");
            //}

            //File.Delete(destPath);
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