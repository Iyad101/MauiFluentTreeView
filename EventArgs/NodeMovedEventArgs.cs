// NodeMovedEventArgs.cs
namespace MauiFluentTreeView.EventsArgs
{
    /// <summary>
    /// Provides data for events that occur when a <see cref="TreeNode"/> is moved
    /// from one parent to another, or to/from the root level.
    /// </summary>
    public class NodeMovedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="TreeNode"/> that was moved.
        /// </summary>
        public Models.TreeNode Node { get; }

        /// <summary>
        /// Gets the previous parent <see cref="TreeNode"/> of the moved node.
        /// Null if the node was previously a root node.
        /// </summary>
        public Models.TreeNode? OldParent { get; }

        /// <summary>
        /// Gets the new parent <see cref="TreeNode"/> of the moved node.
        /// Null if the node is now a root node.
        /// </summary>
        public Models.TreeNode? NewParent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMovedEventArgs"/> class.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> that was moved.</param>
        /// <param name="oldParent">The old parent of the node.</param>
        /// <param name="newParent">The new parent of the node.</param>
        public NodeMovedEventArgs(Models.TreeNode node, Models.TreeNode? oldParent, Models.TreeNode? newParent)
        {
            Node = node;
            OldParent = oldParent;
            NewParent = newParent;
        }
    }
}