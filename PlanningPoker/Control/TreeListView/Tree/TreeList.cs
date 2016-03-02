using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using PlanningPoker.Entity;

namespace Aga.Controls.Tree
{
	public class TreeList: ListView
	{
		#region Properties

		/// <summary>
		/// Internal collection of rows representing visible nodes, actually displayed in the ListView
		/// </summary>
		internal ObservableCollectionAdv<TreeNode> Rows
		{
			get;
			private set;
		} 


		private ITreeModel _model;
		public ITreeModel Model
		{
		  get { return _model; }
		  set 
		  {
			  if (_model != value)
			  {
				  _model = value;
				  _root.Children.Clear();
				  Rows.Clear();
				  CreateChildrenNodes(_root);
			  }

		  }
		}

		private TreeNode _root;
		internal TreeNode Root
		{
			get { return _root; }
		}

		public ReadOnlyCollection<TreeNode> Nodes
		{
			get { return Root.Nodes; }
		}

		internal TreeNode PendingFocusNode
		{
			get;
			set;
		}

		public ICollection<TreeNode> SelectedNodes
		{
			get
			{
				return SelectedItems.Cast<TreeNode>().ToArray();
			}
		}

		public TreeNode SelectedNode
		{
			get
			{
				if (SelectedItems.Count > 0)
					return SelectedItems[0] as TreeNode;
				else
					return null;
			}
		}
		#endregion

		public TreeList()
		{
			Rows = new ObservableCollectionAdv<TreeNode>();
			_root = new TreeNode(this, null);
			_root.IsExpanded = true;
			ItemsSource = Rows;
			ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
		}

		void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
		{
			if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && PendingFocusNode != null)
			{
				var item = ItemContainerGenerator.ContainerFromItem(PendingFocusNode) as TreeListItem;
				if (item != null)
					item.Focus();
				PendingFocusNode = null;
			}
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeListItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeListItem;
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			var ti = element as TreeListItem;
			var node = item as TreeNode;
			if (ti != null && node != null)
			{
				ti.Node = item as TreeNode;
                //base.PrepareContainerForItemOverride(element, node.Tag);
                base.PrepareContainerForItemOverride(element, item);
			}
		}

		internal void SetIsExpanded(TreeNode node, bool value)
		{
			if (value)
			{
				if (!node.IsExpandedOnce)
				{
					node.IsExpandedOnce = true;
					node.AssignIsExpanded(value);
					CreateChildrenNodes(node);
				}
				else
				{
					node.AssignIsExpanded(value);
					CreateChildrenRows(node);
				}
			}
			else
			{
				DropChildrenRows(node, false);
				node.AssignIsExpanded(value);
			}
		}

		internal void CreateChildrenNodes(TreeNode node)
		{
			var children = GetChildren(node);
			if (children != null)
			{
				int rowIndex = Rows.IndexOf(node);

				node.ChildrenSource = children as INotifyCollectionChanged;
				foreach (object obj in children)
				{
					TreeNode child = new TreeNode(this, obj);
					child.HasChildren = HasChildren(child);
					node.Children.Add(child);
				}
				Rows.InsertRange(rowIndex + 1, node.Children.ToArray());
			}
		}

		private void CreateChildrenRows(TreeNode node)
		{
			int index = Rows.IndexOf(node);
			if (index >= 0 || node == _root) // ignore invisible nodes
			{
				var nodes = node.AllVisibleChildren.ToArray();
				Rows.InsertRange(index + 1, nodes);
			}
		}

		internal void DropChildrenRows(TreeNode node, bool removeParent)
		{
			int start = Rows.IndexOf(node);
			if (start >= 0 || node == _root) // ignore invisible nodes
			{
				int count = node.VisibleChildrenCount;
				if (removeParent)
					count++;
				else
					start++;
				Rows.RemoveRange(start, count);
			}
		}

		private IEnumerable GetChildren(TreeNode parent)
		{
            return GetChildren(parent.Tag);
		}

        private IEnumerable GetChildren(object tag)
        {
            if (Model != null)
                return Model.GetChildren(tag);
            else
                return null;
        }

		private bool HasChildren(TreeNode parent)
		{
			if (parent == Root)
				return true;
			else if (Model != null)
				return Model.HasChildren(parent.Tag);
			else
				return false;
		}

		internal void InsertNewNode(TreeNode parent, object tag, int rowIndex, int index)
		{
			TreeNode node = new TreeNode(this, tag);
			if (index >= 0 && index < parent.Children.Count)
				parent.Children.Insert(index, node);
			else
			{
				index = parent.Children.Count;
				parent.Children.Add(node);
			}
			Rows.Insert(rowIndex + index + 1, node);
		}

        private TreeNode FindTreeNode(Stack<TreeNode> queue, TreeNode node, object item)
        {
            if(item.Equals(node.Tag))
            {
                return node;
            }

            foreach (var child in GetChildren(node))
            {
                TreeNode treeNode =  GetTreeNodeInRows(child);

                if(treeNode == null)
                {
                    treeNode = new TreeNode(this, child);
                }

                TreeNode n = FindTreeNode(queue, treeNode, item);

                if(n != null)
                {
                    queue.Push(n);
                    return node;
                }
            }
            return null;
        }

        private TreeNode GetTreeNodeInRows(object item)
        {
            foreach(var r in Rows)
            {
                if(item.Equals(r.Tag))
                {
                    return r;
                }
            }

            return null;
        }

        private TreeNode GetMatchedTreeNode(TreeNode treeNode)
        {
            foreach(var r in Rows)
            {
                if(r.Tag.Equals(treeNode.Tag))
                {
                    return r;
                }
            }
            return null;
        }

        public new void ScrollIntoView(object item)
        {
            var queue = new Stack<TreeNode>();
            FindTreeNode(queue, _root, item);

            TreeNode selectedItem = null;

            while(queue.Count > 0)
            {
                TreeNode treeNode = queue.Pop();
                selectedItem = treeNode;

                if(Rows.Contains(treeNode))
                {
                    treeNode.IsExpanded = true;
                }
                else
                {
                    TreeNode node = GetMatchedTreeNode(treeNode);

                    if(node != null)
                    {
                        node.IsExpanded = true;
                    }
                }
            }

            if (selectedItem != null)
            {
                base.ScrollIntoView(selectedItem);
                base.SelectedItem = selectedItem;
            }
        }
	}
}
