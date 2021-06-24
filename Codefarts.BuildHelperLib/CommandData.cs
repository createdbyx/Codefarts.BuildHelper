// <copyright file="CommandData.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class CommandData : INotifyPropertyChanged
    {
        private readonly CommandDataCollection children;
        private readonly IDictionary<string, object> parameters;
        private string name;
        private CommandData parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandData"/> class.
        /// </summary>
        public CommandData()
        {
            this.parameters = new Dictionary<string, object>();
            this.children = new CommandDataCollection(this);
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
            this.children = new CommandDataCollection(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IDictionary<string, object> Parameters
        {
            get
            {
                return this.parameters;
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
                if (this.parent == value)
                {
                    return;
                }

                if (value != null)
                {
                    value.Children.Add(this);
                    return;
                }

                // value is null so we are removing the parent
                this.parent.Children.Remove(this);
            }
        }

        public CommandDataCollection Children
        {
            get
            {
                return this.children;
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

        /// <summary>
        /// Assigns the parent to the internal <see cref="parent"/> field.
        /// </summary>
        /// <param name="owner">The control that will be the owner/parent of this control.</param>
        internal virtual void AssignParent(CommandData owner)
        {
            if (this.parent != owner)
            {
                this.parent = owner;
                this.OnPropertyChanged(nameof(this.Parent));
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