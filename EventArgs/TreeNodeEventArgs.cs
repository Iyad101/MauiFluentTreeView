// TreeNodeEventArgs.cs
namespace MauiFluentTreeView.EventsArgs
{
    /// <summary>
    /// Provides data for events related to a single <see cref="TreeNode"/>.
    /// </summary>
    public class TreeNodeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="TreeNode"/> associated with the event.
        /// </summary>
        public Models.TreeNode Node { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeEventArgs"/> class.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> associated with the event.</param>
        public TreeNodeEventArgs(Models.TreeNode node)
        {
            Node = node;
        }
    }
}