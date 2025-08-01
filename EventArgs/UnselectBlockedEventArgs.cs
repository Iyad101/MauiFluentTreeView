// UnselectBlockedEventArgs.cs
namespace MauiFluentTreeView.EventsArgs
{
    /// <summary>
    /// Provides data for events that occur when a node's unselection is blocked,
    /// typically due to hierarchical selection rules (e.g., trying to unselect
    /// a parent with selected children).
    /// </summary>
    public class UnselectBlockedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the display title of the node whose unselection was blocked.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the unique key of the node whose unselection was blocked.
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// Gets the count of selected children of the node whose unselection was blocked.
        /// </summary>
        public int SelectedChildCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnselectBlockedEventArgs"/> class.
        /// </summary>
        /// <param name="title">The title of the node.</param>
        /// <param name="key">The key of the node.</param>
        /// <param name="selectedChildCount">The number of selected children.</param>
        public UnselectBlockedEventArgs(string title, object key, int selectedChildCount)
        {
            Title = title;
            Key = key;
            SelectedChildCount = selectedChildCount;
        }
    }
}
