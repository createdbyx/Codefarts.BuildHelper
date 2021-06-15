// <copyright file="PluginCollection.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides an observable collection for storing plugin references.
    /// </summary>
    public class PluginCollection : ObservableCollection<ICommandPlugin>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCollection"/> class.
        /// </summary>
        public PluginCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCollection"/> class.
        /// </summary>
        /// <param name="collection"></param>
        public PluginCollection(IEnumerable<ICommandPlugin> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCollection"/> class.
        /// </summary>
        /// <param name="list"></param>
        public PluginCollection(List<ICommandPlugin> list)
            : base(list)
        {
        }
    }
}