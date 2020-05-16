// <copyright file="ExecuteCommandArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class ExecuteCommandArgs
    {
        public ExecuteCommandArgs(Action<string> output, IDictionary<string, string> variables, XElement element)
        {
            this.Output = output;
            this.Variables = variables;
            this.Element = element;
        }

        public XElement Element
        {
            get;
        }

        public Action<string> Output
        {
            get;
        }

        public IDictionary<string, string> Variables
        {
            get;
        }
    }
}