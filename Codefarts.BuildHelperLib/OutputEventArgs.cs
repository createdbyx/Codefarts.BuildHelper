// <copyright file="OutputEventArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.ComponentModel;

    public delegate void OutputEventHandler(object sender, OutputEventArgs e);

    public class OutputEventArgs : EventArgs, INotifyPropertyChanged
    {
        private string message;
        private string type;
        private string category;

        public event PropertyChangedEventHandler PropertyChanged;

        public OutputEventArgs(string message)
        {
            this.Message = message;
        }

        public OutputEventArgs(string message, string type, string category)
        {
            this.message = message;
            this.type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentException(nameof(type)) : type;
            this.category = string.IsNullOrWhiteSpace(category) ? throw new ArgumentException(nameof(category)) : category;
        }

        public OutputEventArgs(string message, string type)
        {
            this.message = message;
            this.type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentException(nameof(type)) : type;
        }

        public string Category
        {
            get
            {
                return this.category;
            }

            private set
            {
                var currentValue = this.category;
                if (currentValue != value)
                {
                    this.category = value;
                    this.OnPropertyChanged(nameof(this.Category));
                }
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }

            private set
            {
                var currentValue = this.type;
                if (currentValue != value)
                {
                    this.type = value;
                    this.OnPropertyChanged(nameof(this.Type));
                }
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            private set
            {
                var currentValue = this.message;
                if (currentValue != value)
                {
                    this.message = value;
                    this.OnPropertyChanged(nameof(this.Message));
                }
            }
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