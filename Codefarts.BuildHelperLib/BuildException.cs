// <copyright file="BuildException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Runtime.Serialization;

namespace Codefarts.BuildHelper
{
    using System;

    public class BuildException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildException"/> class.
        /// </summary>
        public BuildException()
        {
        }

        protected BuildException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BuildException(string message) : base(message)
        {
        }

        public BuildException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}