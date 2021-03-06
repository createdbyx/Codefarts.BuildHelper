// <copyright file="ExcludeReferenceCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public class ExcludeReferenceCommand : BuildCommandBase
    {
        public ExcludeReferenceCommand(BuildHelper buildHelper)
            : base(buildHelper)
        {
            this.BuildHelper = buildHelper;
        }

        public BuildHelper BuildHelper
        {
            get;
        }
        
        public override string Name => "excludereference";

        public override void Execute(IDictionary<string, string> variables, XElement data)
        {
            //  Debugger.Launch();
            var path = data.GetValue("path");
            var destPath = path != null ? path.ReplaceBuildVariableStrings(variables) : null;
            if (destPath == null)
            {
                throw new XmlException($"Command: {nameof(ExcludeReferenceCommand)} value: path  - Value not found");
            }

            var type = data.GetValue("type");
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new XmlException($"Command: {nameof(ExcludeReferenceCommand)} value: type  - not specified");
            }

            switch (type)
            {
                case "project":
                    var name = data.GetValue("name");
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new XmlException($"Command: {nameof(ExcludeReferenceCommand)} value: name  - not specified");
                    }

                    var appendValue = data.GetValue("append");
                    if (string.IsNullOrWhiteSpace(appendValue))
                    {
                        throw new XmlException($"Command: {nameof(ExcludeReferenceCommand)} value: append  - not specified");
                    }

                    var append = false;
                    if (!bool.TryParse(appendValue, out append))
                    {
                        throw new XmlException($"Command: {nameof(ExcludeReferenceCommand)} value: append  - could not determine bool value.");
                    }

                    // open project file
                    var projectFile = variables["ProjectPath"];
                    var proj = XDocument.Load(projectFile);

                    // search for project reference with matching name
                    var projRefs = proj.Root.Elements()
                            .Where(r => r.Name.LocalName == "ItemGroup").SelectMany(r => r.Elements().Where(x => x.Name.LocalName == "ProjectReference"));
                    var changed = false;
                    foreach (var projReference in projRefs)
                    {
                        var nameElement = projReference.Elements().FirstOrDefault(r => r.Name.LocalName == "Name");
                        if (nameElement.Value.Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            // remove project reference
                            var parent = projReference.Parent;

                            SaveProjectReference(destPath, projReference, append);
                            changed = true;
                            projReference.Remove();

                            // check if ItemGroup element empty and if so remove it
                            if (!parent.HasElements)
                            {
                                parent.Remove();
                                continue;
                            }
                        }
                    }

                    // Save project
                    if (changed)
                    {
                        proj.Save(projectFile);
                    }

                    // done.
                    break;

                case "reference":
                    // TODO: needs implementation
                    break;

                default:
                    throw new XmlException($"Command: {nameof(ExcludeReferenceCommand)} value: type  - unrecognized type specified");
            }
        }

        private void SaveProjectReference(string destPath, XElement projReference, bool append)
        {
            XDocument doc;
            XElement root;
            XElement itemGroup;

            if (append && File.Exists(destPath))
            {
                doc = XDocument.Load(destPath);
                root = doc.Root;
                itemGroup = root.Element("ItemGroup");
            }
            else
            {
                itemGroup = new XElement(projReference.Name.Namespace + "ItemGroup");
                root = new XElement(projReference.Name.Namespace + "Project", itemGroup);
                doc = new XDocument(new XDeclaration("1.0", System.Text.Encoding.UTF8.BodyName, "true"), root);
            }

            itemGroup.Add(projReference);

            doc.Save(destPath);
        }
    }
}