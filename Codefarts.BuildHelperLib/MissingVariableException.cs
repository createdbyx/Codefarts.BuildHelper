// <copyright file="MissingVariableException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Runtime.Serialization;

    public class MissingVariableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingVariableException"/> class.
        /// </summary>
        public MissingVariableException()
        {
        }

        protected MissingVariableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MissingVariableException(string message) : base(message)
        {
        }

        public MissingVariableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}