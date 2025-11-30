using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;


namespace ZidUtilities.CommonCode.Win.Controls.AddressBar
{
    /// <summary>
    /// Collection of GenericNode objects that can be edited at design time
    /// </summary>
    public class GenericNodeCollection : IList<GenericNode>, IList
    {
        #region Events

        /// <summary>
        /// Event fired when the collection changes.
        /// Subscribers receive standard EventArgs.Empty when the collection is modified.
        /// </summary>
        public event EventHandler CollectionChanged;

        #endregion

        #region Class Variables

        private List<GenericNode> items = new List<GenericNode>();
        private GenericNode owner;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new GenericNodeCollection owned by the specified GenericNode.
        /// </summary>
        /// <param name="owner">The GenericNode that owns this collection. Each added node will have its Parent set to this owner.</param>
        public GenericNodeCollection(GenericNode owner)
        {
            this.owner = owner;
        }

        #endregion

        #region IList<GenericNode> Implementation

        /// <summary>
        /// Gets or sets the GenericNode at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The GenericNode at the specified index.</returns>
        public GenericNode this[int index]
        {
            get { return items[index]; }
            set
            {
                items[index] = value;
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// Gets the number of GenericNode items contained in the collection.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <returns>Always returns false; the collection is mutable.</returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds a GenericNode to the end of the collection and sets its Parent to the collection owner.
        /// </summary>
        /// <param name="item">The GenericNode to add. If null, the method does nothing.</param>
        public void Add(GenericNode item)
        {
            if (item != null)
            {
                items.Add(item);
                item.Parent = owner;
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// Removes all nodes from the collection and clears each node's Parent reference.
        /// </summary>
        public void Clear()
        {
            foreach (GenericNode node in items)
            {
                if (node != null)
                    node.Parent = null;
            }
            items.Clear();
            OnCollectionChanged();
        }

        /// <summary>
        /// Determines whether the collection contains a specific GenericNode.
        /// </summary>
        /// <param name="item">The GenericNode to locate in the collection.</param>
        /// <returns>True if item is found; otherwise, false.</returns>
        public bool Contains(GenericNode item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to a GenericNode array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(GenericNode[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of GenericNode objects.
        /// </summary>
        /// <returns>An IEnumerator&lt;GenericNode&gt; for the collection.</returns>
        public IEnumerator<GenericNode> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified GenericNode and returns the zero-based index of the first occurrence.
        /// </summary>
        /// <param name="item">The GenericNode to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence if found; otherwise, -1.</returns>
        public int IndexOf(GenericNode item)
        {
            return items.IndexOf(item);
        }

        /// <summary>
        /// Inserts a GenericNode into the collection at the specified index and sets its Parent to the owner.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The GenericNode to insert. If null, the method does nothing.</param>
        public void Insert(int index, GenericNode item)
        {
            if (item != null)
            {
                items.Insert(index, item);
                item.Parent = owner;
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific GenericNode from the collection and clears its Parent if removed.
        /// </summary>
        /// <param name="item">The GenericNode to remove. If null, the method returns false.</param>
        /// <returns>True if item was successfully removed; otherwise, false.</returns>
        public bool Remove(GenericNode item)
        {
            if (item != null)
            {
                bool result = items.Remove(item);
                if (result)
                {
                    item.Parent = null;
                    OnCollectionChanged();
                }
                return result;
            }
            return false;
        }

        /// <summary>
        /// Removes the GenericNode at the specified index and clears its Parent reference.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            GenericNode item = items[index];
            if (item != null)
                item.Parent = null;
            items.RemoveAt(index);
            OnCollectionChanged();
        }

        /// <summary>
        /// Returns a non-generic enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An IEnumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region IList Implementation

        /// <summary>
        /// Gets or sets the element at the specified index (non-generic interface).
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index as object.</returns>
        object IList.this[int index]
        {
            get { return items[index]; }
            set
            {
                items[index] = (GenericNode)value;
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the IList has a fixed size.
        /// </summary>
        /// <returns>Always returns false.</returns>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        /// <returns>Always returns false.</returns>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        /// <returns>An object to be used for synchronization. Returns the current instance.</returns>
        object ICollection.SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Adds an item to the IList and returns the index at which it was added.
        /// </summary>
        /// <param name="value">The object to add (should be a GenericNode).</param>
        /// <returns>The zero-based index at which the item was added.</returns>
        int IList.Add(object value)
        {
            Add((GenericNode)value);
            return items.Count - 1;
        }

        /// <summary>
        /// Determines whether the IList contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the IList (should be a GenericNode).</param>
        /// <returns>True if found; otherwise, false.</returns>
        bool IList.Contains(object value)
        {
            return Contains((GenericNode)value);
        }

        /// <summary>
        /// Returns the index of the specified object in the IList.
        /// </summary>
        /// <param name="value">The object to locate (should be a GenericNode).</param>
        /// <returns>The index of the object if found; otherwise, -1.</returns>
        int IList.IndexOf(object value)
        {
            return IndexOf((GenericNode)value);
        }

        /// <summary>
        /// Inserts an item into the IList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the item.</param>
        /// <param name="value">The object to insert (should be a GenericNode).</param>
        void IList.Insert(int index, object value)
        {
            Insert(index, (GenericNode)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the IList.
        /// </summary>
        /// <param name="value">The object to remove (should be a GenericNode).</param>
        void IList.Remove(object value)
        {
            Remove((GenericNode)value);
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at the specified index.
        /// </summary>
        /// <param name="array">The destination Array.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)items).CopyTo(array, index);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the CollectionChanged event to notify subscribers that the collection has changed.
        /// </summary>
        protected virtual void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, EventArgs.Empty);
        }

        #endregion
    }

    /// <summary>
    /// Custom collection editor for GenericNodeCollection
    /// </summary>
    public class GenericNodeCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the GenericNodeCollectionEditor for the specified collection type.
        /// </summary>
        /// <param name="type">The type of collection this editor will edit (typically GenericNodeCollection).</param>
        public GenericNodeCollectionEditor(Type type) : base(type)
        {
        }

        /// <summary>
        /// Returns the Type of item that will be created by the collection editor.
        /// </summary>
        /// <returns>The Type representing items in the collection (GenericNode).</returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof(GenericNode);
        }

        /// <summary>
        /// Creates a new instance of a collection item. Called by the collection editor when adding a new item.
        /// </summary>
        /// <param name="itemType">The Type of item to create. Expected to be GenericNode.</param>
        /// <returns>A newly created object instance suitable for insertion into the collection.</returns>
        protected override object CreateInstance(Type itemType)
        {
            GenericNode node = new GenericNode();
            node.DisplayName = "New Node";
            return node;
        }

        /// <summary>
        /// Returns the display text shown in the collection editor for a given item.
        /// </summary>
        /// <param name="value">The item to obtain display text for (expected to be a GenericNode).</param>
        /// <returns>A string to display for the item; uses DisplayName if available or a fallback string.</returns>
        protected override string GetDisplayText(object value)
        {
            GenericNode node = value as GenericNode;
            if (node != null)
            {
                if (!string.IsNullOrEmpty(node.DisplayName))
                    return node.DisplayName;
                else
                    return "(Unnamed Node)";
            }
            return base.GetDisplayText(value);
        }
    }
}
