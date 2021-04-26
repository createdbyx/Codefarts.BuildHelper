// <copyright file="Node.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class Node : INotifyPropertyChanged
    {
        private string name;
        private ObservableCollection<Node> children;
        private IDictionary<string, object> parameters;
        private Node parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            this.parameters = new Dictionary<string, object>();
        }

        public Node(string name, IDictionary<string, object> parameters)
            : this(name)
        {
            this.parameters = parameters;
        }

        public Node(string name)
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

        public Node Parent
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

        public ObservableCollection<Node> Children
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