// SelectedNodesChangedEventArgs.cs
using System.Collections.ObjectModel;

namespace MauiFluentTreeView.EventsArgs
{
    /// <summary>
    /// Provides data for events that occur when the collection of
    /// selected <see cref="TreeNode"/> objects in the TreeView changes.
    /// </summary>
    public class SelectedNodesChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the current collection of selected <see cref="TreeNode"/> objects.
        /// </summary>
        public ObservableCollection<Models.TreeNode> SelectedNodes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedNodesChangedEventArgs"/> class.
        /// </summary>
        /// <param name="selectedNodes">The updated collection of selected nodes.</param>
        public SelectedNodesChangedEventArgs(ObservableCollection<Models.TreeNode> selectedNodes)
        {
            SelectedNodes = selectedNodes;
        }
    }
}
