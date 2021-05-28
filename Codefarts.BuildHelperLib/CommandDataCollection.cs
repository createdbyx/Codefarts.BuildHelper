// <copyright file="CommandDataCollection.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.ObjectModel;

    public class CommandDataCollection : ObservableCollection<CommandData>
    {
        /// <summary>
        /// The control that owns this collection.
        /// </summary>
        private CommandData owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDataCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner of the collection.</param>
        public CommandDataCollection(CommandData owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Gets the owner of this collection.
        /// </summary>
        public CommandData Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>Inserts an item into the collection at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        /// <remarks>
        /// If the item being added has a parent assigned it will remove itself from the parent <see cref="CommandData.Children"/> collection before setting the parent.
        /// </remarks>
        protected override void InsertItem(int index, CommandData item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Parent == this.owner)
            {
                return;
            }

            if (item.Parent != null)
            {
                item.Parent.Children.Remove(item);
            }

            base.InsertItem(index, item);
            item.AssignParent(this.owner);
        }

        /// <summary>Removes all items from the collection.</summary>
        protected override void ClearItems()
        {
            while (this.Count > 0)
            {
                this.RemoveItem(0);
            }

            base.ClearItems();
        }

        /// <summary>Removes the item at the specified index of the collection.</summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            if (item.Parent == this.owner)
            {
                base.RemoveItem(index);
                item.AssignParent(null);
            }
        }
    }
}