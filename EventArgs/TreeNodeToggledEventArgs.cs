// TreeNodeToggledEventArgs.cs
namespace MauiFluentTreeView.EventsArgs
{
    /// <summary>
    /// Provides data for events that occur when a <see cref="TreeNode"/>'s
    /// expanded state is toggled.
    /// </summary>
    public class TreeNodeToggledEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="TreeNode"/> whose expanded state was toggled.
        /// </summary>
        public Models.TreeNode Node { get; }

        /// <summary>
        /// Gets a value indicating the new expanded state of the node.
        /// True if expanded, false if collapsed.
        /// </summary>
        public bool IsExpanded { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeToggledEventArgs"/> class.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> associated with the event.</param>
        /// <param name="isExpanded">The new expanded state of the node.</param>
        public TreeNodeToggledEventArgs(Models.TreeNode node, bool isExpanded)
        {
            Node = node;
            IsExpanded = isExpanded;
        }
    }
}