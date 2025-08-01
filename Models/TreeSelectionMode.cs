namespace MauiFluentTreeView.Models
{
    /// <summary>
    /// Defines the selection modes available for the TreeView control.
    /// </summary>
    public enum TreeSelectionMode
    {
        /// <summary>
        /// Allows only one node to be selected at a time. Selecting a new node
        /// will deselect any previously selected node.
        /// </summary>
        Single,
        /// <summary>
        /// Allows multiple nodes to be selected simultaneously.
        /// </summary>
        Multiple,
        /// <summary>
        /// Disables all selection in the TreeView.
        /// </summary>
        None
    }
}