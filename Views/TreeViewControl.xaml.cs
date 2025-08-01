#region Usings
using MauiFluentTreeView.EventsArgs;
using MauiFluentTreeView.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
#endregion

namespace MauiFluentTreeView.Views;

public partial class TreeViewControl : ContentView
{
    #region Fields
    private Dictionary<object, TreeNode> _nodeMap = new(); // globally reused node lookup
    private bool _visualRefreshScheduled = false;
    public bool _suppressSelectionSync;
    #endregion

    #region Events
    public event EventHandler<UnselectBlockedEventArgs>? UnselectBlocked;
    public event EventHandler<SelectedNodesChangedEventArgs>? SelectedNodesChanged;
    public event EventHandler<TreeNodeEventArgs>? NodeAdded;
    public event EventHandler<NodeMovedEventArgs>? NodeMoved;
    public event EventHandler<TreeNodeEventArgs>? NodeRemoved;
    public event EventHandler<TreeNodeEventArgs>? NodeUpdated;
    public event EventHandler<TreeNodeToggledEventArgs>? NodeToggled;
    public Action<TreeNode>? OnHierarchicalUnselectBlocked;
    #endregion

    #region Properties

    #region SelectedNodes Property
    public static readonly BindableProperty SelectedNodesProperty =
        BindableProperty.Create(
            propertyName: nameof(SelectedNodes),
            returnType: typeof(ObservableCollection<TreeNode>),
            declaringType: typeof(TreeViewControl),
            defaultValue: new ObservableCollection<TreeNode>(),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is TreeViewControl control)
                {
                    if (oldValue is ObservableCollection<TreeNode> oldCol)
                        oldCol.CollectionChanged -= control.OnSelectedNodesCollectionChanged;

                    if (newValue is ObservableCollection<TreeNode> newCol)
                        newCol.CollectionChanged += control.OnSelectedNodesCollectionChanged;

                    OnSelectedNodesChanged(bindable, oldValue, newValue);
                }
            });

    public ObservableCollection<TreeNode> SelectedNodes
    {
        get => (ObservableCollection<TreeNode>)GetValue(SelectedNodesProperty);
        set => SetValue(SelectedNodesProperty, value);
    }
    #endregion

    #region SelectionMode Property
    public static readonly BindableProperty SelectionModeProperty =
        BindableProperty.Create(
            propertyName: nameof(SelectionMode),
            returnType: typeof(TreeSelectionMode),
            declaringType: typeof(TreeViewControl),
            defaultValue: TreeSelectionMode.Multiple,
            defaultBindingMode: BindingMode.OneWay);

    public TreeSelectionMode SelectionMode
    {
        get => (TreeSelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }
    #endregion

    #region DataSource Property
    public static readonly BindableProperty DataSourceProperty =
        BindableProperty.Create(
            propertyName: nameof(DataSource),
            returnType: typeof(ObservableCollection<FlatNode>),
            declaringType: typeof(TreeViewControl),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnDataSourceChanged);

    public ObservableCollection<FlatNode> DataSource
    {
        get => (ObservableCollection<FlatNode>)GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }
    #endregion

    #region HierarchicalDataSource Property
    public static readonly BindableProperty HierarchicalDataSourceProperty =
        BindableProperty.Create(
            propertyName: nameof(HierarchicalDataSource),
            returnType: typeof(ObservableCollection<TreeNode>),
            declaringType: typeof(TreeViewControl),
            defaultValue: new ObservableCollection<TreeNode>(),
            defaultBindingMode: BindingMode.OneWayToSource);

    public ObservableCollection<TreeNode> HierarchicalDataSource
    {
        get => (ObservableCollection<TreeNode>)GetValue(HierarchicalDataSourceProperty);
        private set => SetValue(HierarchicalDataSourceProperty, value);
    }
    #endregion

    #region UseHierarchicalSelection Property
    public static readonly BindableProperty UseHierarchicalSelectionProperty =
        BindableProperty.Create(nameof(UseHierarchicalSelection), typeof(bool), typeof(TreeViewControl), false);

    public bool UseHierarchicalSelection
    {
        get => (bool)GetValue(UseHierarchicalSelectionProperty);
        set => SetValue(UseHierarchicalSelectionProperty, value);
    }
    #endregion

    #endregion

    #region Constructor
    public TreeViewControl()
    {
        InitializeComponent();
        SelectedNodes.CollectionChanged += OnSelectedNodesCollectionChanged;
    }
    #endregion

    #region Public Methods

    // New method to clear the selected nodes
    public void ClearSelectedNodes()
    {
        // Clearing the collection will automatically trigger the OnSelectedNodesCollectionChanged handler.
        SelectedNodes.Clear();
    }

    // New method to add a node at a specific location
    public void AddNode(FlatNode newNode, object? parentKey = null)
    {
        if (newNode == null)
        {
            throw new ArgumentNullException(nameof(newNode));
        }

        if (newNode.Id == null)
        {
            throw new ArgumentException("The new node must have a non-null Id.", nameof(newNode));
        }

        // Find if a node with the same ID already exists
        if (FindNodeByKey(newNode.Id) != null)
        {
            throw new ArgumentException($"A node with the ID '{newNode.Id}' already exists.", nameof(newNode.Id));
        }

        var newTreeNode = new TreeNode(
           selectionMode: this.SelectionMode,
           key: newNode.Id,
           title: newNode.Title,
           parentKey: newNode.ParentId,
           isActive: newNode.IsActive,
           icon: newNode.Icon
        );
        newTreeNode.ParentControl = this;

        if (parentKey == null || EqualityComparer<object>.Default.Equals(parentKey, 0))
        {
            // Add to root
            HierarchicalDataSource.Add(newTreeNode);
            newTreeNode.DepthDirty = true;

            newTreeNode.DepthDirty = true;
            ScheduleVisualRefresh();

        }
        else
        {
            // Find the parent node
            var parentNode = FindNodeByKey(parentKey);
            if (parentNode == null)
            {
                throw new InvalidOperationException($"The parent node with key '{parentKey}' was not found.");
            }

            parentNode.Children.Add(newTreeNode);
            newTreeNode.Parent = parentNode;
            newTreeNode.DepthDirty = true;
            ScheduleVisualRefresh();
        }

        NodeAdded?.Invoke(this, new TreeNodeEventArgs(newTreeNode));
    }

    // Method to remove a node by its key
    public void RemoveNode(object key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var nodeToRemove = FindNodeByKey(key);
        if (nodeToRemove == null)
        {
            throw new InvalidOperationException($"A node with the key '{key}' was not found.");
        }

        var hasChildren = nodeToRemove.Children.Count > 0;
        if (hasChildren)
        {
            throw new InvalidOperationException($"A node with the key '{key}' has children and cannot be removed.");
        }
        // Remove the node from the HierarchicalDataSource or its parent's Children collection
        if (nodeToRemove.Parent == null)
        {
            HierarchicalDataSource.Remove(nodeToRemove);
        }
        else
        {
            nodeToRemove.Parent.Children.Remove(nodeToRemove);
        }

        // Unselect the node if it's currently selected
        if (SelectedNodes.Contains(nodeToRemove))
        {
            SelectedNodes.Remove(nodeToRemove);
        }

        NodeRemoved?.Invoke(this, new TreeNodeEventArgs(nodeToRemove));
    }

    // Method to update a node's properties
    public void UpdateNode(object key, FlatNode updatedData)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        if (updatedData == null)
        {
            throw new ArgumentNullException(nameof(updatedData));
        }

        var nodeToUpdate = FindNodeByKey(key);
        if (nodeToUpdate == null)
        {
            throw new InvalidOperationException($"A node with the key '{key}' was not found.");
        }

        nodeToUpdate.Title = updatedData.Title;
        nodeToUpdate.Icon = updatedData.Icon;
        nodeToUpdate.IsActive = updatedData.IsActive;

        NodeUpdated?.Invoke(this, new TreeNodeEventArgs(nodeToUpdate));
    }

    // Method to move a node to a new parent
    public void MoveNode(object nodeKey, object? newParentKey)
    {
        if (nodeKey == null)
        {
            throw new ArgumentNullException(nameof(nodeKey));
        }

        var nodeToMove = FindNodeByKey(nodeKey);
        if (nodeToMove == null)
        {
            throw new InvalidOperationException($"The node to move with key '{nodeKey}' was not found.");
        }

        TreeNode? newParentNode = null;
        if (newParentKey != null)
        {
            newParentNode = FindNodeByKey(newParentKey);
            if (newParentNode == null)
            {
                throw new InvalidOperationException($"The new parent node with key '{newParentKey}' was not found.");
            }
        }

        if (newParentNode != null && IsDescendant(nodeToMove, newParentNode))
        {
            throw new InvalidOperationException("Cannot move a node to one of its own descendants.");
        }

        var oldParent = nodeToMove.Parent;
        // Remove from old location
        if (oldParent == null)
        {
            HierarchicalDataSource.Remove(nodeToMove);
        }
        else
        {
            oldParent.Children.Remove(nodeToMove);
        }

        // Add to new location
        if (newParentNode == null)
        {
            HierarchicalDataSource.Add(nodeToMove);
        }
        else
        {
            newParentNode.Children.Add(nodeToMove);
        }

        // Update properties
        nodeToMove.Parent = newParentNode;
        nodeToMove.ParentKey = newParentKey;
        nodeToMove.DepthDirty = true;
        ScheduleVisualRefresh();

        NodeMoved?.Invoke(this, new NodeMovedEventArgs(nodeToMove, oldParent, newParentNode));
    }

    // Method to select all nodes
    public void SelectAll()
    {
        SelectedNodes.Clear();
        foreach (var node in GetAllNodesRecursive())
        {
            SelectedNodes.Add(node);
        }
    }

    // Method to find a node by its key
    public TreeNode? FindNodeByKey(object key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        return _nodeMap.TryGetValue(key, out var node) ? node : null;
    }

    // Method to find a node's children by its key
    public ObservableCollection<TreeNode> FindChildrenByKey(object key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var parentNode = FindNodeByKey(key);
        if (parentNode == null)
        {
            return new ObservableCollection<TreeNode>();
        }

        return parentNode.Children;
    }

    // Methods to expand/collapse the tree
    public void ExpandAll() => SetExpansionState(HierarchicalDataSource, true);
    public void CollapseAll() => SetExpansionState(HierarchicalDataSource, false);

    #endregion

    #region Private Methods

    private void OnSelectedNodesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _suppressSelectionSync = true;
        foreach (var node in GetAllNodesRecursive())
            node.IsSelected = SelectedNodes.Contains(node);
        _suppressSelectionSync = false;

        SelectedNodesChanged?.Invoke(this, new SelectedNodesChangedEventArgs(SelectedNodes));
    }

    private static void OnSelectedNodesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TreeViewControl control &&
            newValue is ObservableCollection<TreeNode> selected)
        {
            control._suppressSelectionSync = true;
            foreach (var node in control.GetAllNodesRecursive())
                node.IsSelected = false;
            foreach (var selectedNode in selected)
                selectedNode.IsSelected = true;
            control._suppressSelectionSync = false;
        }
    }

    internal void RefreshTreeVisuals()
    {
        foreach (var node in GetAllNodesRecursive())
        {
            if (node.DepthDirty)
            {
                SyncDepths(node, node.Parent?.Depth + 1 ?? 0);
                node.DepthDirty = false;
            }

            if (node.ExpansionDirty)
            {
                node.IsExpanded = node.IsExpanded; // optional visual refresh
                node.ExpansionDirty = false;
            }
        }
    }

    // Helper to check if a node is a descendant of another
    private bool IsDescendant(TreeNode descendant, TreeNode potentialAncestor)
    {
        var current = descendant;
        while (current != null)
        {
            if (current == potentialAncestor)
            {
                return true;
            }
            current = current.Parent;
        }
        return false;
    }

    private static void OnDataSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TreeViewControl control)
        {
            if (oldValue is ObservableCollection<FlatNode> oldCollection)
            {
                oldCollection.CollectionChanged -= control.OnFlatDataSourceCollectionChanged;
            }

            if (newValue is ObservableCollection<FlatNode> newCollection)
            {
                newCollection.CollectionChanged += control.OnFlatDataSourceCollectionChanged;
            }
            control.BuildTree();
        }
    }

    private void OnFlatDataSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        BuildTree();
    }

    public void BuildTree()
    {
        if (HierarchicalDataSource == null)
            HierarchicalDataSource = new ObservableCollection<TreeNode>();
        HierarchicalDataSource.Clear();
        _nodeMap.Clear();

        if (DataSource == null || !DataSource.Any())
        {
            Debug.WriteLine("No data in DataSource to build tree.");
            return;
        }

        foreach (var flatNode in DataSource)
        {
            if (flatNode.Id == null)
            {
                Debug.WriteLine($"Warning: FlatNode with null Id encountered. Skipping: {flatNode.Title}");
                continue;
            }
            if (_nodeMap.ContainsKey(flatNode.Id))
            {
                Debug.WriteLine($"Warning: Duplicate FlatNode Id encountered: '{flatNode.Id}'. Skipping duplicate.");
                continue;
            }

            var treeNode = new TreeNode(
                selectionMode: this.SelectionMode,
                key: flatNode.Id,
                title: flatNode.Title,
                parentKey: flatNode.ParentId,
                isActive: flatNode.IsActive,
                icon: flatNode.Icon
            );
            treeNode.ParentControl = this;
            treeNode.Children.Clear();
            treeNode.IsExpanded = false;

            treeNode.DepthDirty = true;
            treeNode.ExpansionDirty = true;

            _nodeMap[treeNode.Key] = treeNode;
        }

        object rootParentKeyValue = 0;
        foreach (var treeNode in _nodeMap.Values)
        {
            bool isRoot = treeNode.ParentKey == null ||
            EqualityComparer<object>.Default.Equals(treeNode.ParentKey, rootParentKeyValue);

            if (isRoot)
            {
                HierarchicalDataSource.Add(treeNode);
                treeNode.ParentControl = this;
            }
            else if (_nodeMap.TryGetValue(treeNode.ParentKey!, out var parentNode))
            {
                parentNode.Children.Add(treeNode);
                treeNode.Parent = parentNode;
                treeNode.ParentControl = this;
            }
            else
            {
                Debug.WriteLine($"Warning: Node '{treeNode.Title}' (Key: '{treeNode.Key}') has an unknown ParentKey: '{treeNode.ParentKey}'. Adding as root.");
                HierarchicalDataSource.Add(treeNode);
            }
        }

        foreach (var rootNode in HierarchicalDataSource)
        {
            PropagateSelectionMode(rootNode, this.SelectionMode);
            SyncDepths(rootNode, 0); // Uses early-exit optimized method
        }

        Device.BeginInvokeOnMainThread(() => RefreshTreeVisuals());
        Debug.WriteLine($"Tree built with {HierarchicalDataSource.Count} root nodes.");
    }

    void PropagateSelectionMode(TreeNode node, TreeSelectionMode mode)
    {
        node.SelectionMode = mode;
        foreach (var child in node.Children)
            PropagateSelectionMode(child, mode);
    }

    internal void UpdateSelectedNodes(TreeNode node, bool isSelected)
    {
        Debug.WriteLine($"UpdateSelectedNodes called for: {node.Title}, isSelected: {isSelected}");
        if (SelectedNodes == null)
        {
            Debug.WriteLine("SelectedNodes is null, skipping.");
            return;
        }

        switch (SelectionMode)
        {
            case TreeSelectionMode.Single:
                SelectedNodes.Clear();
                if (isSelected)
                {
                    Debug.WriteLine($"[Single] Adding: {node.Title}");
                    SelectedNodes.Add(node);
                }
                break;
            case TreeSelectionMode.Multiple:
                if (UseHierarchicalSelection)
                    UpdateHierarchicalSelection(node, isSelected);
                else
                    UpdateRandomSelection(node, isSelected);
                break;

            default:
                break;
        }
    }
    private void UpdateRandomSelection(TreeNode node, bool isSelected)
    {
        if (isSelected)
        {
            if (!SelectedNodes.Contains(node))
            {
                Debug.WriteLine($"[Multiple Random] Adding: {node.Title}");
                SelectedNodes.Add(node);
            }
        }
        else
        {
            if (SelectedNodes.Contains(node))
            {
                Debug.WriteLine($"[Multiple Random] Removing: {node.Title}");
                SelectedNodes.Remove(node);
            }
        }
    }
    private void UpdateHierarchicalSelection(TreeNode node, bool isSelected)
    {
        if (isSelected)
        {
            var current = node;
            while (current != null)
            {
                if (!SelectedNodes.Contains(current))
                {
                    Debug.WriteLine($"[Hierarchical] Adding: {current.Title}");
                    SelectedNodes.Add(current);
                }

                if (!current.IsSelected)
                    current.IsSelected = true;
                current = current.Parent;
            }
        }
        else
        {
            bool hasSelectedChildren = node.Children.Any(c => c.IsSelected);
            if (hasSelectedChildren)
            {
                Debug.WriteLine($"[Hierarchical] Blocked unselect for: {node.Title} — has selected children");
                UnselectBlocked?.Invoke(this, new UnselectBlockedEventArgs(
                       title: node.Title,
                       key: node.Key,
                       selectedChildCount: node.Children.Count
                   ));
                OnHierarchicalUnselectBlocked?.Invoke(node);
                return;
            }

            if (SelectedNodes.Contains(node))
            {
                Debug.WriteLine($"[Hierarchical] Removing: {node.Title}");
                SelectedNodes.Remove(node);
            }

            if (node.IsSelected)
                node.IsSelected = false;
        }
    }

    public IEnumerable<TreeNode> GetAllNodesRecursive()
    {
        foreach (var root in HierarchicalDataSource)
            foreach (var node in Traverse(root))
                yield return node;
        IEnumerable<TreeNode> Traverse(TreeNode node)
        {
            yield return node;
            foreach (var child in node.Children)
                foreach (var descendant in Traverse(child))
                    yield return descendant;
        }
    }

    private void SetExpansionState(ObservableCollection<TreeNode> nodes, bool isExpanded)
    {
        foreach (var node in nodes)
        {
            if (node.IsExpanded != isExpanded)
            {
                node.IsExpanded = isExpanded;
                NodeToggled?.Invoke(this, new TreeNodeToggledEventArgs(node, isExpanded));
            }

            SetExpansionState(node.Children, isExpanded);
        }
    }

    private void SyncDepths(TreeNode node, int expectedDepth)
    {
        if (node.Depth == expectedDepth &&
            node.Children.All(c => c.Depth == expectedDepth + 1))
            return;
        node.Depth = expectedDepth;

        foreach (var child in node.Children)
            SyncDepths(child, expectedDepth + 1);
    }

    private void ScheduleVisualRefresh()
    {
        if (_visualRefreshScheduled) return;
        _visualRefreshScheduled = true;

        ScheduleVisualRefresh();
    }
    #endregion
}