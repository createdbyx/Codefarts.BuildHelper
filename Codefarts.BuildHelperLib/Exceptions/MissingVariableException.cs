// <copyright file="MissingVariableException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Codefarts.BuildHelper.Exceptions;

public class MissingVariableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingVariableException"/> class.
    /// </summary>
    public MissingVariableException()
    {
    }

    public MissingVariableException(string variableName)
        : base($"Command requires a '{variableName}' variable to run.")
    {
        this.VariableName = variableName;
    }

    public MissingVariableException(string variableName, string message)
        : base(message)
    {
        this.VariableName = variableName;
    }

    public MissingVariableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected MissingVariableException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public string VariableName { get; }
}