using System;
using System.Drawing;

namespace ZidUtilities.CommonCode.Win.Controls.AddressBar
{
    /// <summary>
    /// Represents a node in an address-like hierarchy used by the AddressBar control.
    /// Implementations provide a display name, icon, identification, children enumeration,
    /// and operations to update and clone the node.
    /// </summary>
    public interface IAddressNode
    {
        /// <summary>
        /// Gets/Sets the parent of this node.
        /// </summary>
        /// <value>
        /// The parent <see cref="IAddressNode"/> instance, or null if this node has no parent (root).
        /// Setting this property associates or dissociates this node with a parent container.
        /// </value>
        IAddressNode Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the Display name of this node.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> containing the user-visible name for this node. Implementations
        /// may update this value during <see cref="UpdateNode"/>.
        /// </value>
        String DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Icon that represents this node type.
        /// </summary>
        /// <value>
        /// An <see cref="Icon"/> instance used when rendering the node in the UI.
        /// This property may return null if no icon is associated.
        /// </value>
        Icon Icon
        {
            get;
        }

        /// <summary>
        /// Gets the Unique ID for this node.
        /// </summary>
        /// <value>
        /// An <see cref="Object"/> that uniquely identifies this node within its context.
        /// Comparison semantics are implementation-defined (for example: reference equality,
        /// a specific ID value, or <see cref="Object.Equals"/>).
        /// </value>
        Object UniqueID
        {
            get;
        }

        /// <summary>
        /// Gets/Sets any user defined extra data for this node.
        /// </summary>
        /// <value>
        /// An arbitrary object assigned by consumers of the node to store additional metadata.
        /// </value>
        Object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an array of Child Nodes.
        /// </summary>
        /// <value>
        /// An array of immediate child <see cref="IAddressNode"/> instances. Implementations
        /// may return an empty array if there are no children.
        /// </value>
        IAddressNode[] Children
        {
            get;
        }

        /// <summary>
        /// Updates the internal state of this node and refreshes any associated details.
        /// </summary>
        /// <remarks>
        /// Implementations should use this method to gather or refresh information
        /// relevant to the node (for example: enumerating children, updating the
        /// display name, icon, or other cached data). This method updates the
        /// node instance in-place and does not produce a separate return value.
        /// Calling <see cref="UpdateNode"/> may be required before accessing
        /// properties such as <see cref="Children"/> or <see cref="DisplayName"/>.
        /// </remarks>
        /// <returns>None. This method performs updates on the current instance.</returns>
        void UpdateNode();

        /// <summary>
        /// Searches for and returns a child node that matches the specified unique identifier.
        /// </summary>
        /// <param name="uniqueID">
        /// The unique identifier that identifies the child node to find. Comparison semantics
        /// are implementation-defined (for example: reference equality, <see cref="Object.Equals"/>,
        /// or a specific identifier match). Implementations should document the exact
        /// comparison behavior they use.
        /// </param>
        /// <param name="recursive">
        /// If true, the search will recurse into descendant nodes (children of children);
        /// if false, the search will be limited to the immediate child collection.
        /// </param>
        /// <returns>
        /// The matching <see cref="IAddressNode"/> if a child with the given <paramref name="uniqueID"/>
        /// is found; otherwise, null.
        /// </returns>
        IAddressNode GetChild(object uniqueID, bool recursive);

        /// <summary>
        /// Creates a clone of this node.
        /// </summary>
        /// <remarks>
        /// The exact cloning semantics are implementation-specific: implementations should document
        /// whether the clone copies child nodes, the <see cref="Tag"/>, and other state, or whether
        /// it performs a shallow copy. The returned node should be a separate instance such that
        /// modifications to the clone do not affect the original.
        /// </remarks>
        /// <returns>
        /// A new <see cref="IAddressNode"/> instance that represents a copy of this node.
        /// The caller receives ownership of the clone and may modify or dispose it independently.
        /// </returns>
        IAddressNode Clone();
    }
}
