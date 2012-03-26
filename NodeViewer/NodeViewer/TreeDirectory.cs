namespace NodeViewer
{
    using Hero.Definition;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    public class TreeDirectory : IComparable
    {
        public Dictionary<string, TreeDirectory> Folders;
        public string Name;
        public HeroNodeDef Node;
        protected System.Windows.Forms.TreeNode treeNode;

        public TreeDirectory(string Name)
        {
            this.Folders = new Dictionary<string, TreeDirectory>();
            this.Name = Name;
            this.Node = null;
        }

        public TreeDirectory(string Name, HeroNodeDef Node)
        {
            this.Folders = new Dictionary<string, TreeDirectory>();
            this.Name = Name;
            this.Node = Node;
        }

        public int CompareTo(object obj)
        {
            return this.Name.CompareTo((obj as TreeDirectory).Name);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public System.Windows.Forms.TreeNode[] Children
        {
            get
            {
                int index = 0;
                System.Windows.Forms.TreeNode[] nodeArray = new System.Windows.Forms.TreeNode[this.Folders.Count];
                TreeDirectory[] array = new TreeDirectory[this.Folders.Count];
                this.Folders.Values.CopyTo(array, 0);
                Array.Sort<TreeDirectory>(array);
                foreach (TreeDirectory directory in array)
                {
                    nodeArray[index] = directory.TreeNode;
                    index++;
                }
                return nodeArray;
            }
        }

        public System.Windows.Forms.TreeNode TreeNode
        {
            get
            {
                if (this.treeNode == null)
                {
                    this.treeNode = new System.Windows.Forms.TreeNode(this.Name);
                    this.treeNode.Tag = this;
                    if (this.Folders.Count != 0)
                    {
                        this.treeNode.Nodes.Add("VirtualNode");
                    }
                }
                return this.treeNode;
            }
        }
    }
}

