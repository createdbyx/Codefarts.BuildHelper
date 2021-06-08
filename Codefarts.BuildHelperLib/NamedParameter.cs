// <copyright file="NamedParameter.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class NamedParameter : Attribute
    {
        public NamedParameter(string name, Type type, bool required, string description)
        {
            this.Name = name;
            this.Type = type;
            this.Required = required;
            this.Description = description;
        }

        public string Name { get; }

        public Type Type { get; }

        public bool Required { get; }

        public string Description { get; }
    }
}