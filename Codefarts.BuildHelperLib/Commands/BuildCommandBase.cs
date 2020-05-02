// <copyright file="BuildCommandBase.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public abstract class BuildCommandBase : IBuildCommand
    {
        private BuildHelper BuildHelper
        {
            get;
        }

        public BuildCommandBase(BuildHelper buildHelper)
        {
            this.BuildHelper = buildHelper;
        }

        public abstract string Name
        {
            get;
        }

        public void Output(string message, params object[] args)
        {
            this.BuildHelper.Output(this.Name + ": " + message, args);
        }

        public abstract void Execute(IDictionary<string, string> variables, XElement data);
    }
}