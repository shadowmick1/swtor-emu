namespace NodeViewer
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class LogViewControl : UserControl
    {
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader9;
        private IContainer components;
        protected NodeViewer.Log.LogAddedHandler handler;
        private Label label2;
        private ListView listViewLog;
        protected NodeViewer.Log log;

        public LogViewControl()
        {
            this.InitializeComponent();
            this.log = null;
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
            this.listViewLog = new ListView();
            this.columnHeader5 = new ColumnHeader();
            this.columnHeader9 = new ColumnHeader();
            this.label2 = new Label();
            base.SuspendLayout();
            this.listViewLog.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.listViewLog.Columns.AddRange(new ColumnHeader[] { this.columnHeader5, this.columnHeader9 });
            this.listViewLog.Location = new Point(4, 0x22);
            this.listViewLog.Margin = new Padding(4, 4, 4, 4);
            this.listViewLog.Name = "listViewLog";
            this.listViewLog.Size = new Size(0x34b, 0x68);
            this.listViewLog.TabIndex = 3;
            this.listViewLog.UseCompatibleStateImageBehavior = false;
            this.listViewLog.View = View.Details;
            this.columnHeader5.Text = "Time";
            this.columnHeader5.Width = 0x70;
            this.columnHeader9.Text = "Message";
            this.columnHeader9.Width = 0x1fc;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(12, 15);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x24, 0x11);
            this.label2.TabIndex = 2;
            this.label2.Text = "Log:";
            base.AutoScaleDimensions = new SizeF(8f, 16f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.listViewLog);
            base.Controls.Add(this.label2);
            base.Margin = new Padding(4, 4, 4, 4);
            base.Name = "LogViewControl";
            base.Size = new Size(0x354, 0x8f);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void log_OnLogAdded(NodeViewer.Log.LogRecord record)
        {
            if (this.listViewLog.InvokeRequired)
            {
                this.listViewLog.Invoke(new NodeViewer.Log.LogAddedHandler(this.log_OnLogAdded), new object[] { record });
            }
            else
            {
                this.listViewLog.Items.Add(record.Time.ToLongTimeString()).SubItems.Add(record.Message);
            }
        }

        private void PopulateLog()
        {
            this.listViewLog.Items.Clear();
            lock (this.log)
            {
                foreach (NodeViewer.Log.LogRecord record in this.log.Records)
                {
                    this.log_OnLogAdded(record);
                }
            }
        }

        public NodeViewer.Log Log
        {
            get
            {
                return this.log;
            }
            set
            {
                if (this.log != value)
                {
                    if (this.log != null)
                    {
                        this.log.OnLogAdded -= this.handler;
                    }
                    this.log = value;
                    if (this.log != null)
                    {
                        this.PopulateLog();
                        this.handler = new NodeViewer.Log.LogAddedHandler(this.log_OnLogAdded);
                        this.log.OnLogAdded += this.handler;
                    }
                }
            }
        }
    }
}

