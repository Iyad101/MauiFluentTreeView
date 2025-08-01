
// FluentTreeViewTest.Models.TreeNode.cs
#region Usings
using MauiFluentTreeView.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
#endregion

namespace MauiFluentTreeView.Models
{
    /// <summary>
    /// Represents a single node within the TreeView structure, implementing INotifyPropertyChanged
    /// for data binding updates.
    /// </summary>
    public class TreeNode : INotifyPropertyChanged
    {
        #region Internal Properties
        /// <summary>
        /// Indicates if the node's depth needs to be recalculated.
        /// </summary>
        internal bool DepthDirty { get; set; }
        /// <summary>
        /// Indicates if the node's expansion state needs a visual refresh.
        /// </summary>
        internal bool ExpansionDirty { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the parent TreeViewControl instance this node belongs to.
        /// </summary>
        public TreeViewControl? ParentControl { get; set; }

        #region Private Backing Fields
        private TreeSelectionMode _selectionMode;
        private string _title;
        private bool _isExpanded;
        private string? _icon;
        private TreeNode? _parent;
        private bool _isSelected;
        private int _depth;
        private bool _isLeaf;
        private bool _isActive;
        private bool _shouldHighlight;
        #endregion

        /// <summary>
        /// Gets or sets the unique key identifying this node.
        /// </summary>
        public object Key { get; set; }
        /// <summary>
        /// Gets or sets the key of this node's parent. Null if it's a root node.
        /// </summary>
        public object? ParentKey { get; set; }
        /// <summary>
        /// The collection of child nodes.
        /// </summary>
        private ObservableCollection<TreeNode> _children = new();

        /// <summary>
        /// Gets or sets the depth of the node in the tree hierarchy (0 for root nodes).
        /// </summary>
        public int Depth
        {
            get => _depth;
            set
            {
                if (_depth != value)
                {
                    _depth = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selection mode applicable to this node.
        /// </summary>
        public TreeSelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                if (_selectionMode == value) return;
                _selectionMode = value;
                OnPropertyChanged();
                NotifySelectionChanged();
                RecalculateHighlight();
            }
        }

        /// <summary>
        /// Gets or sets the display title of the node.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the collection of child nodes for this node.
        /// Automatically updates <see cref="IsLeaf"/> when changed.
        /// </summary>
        public ObservableCollection<TreeNode> Children
        {
            get => _children;
            set
            {
                if (_children != value)
                {
                    if (_children != null)
                        _children.CollectionChanged -= OnChildrenCollectionChanged;
                    _children = value ?? new ObservableCollection<TreeNode>();
                    _children.CollectionChanged += OnChildrenCollectionChanged;
                    OnPropertyChanged();
                    UpdateIsLeaf();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this node has no children.
        /// </summary>
        public bool IsLeaf
        {
            get => _isLeaf;
            private set
            {
                if (_isLeaf != value)
                {
                    _isLeaf = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is expanded in the UI.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the icon string for the node.
        /// </summary>
        public string? Icon
        {
            get => _icon;
            set
            {
                if (_icon == value) return;
                _icon = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the parent <see cref="TreeNode"/> of this node.
        /// </summary>
        public TreeNode? Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;
                _parent = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is currently selected.
        /// Includes logic for hierarchical selection blocking.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                // Logic to block deselection if hierarchical selection is on and node has selected children.
                if (!value && SelectionMode == TreeSelectionMode.Multiple &&
                    ParentControl?.UseHierarchicalSelection == true)
                {
                    bool hasSelectedChildren = Children.Any(c => c.IsSelected);
                    if (hasSelectedChildren)
                    {
                        Debug.WriteLine($"Blocked visual deselection of '{Title}' due to selected children.");
                        // Restore prior visual if user toggled accidentally
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            IsSelected = true;
                            ShouldHighlight = true;
                        });
                        ParentControl?.OnHierarchicalUnselectBlocked?.Invoke(this);
                        return;
                    }
                }

                _isSelected = value;
                OnPropertyChanged();
                NotifySelectionChanged();
                RecalculateHighlight();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node should be highlighted.
        /// </summary>
        public bool ShouldHighlight
        {
            get => _shouldHighlight;
            private set
            {
                if (_shouldHighlight == value) return;
                _shouldHighlight = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command to toggle the selected state of the node.
        /// </summary>
        public ICommand ToggleSelectCommand { get; }

        /// <summary>
        /// Command to toggle the expanded state of the node.
        /// Also handles single selection logic when expanding.
        /// </summary>
        public ICommand ToggleExpandCommand { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class.
        /// This is the default constructor.
        /// </summary>
        public TreeNode()
        {
            _children = new ObservableCollection<TreeNode>();
            _children.CollectionChanged += OnChildrenCollectionChanged;
            ToggleSelectCommand = new Command(() => IsSelected = !IsSelected);
            ToggleExpandCommand = new Command(() =>
            {
                IsExpanded = !IsExpanded;
                // If in single selection mode and expanding, ensure only this node is selected.
                if (SelectionMode == TreeSelectionMode.Single && ParentControl is TreeViewControl tree)
                {
                    foreach (var node in tree.GetAllNodesRecursive())
                        node.IsSelected = false;

                    IsSelected = true;
                }
            });
            UpdateIsLeaf();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class with specified properties.
        /// </summary>
        /// <param name="selectionMode">The selection mode for the node.</param>
        /// <param name="key">The unique key of the node.</param>
        /// <param name="title">The display title of the node.</param>
        /// <param name="parentKey">The key of the parent node (optional).</param>
        /// <param name="isActive">A flag indicating if the node is active (optional, default is true).</param>
        /// <param name="icon">The icon string for the node (optional).</param>
        public TreeNode(TreeSelectionMode selectionMode, object key, string title, object? parentKey = null, bool isActive = true, string? icon = null) : this()
        {
            Key = key;
            Title = title;
            ParentKey = parentKey;
            IsActive = isActive;
            Icon = icon;
            IsExpanded = true;
            SelectionMode = selectionMode;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles changes to the Children collection to update the IsLeaf property.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateIsLeaf();
        }

        /// <summary>
        /// Updates the <see cref="IsLeaf"/> property based on whether the node has children.
        /// </summary>
        private void UpdateIsLeaf()
        {
            IsLeaf = _children.Count == 0;
        }

        /// <summary>
        /// Notifies the parent TreeViewControl about selection changes, unless synchronization is suppressed.
        /// </summary>
        internal void NotifySelectionChanged()
        {
            if (ParentControl?._suppressSelectionSync == true)
                return;

            ParentControl?.UpdateSelectedNodes(this, IsSelected);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed (automatically populated by CallerMemberName).</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Recalculates the <see cref="ShouldHighlight"/> property based on the current selection state and selection mode.
        /// </summary>
        public void RecalculateHighlight()
        {
            ShouldHighlight = IsSelected && SelectionMode == TreeSelectionMode.Single;
        }
        #endregion
    }
}
