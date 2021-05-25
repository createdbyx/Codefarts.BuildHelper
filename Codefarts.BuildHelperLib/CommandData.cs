﻿// <copyright file="CommandData.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class CommandData : INotifyPropertyChanged
    {
        private string name;
        private ObservableCollection<CommandData> children;
        private IDictionary<string, object> parameters;
        private CommandData parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandData"/> class.
        /// </summary>
        public CommandData()
        {
            this.parameters = new Dictionary<string, object>();
        }

        public CommandData(string name, IDictionary<string, object> parameters)
            : this(name)
        {
            this.parameters = parameters;
        }

        public CommandData(string name)
        {
            this.name = name;
            this.parameters = new Dictionary<string, object>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IDictionary<string, object> Parameters
        {
            get
            {
                return this.parameters;
            }

            set
            {
                var currentValue = this.parameters;
                if (currentValue != value)
                {
                    this.parameters = value;
                    this.OnPropertyChanged(nameof(this.Parameters));
                }
            }
        }

        public CommandData Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                var currentValue = this.parent;
                if (currentValue != value)
                {
                    this.parent = value;
                    this.OnPropertyChanged(nameof(this.Parent));
                }
            }
        }

        public ObservableCollection<CommandData> Children
        {
            get
            {
                return this.children;
            }

            set
            {
                var currentValue = this.children;
                if (currentValue != value)
                {
                    this.children = value;
                    this.OnPropertyChanged(nameof(this.Children));
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                var currentValue = this.name;
                if (currentValue != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(nameof(this.Name));
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