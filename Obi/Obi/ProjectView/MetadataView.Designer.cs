namespace Obi.ProjectView
{
    partial class MetadataView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetadataView));
            this.mMetadataListView = new System.Windows.Forms.ListView();
            this.mNameColumn = new System.Windows.Forms.ColumnHeader();
            this.mContentColumn = new System.Windows.Forms.ColumnHeader();
            this.mContentTextbox = new System.Windows.Forms.TextBox();
            this.mUpdateButton = new System.Windows.Forms.Button();
            this.mNameTextbox = new System.Windows.Forms.TextBox();
            this.mNameLabel = new System.Windows.Forms.Label();
            this.mContentLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.m_BtnContextMenu = new System.Windows.Forms.Button();
            this.mMetadataContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SetDefaultMetadataStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetDefaultMetadataOverwriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsDefaultMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMetadataContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMetadataListView
            // 
            resources.ApplyResources(this.mMetadataListView, "mMetadataListView");
            this.mMetadataListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mMetadataListView.CheckBoxes = true;
            this.mMetadataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mNameColumn,
            this.mContentColumn});
            this.mMetadataListView.FullRowSelect = true;
            this.mMetadataListView.MultiSelect = false;
            this.mMetadataListView.Name = "mMetadataListView";
            this.mMetadataListView.ShowItemToolTips = true;
            this.mMetadataListView.UseCompatibleStateImageBehavior = false;
            this.mMetadataListView.View = System.Windows.Forms.View.Details;
            this.mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.mMetadataListView_ItemChecked);
            this.mMetadataListView.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.mMetadataListView_ItemMouseHover);
            this.mMetadataListView.SelectedIndexChanged += new System.EventHandler(this.mMetadataListView_SelectedIndexChanged);
            this.mMetadataListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.mMetadataListView_ItemCheck);
            // 
            // mNameColumn
            // 
            resources.ApplyResources(this.mNameColumn, "mNameColumn");
            // 
            // mContentColumn
            // 
            resources.ApplyResources(this.mContentColumn, "mContentColumn");
            // 
            // mContentTextbox
            // 
            resources.ApplyResources(this.mContentTextbox, "mContentTextbox");
            this.mContentTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mContentTextbox.Name = "mContentTextbox";
            this.mContentTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mContentTextbox_KeyDown);
            this.mContentTextbox.Leave += new System.EventHandler(this.mContentTextbox_Leave);
            // 
            // mUpdateButton
            // 
            resources.ApplyResources(this.mUpdateButton, "mUpdateButton");
            this.mUpdateButton.Name = "mUpdateButton";
            this.mUpdateButton.UseVisualStyleBackColor = true;
            this.mUpdateButton.Click += new System.EventHandler(this.mCommitButton_Click);
            // 
            // mNameTextbox
            // 
            resources.ApplyResources(this.mNameTextbox, "mNameTextbox");
            this.mNameTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNameTextbox.Name = "mNameTextbox";
            this.mNameTextbox.Leave += new System.EventHandler(this.mNameTextbox_Leave);
            // 
            // mNameLabel
            // 
            resources.ApplyResources(this.mNameLabel, "mNameLabel");
            this.mNameLabel.Name = "mNameLabel";
            // 
            // mContentLabel
            // 
            resources.ApplyResources(this.mContentLabel, "mContentLabel");
            this.mContentLabel.Name = "mContentLabel";
            // 
            // m_BtnContextMenu
            // 
            resources.ApplyResources(this.m_BtnContextMenu, "m_BtnContextMenu");
            this.m_BtnContextMenu.Name = "m_BtnContextMenu";
            this.m_BtnContextMenu.UseVisualStyleBackColor = true;
            this.m_BtnContextMenu.Click += new System.EventHandler(this.m_BtnContextMenu_Click);
            // 
            // mMetadataContextMenuStrip
            // 
            this.mMetadataContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetDefaultMetadataStripMenuItem,
            this.SetDefaultMetadataOverwriteToolStripMenuItem,
            this.SaveAsDefaultMetadataToolStripMenuItem});
            this.mMetadataContextMenuStrip.Name = "mMetadataContextMenuStrip";
            resources.ApplyResources(this.mMetadataContextMenuStrip, "mMetadataContextMenuStrip");
            // 
            // SetDefaultMetadataStripMenuItem
            // 
            this.SetDefaultMetadataStripMenuItem.Name = "SetDefaultMetadataStripMenuItem";
            resources.ApplyResources(this.SetDefaultMetadataStripMenuItem, "SetDefaultMetadataStripMenuItem");
            this.SetDefaultMetadataStripMenuItem.Click += new System.EventHandler(this.SetDefaultMetadataStripMenuItem_Click);
            // 
            // SetDefaultMetadataOverwriteToolStripMenuItem
            // 
            this.SetDefaultMetadataOverwriteToolStripMenuItem.Name = "SetDefaultMetadataOverwriteToolStripMenuItem";
            resources.ApplyResources(this.SetDefaultMetadataOverwriteToolStripMenuItem, "SetDefaultMetadataOverwriteToolStripMenuItem");
            this.SetDefaultMetadataOverwriteToolStripMenuItem.Click += new System.EventHandler(this.SetDefaultMetadataOverwriteToolStripMenuItem_Click);
            // 
            // SaveAsDefaultMetadataToolStripMenuItem
            // 
            this.SaveAsDefaultMetadataToolStripMenuItem.Name = "SaveAsDefaultMetadataToolStripMenuItem";
            resources.ApplyResources(this.SaveAsDefaultMetadataToolStripMenuItem, "SaveAsDefaultMetadataToolStripMenuItem");
            this.SaveAsDefaultMetadataToolStripMenuItem.Click += new System.EventHandler(this.SaveAsDefaultMetadataToolStripMenuItem_Click);
            // 
            // MetadataView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.m_BtnContextMenu);
            this.Controls.Add(this.mContentLabel);
            this.Controls.Add(this.mNameLabel);
            this.Controls.Add(this.mMetadataListView);
            this.Controls.Add(this.mNameTextbox);
            this.Controls.Add(this.mUpdateButton);
            this.Controls.Add(this.mContentTextbox);
            resources.ApplyResources(this, "$this");
            this.Name = "MetadataView";
            this.VisibleChanged += new System.EventHandler(this.MetadataView_VisibleChanged);
            this.mMetadataContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView mMetadataListView;
        private System.Windows.Forms.TextBox mContentTextbox;
        private System.Windows.Forms.Button mUpdateButton;
        private System.Windows.Forms.TextBox mNameTextbox;
        private System.Windows.Forms.ColumnHeader mNameColumn;
        private System.Windows.Forms.ColumnHeader mContentColumn;
        private System.Windows.Forms.Label mNameLabel;
        private System.Windows.Forms.Label mContentLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button m_BtnContextMenu;
        private System.Windows.Forms.ContextMenuStrip mMetadataContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem SetDefaultMetadataStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsDefaultMetadataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetDefaultMetadataOverwriteToolStripMenuItem;

    }
}
