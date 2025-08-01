namespace MauiFluentTreeView.Models
{
    /// <summary>
    /// Represents a flattened data node used as a source for building the hierarchical TreeView.
    /// This class contains basic properties for a node without hierarchical references within itself.
    /// </summary>
    public class FlatNode
    {
        /// <summary>
        /// Gets or sets the unique identifier for this node.
        /// Changed Id and ParentId to object to support various key types (int, long, string, Guid, etc.).
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the parent node.
        /// ParentId can be null, 0, or any other object that signifies a root or a parent key.
        /// A null or zero ParentId typically indicates a root-level node.
        /// </summary>
        public object? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the display title of the node.
        /// Initializes to an empty string to ensure it's never null.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional icon identifier or path for the node.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the node is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the node is selected.
        /// This property is typically used for initial state and might be
        /// synchronized with the TreeView's selection logic.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}