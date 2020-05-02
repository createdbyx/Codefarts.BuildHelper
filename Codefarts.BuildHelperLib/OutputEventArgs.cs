// <copyright file="OutputEventArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;

    public delegate void OutputEventHandler(object sender, OutputEventArgs e);

    public class OutputEventArgs : EventArgs
    {
        public OutputEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message
        {
            get;
        }
    }
}