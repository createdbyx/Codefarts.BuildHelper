// <copyright file="ExecuteCommandArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ExecuteCommandArgs : INotifyPropertyChanged
    {
        public ExecuteCommandArgs(Action<string> output, IDictionary<string, string> variables, Node command)
        {
            // we wrap output callback here to ensure any call to it does not throw null reference exceptions
            this.Output = msg =>
            {
                if (output != null)
                {
                    output(msg);
                }
            };
            this.Variables = variables;
            this.Command = command;
        }

        //public ExecuteCommandArgs(Action<string> output, IDictionary<string, string> variables, IDictionary<string, object> parameters)
        //{
        //    // we wrap output callback here to ensure any call to it does not throw null reference exceptions
        //    this.Output = msg =>
        //    {
        //        if (output != null)
        //        {
        //            output(msg);
        //        }
        //    };
        //    this.Variables = variables;
        //    this.parameters = parameters;
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        public Node Command
        {
            get;
        }

        public Action<string> Output
        {
            get;
        }

        public IDictionary<string, object> Parameters
        {
            get
            {
                return this.Command.Parameters;
            }
        }

        public IDictionary<string, string> Variables
        {
            get;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}