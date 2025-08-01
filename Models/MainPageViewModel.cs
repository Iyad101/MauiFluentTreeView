using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiFluentTreeView.Models
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FlatNode> _myFlatData;
        private int _nodeIntKeyCounter = 0; // Counter for generating integer-based object keys


        public event PropertyChangedEventHandler? PropertyChanged;
        private string _lbl;
        public string Lbl
        {
            get => _lbl;
            set
            {
                if (_lbl != value)
                {
                    _lbl = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<TreeNode> _mySelectedNodes;

        public ObservableCollection<TreeNode> MySelectedNodes
        {
            get => _mySelectedNodes;
            set
            {
                if (_mySelectedNodes != value)
                {
                    _mySelectedNodes = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<FlatNode> MyFlatData
        {
            get => _myFlatData;
            set
            {
                if (_myFlatData != value)
                {
                    _myFlatData = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddNodeCommand { get; }
        public ICommand SelectRandomNodeCommand { get; }

        public MainPageViewModel()
        {
            _myFlatData = new ObservableCollection<FlatNode>();
            GenerateSampleData();

            AddNodeCommand = new Command(AddNewNode);
            SelectRandomNodeCommand = new Command(SelectRandomNode);
            MySelectedNodes = new ObservableCollection<TreeNode>();

        }

        private void GenerateSampleData()
        {

            List<FlatNode> f = new List<FlatNode>
            {
              new FlatNode{ Title ="Project A", Id = 1 , ParentId = 0, IsActive = true, Icon = "dotnet_bot.png" },

               new FlatNode{ Title ="folder 1", Id = 2 , ParentId = 1, IsActive = true},

                new FlatNode{ Title ="File 1.1", Id = 3 , ParentId = 2, IsActive = true},

                  new FlatNode{ Title ="File 1.2", Id = 4 , ParentId = 2, IsActive = true},

                new FlatNode{ Title ="Project B", Id = 5 , ParentId = 0, IsActive = true},

                 new FlatNode{ Title ="folder 2", Id = 6 , ParentId = 5, IsActive = true},

               new FlatNode{ Title ="SubFolder 2.1", Id = 7 , ParentId = 6, IsActive = true},

                   new FlatNode{ Title ="SubFolder 2.2", Id = 8 , ParentId = 6, IsActive = true},

           new FlatNode{ Title ="File 2.1.1", Id = 9 , ParentId = 8, IsActive = true},

             new FlatNode{ Title ="File 2.1.2", Id = 9 , ParentId = 8, IsActive = true},

           new FlatNode{ Title ="Project C", Id = 10 , ParentId = 0, IsActive = true},


            };

            MyFlatData.Clear();
            foreach (var n in f)
            {
                MyFlatData.Add(n);
            }

        }

        private void AddNewNode()
        {
            if (MySelectedNodes == null)
                return;
            Lbl = MySelectedNodes.Count.ToString();
            foreach (var node in MySelectedNodes)
            {
                Debug.WriteLine(node.Title);
            }
            //_nodeIntKeyCounter++;
            //FlatNode newNode;
            //object newId = _nodeIntKeyCounter; // New nodes will use integer-based object keys for simplicity
            //object parentId = 0; // Default new node as a root

            //if (MyFlatData.Any())
            //{
            //    // Select a random existing node to be the parent
            //    var parentCandidate = MyFlatData[_random.Next(MyFlatData.Count)];
            //    parentId = parentCandidate.Id;
            //}

            //newNode = new FlatNode { Id = newId, Title = $"New Item (ID {_nodeIntKeyCounter})", ParentId = parentId, IsActive = true, Icon = "add" };
            //MyFlatData.Add(newNode);
            // The TreeViewControl's OnFlatDataSourceCollectionChanged will trigger BuildTree.
        }

        private void SelectRandomNode()
        {
            if (MyFlatData.Any())
            {
                // This logic needs to interact with the TreeViewControl's HierarchicalDataSource
                // to select a node and expand its parents. This cannot be done directly on MyFlatData.
                // For now, we'll just log a message.
                Debug.WriteLine("SelectRandomNode command executed. Actual selection logic needs to be implemented in TreeViewControl.");
                // To implement this fully, you might need an event from TreeViewControl
                // or a way to access the hierarchical nodes from here.
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}