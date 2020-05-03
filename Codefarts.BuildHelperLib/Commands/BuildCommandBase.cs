// <copyright file="BuildCommandBase.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public abstract class BuildCommandBase : IBuildCommand
    {
        protected Action<string> WriteOutput
        {
            get;
            set;
        }

        public BuildCommandBase(Action<string> writeOutput)
        {
            this.WriteOutput = writeOutput;
        }

        public abstract string Name
        {
            get;
        }

        public void Output(string message, params object[] args)
        {
            this.WriteOutput(string.Format(this.Name + ": " + message, args));
        }

        public abstract void Execute(IDictionary<string, string> variables, XElement data);
    }
}