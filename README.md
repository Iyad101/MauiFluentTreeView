# Iyad.UI.TreeViewControl

A flexible and ergonomic TreeView control for .NET MAUI. Supports flat-to-hierarchical data hydration, multiselection, with both hierarchical and boundless selection constraints.

## 📦 Installation

```bash
dotnet add package Iyad.UI.TreeViewControl

🚀 Quick Star

var control = new TreeViewControl
{
    DataSource = new ObservableCollection<FlatNode>
    {
        new FlatNode { Id = 1, Title = "Root", ParentId = 0 },
        new FlatNode { Id = 2, Title = "Child", ParentId = 1 },
        new FlatNode { Id = 3, Title = "Grandchild", ParentId = 2 }
    },
    SelectionMode = TreeSelectionMode.Multiple,
    UseHierarchicalSelection = true
};
control.BuildTree();

✅ Features
- Flat data hydration → recursive hierarchy
- Hierarchical selection logic
- Selection blocking when child nodes are selected
- FindNodeByKey() for runtime access
- Fully bindable with MVVM support

💡 Advanced
- Callback OnHierarchicalUnselectBlocked fires when deselection is blocked by child selections
- Supports expansion state preservation
- Fluent test builder (TreeBuilder) for isolated test setup

📚 API Reference
See XML comments in source for details on each bindable property and method.
