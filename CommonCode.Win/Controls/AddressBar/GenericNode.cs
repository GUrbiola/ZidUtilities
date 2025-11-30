using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ZidUtilities.CommonCode.Win.Controls.AddressBar
{
    /// <summary>
    /// Generic node implementation for the AddressBar control that supports event-driven navigation.
    /// This node can be configured at design time and allows building navigation hierarchies dynamically.
    /// </summary>
    [DesignerCategory("Component")]
    [ToolboxItem(true)]
    public class GenericNode : Component, IAddressNode
    {
        #region Events

        /// <summary>
        /// Event fired when the node needs to update its children.
        /// Subscribe to this event to populate children dynamically.
        /// </summary>
        public event EventHandler<UpdateChildrenEventArgs> UpdateChildren;

        /// <summary>
        /// Event fired when a specific child needs to be retrieved by UniqueID.
        /// </summary>
        public event EventHandler<GetChildEventArgs> GetChildRequested;

        #endregion

        #region Class Variables

        /// <summary>
        /// Stores the parent node to this node
        /// </summary>
        private IAddressNode parent = null;

        /// <summary>
        /// Stores the display name of this Node
        /// </summary>
        private string displayName = "Node";

        /// <summary>
        /// Stores the unique ID for this node
        /// </summary>
        private object uniqueId = null;

        /// <summary>
        /// Stores the Icon for this node
        /// </summary>
        private Icon icon = null;

        /// <summary>
        /// Stores the child nodes
        /// </summary>
        private List<IAddressNode> children = new List<IAddressNode>();

        /// <summary>
        /// Stores user defined data for this node
        /// </summary>
        private object tag = null;

        /// <summary>
        /// Indicates whether children have been loaded
        /// </summary>
        private bool childrenLoaded = false;

        /// <summary>
        /// Indicates whether to automatically load children on UpdateNode
        /// </summary>
        private bool autoLoadChildren = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets the parent node to this node
        /// </summary>
        [Browsable(false)]
        public IAddressNode Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// Gets/Sets the Display name of this node
        /// </summary>
        [Category("Appearance")]
        [Description("The display name shown in the address bar")]
        [DefaultValue("Node")]
        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        /// <summary>
        /// Gets/Sets the Icon that represents this node type.
        /// </summary>
        [Category("Appearance")]
        [Description("The icon displayed for this node")]
        [DefaultValue(null)]
        public Icon Icon
        {
            get { return this.icon; }
            set { this.icon = value; }
        }

        /// <summary>
        /// Returns the Unique Id for this node
        /// </summary>
        [Category("Data")]
        [Description("Unique identifier for this node")]
        [DefaultValue(null)]
        public object UniqueID
        {
            get { return this.uniqueId ?? this.displayName; }
            set { this.uniqueId = value; }
        }

        /// <summary>
        /// Gets/Sets user defined data for this object
        /// </summary>
        [Category("Data")]
        [Description("User-defined data associated with this node")]
        [DefaultValue(null)]
        [Browsable(false)]
        public object Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        /// <summary>
        /// Gets the children of this node
        /// </summary>
        [Browsable(false)]
        public IAddressNode[] Children
        {
            get { return this.children.ToArray(); }
        }

        /// <summary>
        /// Gets/Sets the child nodes collection for design-time editing
        /// </summary>
        [Category("Data")]
        [Description("Collection of child nodes")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(GenericNodeCollectionEditor), typeof(UITypeEditor))]
        public GenericNodeCollection ChildNodes { get; private set; }

        /// <summary>
        /// Gets/Sets whether to automatically fire UpdateChildren event when UpdateNode is called
        /// </summary>
        [Category("Behavior")]
        [Description("Automatically fire UpdateChildren event when UpdateNode is called")]
        [DefaultValue(true)]
        public bool AutoLoadChildren
        {
            get { return this.autoLoadChildren; }
            set { this.autoLoadChildren = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for GenericNode.
        /// Initializes internal ChildNodes collection and subscribes to its CollectionChanged event.
        /// </summary>
        public GenericNode()
        {
            this.ChildNodes = new GenericNodeCollection(this);
            this.ChildNodes.CollectionChanged += ChildNodes_CollectionChanged;
        }

        /// <summary>
        /// Constructor for design-time support.
        /// Adds this component to the provided container.
        /// </summary>
        /// <param name="container">Container for the component. May be null.</param>
        public GenericNode(IContainer container) : this()
        {
            if (container != null)
            {
                container.Add(this);
            }
        }

        /// <summary>
        /// Creates a GenericNode with a display name.
        /// Sets the UniqueID to the display name by default.
        /// </summary>
        /// <param name="displayName">Display name for the node.</param>
        public GenericNode(string displayName) : this()
        {
            this.displayName = displayName;
            this.uniqueId = displayName;
        }

        /// <summary>
        /// Creates a GenericNode with a display name and unique ID.
        /// </summary>
        /// <param name="displayName">Display name for the node.</param>
        /// <param name="uniqueId">Unique identifier for the node.</param>
        public GenericNode(string displayName, object uniqueId) : this()
        {
            this.displayName = displayName;
            this.uniqueId = uniqueId;
        }

        /// <summary>
        /// Creates a GenericNode with display name, unique ID and icon.
        /// </summary>
        /// <param name="displayName">Display name for the node.</param>
        /// <param name="uniqueId">Unique identifier for the node.</param>
        /// <param name="icon">Icon for the node (may be null).</param>
        public GenericNode(string displayName, object uniqueId, Icon icon) : this()
        {
            this.displayName = displayName;
            this.uniqueId = uniqueId;
            this.icon = icon;
        }

        #endregion

        #region IAddressNode Implementation

        /// <summary>
        /// Updates the contents of this node. Fires the UpdateChildren event if AutoLoadChildren is true
        /// and children have not already been loaded.
        /// </summary>
        /// <remarks>
        /// If AutoLoadChildren is true and the children have not been loaded, this method sets the
        /// internal childrenLoaded flag, raises the UpdateChildren event, and replaces the internal
        /// children collection with any nodes provided by event subscribers.
        /// </remarks>
        public void UpdateNode()
        {
            if (this.autoLoadChildren && !this.childrenLoaded)
            {
                this.childrenLoaded = true;

                // Fire the event to allow subscribers to populate children
                if (UpdateChildren != null)
                {
                    UpdateChildrenEventArgs args = new UpdateChildrenEventArgs();
                    UpdateChildren(this, args);

                    // Add children from the event
                    if (args.Children != null)
                    {
                        this.children.Clear();
                        foreach (IAddressNode child in args.Children)
                        {
                            child.Parent = this;
                            this.children.Add(child);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns an individual child node, based on a given unique ID.
        /// </summary>
        /// <param name="uniqueID">Unique object used to identify the child.</param>
        /// <param name="recursive">Indicates whether the search should be performed recursively into descendants.</param>
        /// <returns>
        /// The matching <see cref="IAddressNode"/> if found; otherwise null. If <paramref name="recursive"/>
        /// is true the method searches descendants; otherwise only direct children are inspected.
        /// If not found in memory, the <see cref="GetChildRequested"/> event is raised to allow custom retrieval.
        /// </returns>
        public IAddressNode GetChild(object uniqueID, bool recursive)
        {
            // First, try to find in direct children
            foreach (IAddressNode child in this.children)
            {
                if (child.UniqueID != null && child.UniqueID.Equals(uniqueID))
                    return child;
            }

            // If recursive, search in children's children
            if (recursive)
            {
                foreach (IAddressNode child in this.children)
                {
                    IAddressNode found = child.GetChild(uniqueID, true);
                    if (found != null)
                        return found;
                }
            }

            // Fire the event to allow custom retrieval logic
            if (GetChildRequested != null)
            {
                GetChildEventArgs args = new GetChildEventArgs(uniqueID, recursive);
                GetChildRequested(this, args);
                if (args.ChildNode != null)
                    return args.ChildNode;
            }

            return null;
        }

        /// <summary>
        /// Creates a clone of this node including display properties, tag and event handlers.
        /// Child nodes defined in the design-time ChildNodes collection are deep-cloned.
        /// </summary>
        /// <returns>A new <see cref="IAddressNode"/> which is a clone of the current node.</returns>
        public IAddressNode Clone()
        {
            GenericNode clone = new GenericNode(this.displayName, this.uniqueId, this.icon);
            clone.tag = this.tag;
            clone.autoLoadChildren = this.autoLoadChildren;

            // Copy event handlers
            if (this.UpdateChildren != null)
                clone.UpdateChildren = (EventHandler<UpdateChildrenEventArgs>)this.UpdateChildren.Clone();

            if (this.GetChildRequested != null)
                clone.GetChildRequested = (EventHandler<GetChildEventArgs>)this.GetChildRequested.Clone();

            // Clone child nodes from ChildNodes collection
            foreach (GenericNode child in this.ChildNodes)
            {
                GenericNode childClone = (GenericNode)child.Clone();
                childClone.Parent = clone;
                clone.ChildNodes.Add(childClone);
            }

            return clone;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Manually adds a child node to this node.
        /// </summary>
        /// <param name="child">Child node to add. If null, the method does nothing.</param>
        public void AddChild(IAddressNode child)
        {
            if (child != null)
            {
                child.Parent = this;
                this.children.Add(child);
            }
        }

        /// <summary>
        /// Manually removes a child node from this node.
        /// </summary>
        /// <param name="child">Child node to remove. If null or not present, the method does nothing.</param>
        public void RemoveChild(IAddressNode child)
        {
            if (child != null)
            {
                this.children.Remove(child);
                child.Parent = null;
            }
        }

        /// <summary>
        /// Clears all child nodes from this node, disconnecting their Parent references and marking children as not loaded.
        /// </summary>
        public void ClearChildren()
        {
            foreach (IAddressNode child in this.children)
            {
                child.Parent = null;
            }
            this.children.Clear();
            this.childrenLoaded = false;
        }

        /// <summary>
        /// Resets the node so children will be reloaded on next UpdateNode call.
        /// </summary>
        public void Reset()
        {
            this.childrenLoaded = false;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handler for changes in the design-time ChildNodes collection.
        /// Synchronizes the internal children list with the ChildNodes collection and sets Parent references.
        /// </summary>
        /// <param name="sender">Event sender (the ChildNodes collection).</param>
        /// <param name="e">Event arguments (unused).</param>
        private void ChildNodes_CollectionChanged(object sender, EventArgs e)
        {
            // Sync the ChildNodes collection with the internal children list
            this.children.Clear();
            foreach (GenericNode node in this.ChildNodes)
            {
                node.Parent = this;
                this.children.Add(node);
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Releases resources used by the component.
        /// Disposes the node's icon (if any) and clears child nodes when disposing is true.
        /// </summary>
        /// <param name="disposing">True when called from Dispose; false when called from finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (icon != null)
                {
                    icon.Dispose();
                    icon = null;
                }

                ClearChildren();
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    #region Event Arguments

    /// <summary>
    /// Event arguments for the UpdateChildren event.
    /// Contains a list of children that subscribers can populate.
    /// </summary>
    public class UpdateChildrenEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the children to be added to the node.
        /// Subscribers should populate this list with new child nodes during the UpdateChildren event.
        /// </summary>
        public List<IAddressNode> Children { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="UpdateChildrenEventArgs"/> with an empty children list.
        /// </summary>
        public UpdateChildrenEventArgs()
        {
            Children = new List<IAddressNode>();
        }
    }

    /// <summary>
    /// Event arguments for the GetChild event.
    /// Carries the unique identifier being searched and whether the search is recursive,
    /// and allows the event handler to provide a resulting child node.
    /// </summary>
    public class GetChildEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the unique ID being searched for.
        /// </summary>
        public object UniqueID { get; private set; }

        /// <summary>
        /// Gets whether the search should be recursive.
        /// </summary>
        public bool Recursive { get; private set; }

        /// <summary>
        /// Gets or sets the child node result. Event handlers may set this property to return a node.
        /// </summary>
        public IAddressNode ChildNode { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="GetChildEventArgs"/>.
        /// </summary>
        /// <param name="uniqueID">The unique identifier to search for.</param>
        /// <param name="recursive">True to indicate search should be recursive; otherwise false.</param>
        public GetChildEventArgs(object uniqueID, bool recursive)
        {
            this.UniqueID = uniqueID;
            this.Recursive = recursive;
            this.ChildNode = null;
        }
    }

    #endregion
}
