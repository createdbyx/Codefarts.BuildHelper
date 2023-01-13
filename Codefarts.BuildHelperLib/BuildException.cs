// <copyright file="BuildException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper;

using System;
using System.Runtime.Serialization;

// TODO: This exception should be moved into whatever plug-in needs it it should not be part of the core library 
public class BuildException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildException"/> class.
    /// </summary>
    public BuildException()
    {
    }

    public BuildException(string message)
        : base(message)
    {
    }

    public BuildException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected BuildException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}