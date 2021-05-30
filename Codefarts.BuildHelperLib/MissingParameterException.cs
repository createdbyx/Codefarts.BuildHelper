// <copyright file="MissingParameterException.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Runtime.Serialization;

    public class MissingParameterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingParameterException"/> class.
        /// </summary>
        public MissingParameterException()
        {
        }

        protected MissingParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MissingParameterException(string message) : base(message)
        {
        }

        public MissingParameterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}