// <copyright file="NamedParameter.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper;

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class NamedParameter : Attribute
{
    public NamedParameter(string name, Type type, bool required = false, string description = null)
    {
        this.Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Required = required;
        this.Description = description;
    }

    public string Name { get; }

    public Type Type { get; }

    public bool Required { get; }

    public string Description { get; }
}