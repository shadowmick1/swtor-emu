namespace NodeViewer
{
    using Hero;
    using Hero.Definition;
    using Hero.Types;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormMain : Form
    {
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderType;
        private ColumnHeader columnHeaderValue;
        private IContainer components;
        protected TreeDirectory directory;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label labelFieldDesc;
        private Label labelNodeDesc;
        private Label labelNodeType;
        private Label labelType;
        private Label labelTypeDesc;
        private ListView listViewVariables;
        private LogViewControl logViewControl1;
        private SplitContainer splitContainer1;
        private TreeView treeViewNodes;
        protected Worker worker;

        public FormMain()
        {
            this.InitializeComponent();
            this.directory = new TreeDirectory(null);
            this.logViewControl1.Log = Log.Instance;
            this.worker = new Worker();
            this.worker.OnInitComplete += new Worker.InitCompleteHandler(this.worker_OnInitComplete);
            ImageList list = new ImageList();
            list.Images.Add(new Bitmap(8, 8));
            this.listViewVariables.SmallImageList = list;
        }

        private void AddList(HeroList list, int indent)
        {
            int num = 1;
            foreach (HeroVarId id in list.Data)
            {
                string str2;
                string text = string.Format("{0}", num);
                ListViewItem item = this.listViewVariables.Items.Add(text);
                item.IndentCount = indent;
                num++;
                if (list.Type.Values != null)
                {
                    str2 = list.Type.Values.ToString();
                }
                else
                {
                    str2 = "";
                }
                item.SubItems.Add(str2);
                this.SetValueText(id.Value, indent, item);
            }
        }

        private void AddLookupList(HeroLookupList list, int indent)
        {
            int num = 1;
            foreach (KeyValuePair<HeroVarId, HeroAnyValue> pair in list.Data)
            {
                string str2;
                string text = "";
                switch (pair.Key.Value.Type.Type)
                {
                    case HeroTypes.Id:
                    {
                        text = (pair.Key.Value as HeroID).ValueText;
                        HeroID oid = pair.Key.Value as HeroID;
                        if (GOM.Instance.Definitions.ContainsKey(oid.Id))
                        {
                            object obj2 = text;
                            text = string.Concat(new object[] { obj2, " (", GOM.Instance.Definitions[oid.Id], ")" });
                        }
                        break;
                    }
                    case HeroTypes.Integer:
                    {
                        text = (pair.Key.Value as HeroInt).ValueText;
                        HeroInt num2 = pair.Key.Value as HeroInt;
                        if (GOM.Instance.Definitions.ContainsKey((ulong) num2.Value))
                        {
                            object obj3 = text;
                            text = string.Concat(new object[] { obj3, " (", GOM.Instance.Definitions[(ulong) num2.Value], ")" });
                        }
                        break;
                    }
                    case HeroTypes.Enum:
                        text = (pair.Key.Value as HeroEnum).ValueText;
                        break;

                    case HeroTypes.String:
                        text = (pair.Key.Value as HeroString).ValueText;
                        break;
                }
                ListViewItem item = this.listViewVariables.Items.Add(text);
                item.IndentCount = indent;
                num++;
                if (list.Type.Values != null)
                {
                    str2 = list.Type.Values.ToString();
                }
                else
                {
                    str2 = "";
                }
                item.SubItems.Add(str2);
                this.SetValueText(pair.Value, indent, item);
            }
        }

        private void AddVariables(VariableList variables, int indent)
        {
            foreach (Variable variable in variables)
            {
                string name;
                if (variable.Field.Definition != null)
                {
                    name = variable.Field.Definition.Name;
                }
                else
                {
                    name = string.Format("0x{0:X}", variable.Field.Id);
                }
                ListViewItem item = this.listViewVariables.Items.Add(name);
                item.IndentCount = indent;
                item.SubItems.Add(variable.Value.Type.ToString());
                this.SetValueText(variable.Value, indent, item);
                item.Tag = variable;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new SplitContainer();
            this.treeViewNodes = new TreeView();
            this.labelTypeDesc = new Label();
            this.labelType = new Label();
            this.labelFieldDesc = new Label();
            this.labelNodeDesc = new Label();
            this.labelNodeType = new Label();
            this.label5 = new Label();
            this.label4 = new Label();
            this.label3 = new Label();
            this.label2 = new Label();
            this.label1 = new Label();
            this.listViewVariables = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderType = new ColumnHeader();
            this.columnHeaderValue = new ColumnHeader();
            this.logViewControl1 = new LogViewControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            base.SuspendLayout();
            this.splitContainer1.Dock = DockStyle.Top;
            this.splitContainer1.Location = new Point(0, 0);
            this.splitContainer1.Margin = new Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.treeViewNodes);
            this.splitContainer1.Panel2.Controls.Add(this.labelTypeDesc);
            this.splitContainer1.Panel2.Controls.Add(this.labelType);
            this.splitContainer1.Panel2.Controls.Add(this.labelFieldDesc);
            this.splitContainer1.Panel2.Controls.Add(this.labelNodeDesc);
            this.splitContainer1.Panel2.Controls.Add(this.labelNodeType);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.listViewVariables);
            this.splitContainer1.Size = new Size(0x5bf, 0x2e0);
            this.splitContainer1.SplitterDistance = 0x1a2;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            this.treeViewNodes.Dock = DockStyle.Fill;
            this.treeViewNodes.Location = new Point(0, 0);
            this.treeViewNodes.Margin = new Padding(3, 2, 3, 2);
            this.treeViewNodes.Name = "treeViewNodes";
            this.treeViewNodes.Size = new Size(0x1a2, 0x2e0);
            this.treeViewNodes.TabIndex = 0;
            this.treeViewNodes.BeforeExpand += new TreeViewCancelEventHandler(this.treeViewNodes_BeforeExpand);
            this.treeViewNodes.AfterSelect += new TreeViewEventHandler(this.treeViewNodes_AfterSelect);
            this.labelTypeDesc.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelTypeDesc.Location = new Point(0x9d, 0x2c5);
            this.labelTypeDesc.Margin = new Padding(4, 0, 4, 0);
            this.labelTypeDesc.Name = "labelTypeDesc";
            this.labelTypeDesc.Size = new Size(890, 0x10);
            this.labelTypeDesc.TabIndex = 10;
            this.labelType.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelType.Location = new Point(0x9d, 0x2b6);
            this.labelType.Margin = new Padding(4, 0, 4, 0);
            this.labelType.Name = "labelType";
            this.labelType.Size = new Size(890, 0x10);
            this.labelType.TabIndex = 9;
            this.labelFieldDesc.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelFieldDesc.Location = new Point(0x9d, 0x2a6);
            this.labelFieldDesc.Margin = new Padding(4, 0, 4, 0);
            this.labelFieldDesc.Name = "labelFieldDesc";
            this.labelFieldDesc.Size = new Size(890, 0x10);
            this.labelFieldDesc.TabIndex = 8;
            this.labelNodeDesc.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelNodeDesc.Location = new Point(0x9d, 0x296);
            this.labelNodeDesc.Margin = new Padding(4, 0, 4, 0);
            this.labelNodeDesc.Name = "labelNodeDesc";
            this.labelNodeDesc.Size = new Size(890, 0x10);
            this.labelNodeDesc.TabIndex = 7;
            this.labelNodeType.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelNodeType.Location = new Point(0x9d, 0x286);
            this.labelNodeType.Margin = new Padding(4, 0, 4, 0);
            this.labelNodeType.Name = "labelNodeType";
            this.labelNodeType.Size = new Size(890, 0x10);
            this.labelNodeType.TabIndex = 6;
            this.label5.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label5.Location = new Point(4, 0x2c5);
            this.label5.Margin = new Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x88, 0x11);
            this.label5.TabIndex = 5;
            this.label5.Text = "Type Description:";
            this.label4.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label4.Location = new Point(4, 0x2b6);
            this.label4.Margin = new Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x31, 0x11);
            this.label4.TabIndex = 4;
            this.label4.Text = "Type:";
            this.label3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.label3.AutoSize = true;
            this.label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label3.Location = new Point(4, 0x2a6);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x87, 0x11);
            this.label3.TabIndex = 3;
            this.label3.Text = "Field Description:";
            this.label2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label2.Location = new Point(4, 0x296);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x8a, 0x11);
            this.label2.TabIndex = 2;
            this.label2.Text = "Node Description:";
            this.label1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(4, 0x286);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x5c, 0x11);
            this.label1.TabIndex = 1;
            this.label1.Text = "Node Type:";
            this.listViewVariables.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.listViewVariables.Columns.AddRange(new ColumnHeader[] { this.columnHeaderName, this.columnHeaderType, this.columnHeaderValue });
            this.listViewVariables.Location = new Point(1, 0);
            this.listViewVariables.Margin = new Padding(3, 2, 3, 2);
            this.listViewVariables.Name = "listViewVariables";
            this.listViewVariables.Size = new Size(0x418, 0x283);
            this.listViewVariables.TabIndex = 0;
            this.listViewVariables.UseCompatibleStateImageBehavior = false;
            this.listViewVariables.View = View.Details;
            this.listViewVariables.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.listViewVariables_ItemSelectionChanged);
            this.listViewVariables.SelectedIndexChanged += new EventHandler(this.listViewVariables_SelectedIndexChanged);
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 200;
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 200;
            this.columnHeaderValue.Text = "Value";
            this.columnHeaderValue.Width = 500;
            this.logViewControl1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.logViewControl1.AutoScroll = true;
            this.logViewControl1.Location = new Point(0, 0x2e6);
            this.logViewControl1.Log = null;
            this.logViewControl1.Margin = new Padding(4);
            this.logViewControl1.Name = "logViewControl1";
            this.logViewControl1.Size = new Size(0x5c0, 0x8f);
            this.logViewControl1.TabIndex = 1;
            base.AutoScaleDimensions = new SizeF(8f, 16f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x5bf, 0x374);
            base.Controls.Add(this.logViewControl1);
            base.Controls.Add(this.splitContainer1);
            base.Margin = new Padding(4);
            base.Name = "FormMain";
            this.Text = "FormMain";
            base.WindowState = FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void listViewVariables_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.labelFieldDesc.Text = "";
            this.labelType.Text = "";
            this.labelTypeDesc.Text = "";
            if (e.IsSelected && (e.Item != null))
            {
                HeroAnyValue tag = e.Item.Tag as HeroAnyValue;
                Variable variable = e.Item.Tag as Variable;
                if (variable != null)
                {
                    if (variable.Field.Definition != null)
                    {
                        this.labelFieldDesc.Text = variable.Field.Definition.Description;
                    }
                    tag = variable.Value;
                }
                if ((tag != null) && (tag.Type.Id != null))
                {
                    if (tag.Type.Id.Definition != null)
                    {
                        this.labelType.Text = tag.Type.Id.Definition.Type.ToString();
                        this.labelTypeDesc.Text = tag.Type.Id.Definition.Description;
                    }
                    else
                    {
                        this.labelType.Text = string.Format("0x{0}", tag.Type.Id.Id);
                    }
                }
            }
        }

        private void listViewVariables_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void SetValueText(HeroAnyValue value, int indent, ListViewItem item)
        {
            string text = "";
            item.Tag = value;
            switch (value.Type.Type)
            {
                case HeroTypes.Id:
                {
                    text = (value as HeroID).ValueText;
                    HeroID oid = value as HeroID;
                    if (GOM.Instance.Definitions.ContainsKey(oid.Id))
                    {
                        object obj2 = text;
                        text = string.Concat(new object[] { obj2, " (", GOM.Instance.Definitions[oid.Id], ")" });
                    }
                    break;
                }
                case HeroTypes.Integer:
                {
                    text = (value as HeroInt).ValueText;
                    HeroInt num = value as HeroInt;
                    if (GOM.Instance.Definitions.ContainsKey((ulong) num.Value))
                    {
                        object obj3 = text;
                        text = string.Concat(new object[] { obj3, " (", GOM.Instance.Definitions[(ulong) num.Value], ")" });
                    }
                    break;
                }
                case HeroTypes.List:
                {
                    HeroList list = value as HeroList;
                    this.AddList(list, indent + 1);
                    break;
                }
                case HeroTypes.LookupList:
                {
                    HeroLookupList list2 = value as HeroLookupList;
                    this.AddLookupList(list2, indent + 1);
                    break;
                }
                case HeroTypes.Class:
                {
                    HeroClass class2 = value as HeroClass;
                    this.AddVariables(class2.Variables, indent + 1);
                    break;
                }
                default:
                    text = value.ValueText;
                    break;
            }
            item.SubItems.Add(text);
        }

        private void treeViewNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.listViewVariables.Items.Clear();
            HeroNodeDef node = (e.Node.Tag as TreeDirectory).Node;
            this.labelNodeType.Text = "";
            this.labelNodeDesc.Text = "";
            this.labelType.Text = "";
            this.labelTypeDesc.Text = "";
            this.labelFieldDesc.Text = "";
            if (node != null)
            {
                if (node.baseClass.Definition != null)
                {
                    this.labelNodeType.Text = node.baseClass.Definition.Name;
                    this.labelNodeDesc.Text = node.baseClass.Definition.Description;
                }
                else
                {
                    this.labelNodeType.Text = string.Format("0x{0}", node.baseClass.Id);
                }
                this.AddVariables(node.Variables, 0);
            }
        }

        private void treeViewNodes_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            e.Node.Nodes.AddRange((e.Node.Tag as TreeDirectory).Children);
        }

        private void worker_OnInitComplete()
        {
            foreach (HeroDefinition definition in GOM.Instance.Definitions.Values)
            {
                HeroNodeDef node = definition as HeroNodeDef;
                if (node != null)
                {
                    string[] strArray = node.Name.Split(new char[] { '.' });
                    TreeDirectory directory = this.directory;
                    for (int i = 0; i < (strArray.Length - 1); i++)
                    {
                        if (!directory.Folders.ContainsKey(strArray[i]))
                        {
                            directory.Folders[strArray[i]] = new TreeDirectory(strArray[i]);
                        }
                        directory = directory.Folders[strArray[i]];
                    }
                    directory.Folders[strArray[strArray.Length - 1]] = new TreeDirectory(strArray[strArray.Length - 1], node);
                }
            }
            if (base.InvokeRequired)
            {
                base.Invoke(new Worker.InitCompleteHandler(this.worker_OnInitComplete));
            }
            else
            {
                this.treeViewNodes.Nodes.AddRange(this.directory.Children);
            }
        }
    }
}

