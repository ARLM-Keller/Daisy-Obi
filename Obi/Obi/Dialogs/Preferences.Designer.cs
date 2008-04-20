namespace Obi.Dialogs
{
    partial class Preferences
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.label2 = new System.Windows.Forms.Label();
            this.mDirectoryBox = new System.Windows.Forms.TextBox();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.labelInputDeviceName = new System.Windows.Forms.Label();
            this.comboInputDevice = new System.Windows.Forms.ComboBox();
            this.labelOutputDeviceName = new System.Windows.Forms.Label();
            this.comboOutputDevice = new System.Windows.Forms.ComboBox();
            this.mTab = new System.Windows.Forms.TabControl();
            this.mProjectTab = new System.Windows.Forms.TabPage();
            this.mTooltipsCheckBox = new System.Windows.Forms.CheckBox();
            this.mLastOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.mExportBox = new System.Windows.Forms.TextBox();
            this.mBrowseExportButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.mAudioTab = new System.Windows.Forms.TabPage();
            this.m_txtBitDepth = new System.Windows.Forms.TextBox();
            this.label_BitDepth = new System.Windows.Forms.Label();
            this.m_txtChannels = new System.Windows.Forms.TextBox();
            this.m_txtSamplingRate = new System.Windows.Forms.TextBox();
            this.comboChannels = new System.Windows.Forms.ComboBox();
            this.labelChannels = new System.Windows.Forms.Label();
            this.comboSampleRate = new System.Windows.Forms.ComboBox();
            this.labelSampleRate = new System.Windows.Forms.Label();
            this.mTab.SuspendLayout();
            this.mProjectTab.SuspendLayout();
            this.mAudioTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default projects &directory:";
            // 
            // mDirectoryBox
            // 
            this.mDirectoryBox.AccessibleName = "Default projects directory:";
            this.mDirectoryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mDirectoryBox.Location = new System.Drawing.Point(152, 38);
            this.mDirectoryBox.Name = "mDirectoryBox";
            this.mDirectoryBox.Size = new System.Drawing.Size(372, 20);
            this.mDirectoryBox.TabIndex = 3;
            // 
            // mBrowseButton
            // 
            this.mBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseButton.Location = new System.Drawing.Point(530, 36);
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.Size = new System.Drawing.Size(75, 25);
            this.mBrowseButton.TabIndex = 4;
            this.mBrowseButton.Text = "&Browse";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.mBrowseButton_Click);
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(243, 466);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 25);
            this.mOKButton.TabIndex = 5;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(324, 466);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 25);
            this.mCancelButton.TabIndex = 6;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // labelInputDeviceName
            // 
            this.labelInputDeviceName.AutoSize = true;
            this.labelInputDeviceName.Location = new System.Drawing.Point(15, 10);
            this.labelInputDeviceName.Name = "labelInputDeviceName";
            this.labelInputDeviceName.Size = new System.Drawing.Size(98, 13);
            this.labelInputDeviceName.TabIndex = 7;
            this.labelInputDeviceName.Text = "&Input device name:";
            // 
            // comboInputDevice
            // 
            this.comboInputDevice.AccessibleName = "Input device name:";
            this.comboInputDevice.AllowDrop = true;
            this.comboInputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInputDevice.FormattingEnabled = true;
            this.comboInputDevice.Location = new System.Drawing.Point(121, 7);
            this.comboInputDevice.Name = "comboInputDevice";
            this.comboInputDevice.Size = new System.Drawing.Size(484, 21);
            this.comboInputDevice.TabIndex = 8;
            // 
            // labelOutputDeviceName
            // 
            this.labelOutputDeviceName.AutoSize = true;
            this.labelOutputDeviceName.Location = new System.Drawing.Point(6, 38);
            this.labelOutputDeviceName.Name = "labelOutputDeviceName";
            this.labelOutputDeviceName.Size = new System.Drawing.Size(106, 13);
            this.labelOutputDeviceName.TabIndex = 9;
            this.labelOutputDeviceName.Text = "O&utput device name:";
            // 
            // comboOutputDevice
            // 
            this.comboOutputDevice.AccessibleName = "Output device name:";
            this.comboOutputDevice.AllowDrop = true;
            this.comboOutputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOutputDevice.FormattingEnabled = true;
            this.comboOutputDevice.Location = new System.Drawing.Point(121, 35);
            this.comboOutputDevice.Name = "comboOutputDevice";
            this.comboOutputDevice.Size = new System.Drawing.Size(484, 21);
            this.comboOutputDevice.TabIndex = 10;
            // 
            // mTab
            // 
            this.mTab.Controls.Add(this.mProjectTab);
            this.mTab.Controls.Add(this.mAudioTab);
            this.mTab.Location = new System.Drawing.Point(12, 13);
            this.mTab.Name = "mTab";
            this.mTab.SelectedIndex = 0;
            this.mTab.Size = new System.Drawing.Size(619, 446);
            this.mTab.TabIndex = 11;
            // 
            // mProjectTab
            // 
            this.mProjectTab.Controls.Add(this.mTooltipsCheckBox);
            this.mProjectTab.Controls.Add(this.mLastOpenCheckBox);
            this.mProjectTab.Controls.Add(this.mExportBox);
            this.mProjectTab.Controls.Add(this.mBrowseExportButton);
            this.mProjectTab.Controls.Add(this.label3);
            this.mProjectTab.Controls.Add(this.label2);
            this.mProjectTab.Controls.Add(this.mBrowseButton);
            this.mProjectTab.Controls.Add(this.mDirectoryBox);
            this.mProjectTab.Location = new System.Drawing.Point(4, 22);
            this.mProjectTab.Name = "mProjectTab";
            this.mProjectTab.Padding = new System.Windows.Forms.Padding(3);
            this.mProjectTab.Size = new System.Drawing.Size(611, 420);
            this.mProjectTab.TabIndex = 0;
            this.mProjectTab.Text = "Project";
            this.mProjectTab.UseVisualStyleBackColor = true;
            // 
            // mTooltipsCheckBox
            // 
            this.mTooltipsCheckBox.AutoSize = true;
            this.mTooltipsCheckBox.Location = new System.Drawing.Point(6, 127);
            this.mTooltipsCheckBox.Name = "mTooltipsCheckBox";
            this.mTooltipsCheckBox.Size = new System.Drawing.Size(95, 17);
            this.mTooltipsCheckBox.TabIndex = 9;
            this.mTooltipsCheckBox.Text = "&Enable tooltips";
            this.mTooltipsCheckBox.UseVisualStyleBackColor = true;
            // 
            // mLastOpenCheckBox
            // 
            this.mLastOpenCheckBox.AutoSize = true;
            this.mLastOpenCheckBox.Location = new System.Drawing.Point(6, 103);
            this.mLastOpenCheckBox.Name = "mLastOpenCheckBox";
            this.mLastOpenCheckBox.Size = new System.Drawing.Size(191, 17);
            this.mLastOpenCheckBox.TabIndex = 8;
            this.mLastOpenCheckBox.Text = "Open &last project when starting Obi";
            this.mLastOpenCheckBox.UseVisualStyleBackColor = true;
            // 
            // mExportBox
            // 
            this.mExportBox.AccessibleName = "Default export directory:";
            this.mExportBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mExportBox.Location = new System.Drawing.Point(152, 69);
            this.mExportBox.Name = "mExportBox";
            this.mExportBox.Size = new System.Drawing.Size(372, 20);
            this.mExportBox.TabIndex = 6;
            // 
            // mBrowseExportButton
            // 
            this.mBrowseExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseExportButton.Location = new System.Drawing.Point(530, 67);
            this.mBrowseExportButton.Name = "mBrowseExportButton";
            this.mBrowseExportButton.Size = new System.Drawing.Size(75, 25);
            this.mBrowseExportButton.TabIndex = 7;
            this.mBrowseExportButton.Text = "&Browse";
            this.mBrowseExportButton.UseVisualStyleBackColor = true;
            this.mBrowseExportButton.Click += new System.EventHandler(this.mBrowseExportButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Default e&xport directory:";
            // 
            // mAudioTab
            // 
            this.mAudioTab.Controls.Add(this.m_txtBitDepth);
            this.mAudioTab.Controls.Add(this.label_BitDepth);
            this.mAudioTab.Controls.Add(this.m_txtChannels);
            this.mAudioTab.Controls.Add(this.m_txtSamplingRate);
            this.mAudioTab.Controls.Add(this.comboChannels);
            this.mAudioTab.Controls.Add(this.labelChannels);
            this.mAudioTab.Controls.Add(this.comboSampleRate);
            this.mAudioTab.Controls.Add(this.labelSampleRate);
            this.mAudioTab.Controls.Add(this.comboOutputDevice);
            this.mAudioTab.Controls.Add(this.comboInputDevice);
            this.mAudioTab.Controls.Add(this.labelOutputDeviceName);
            this.mAudioTab.Controls.Add(this.labelInputDeviceName);
            this.mAudioTab.Location = new System.Drawing.Point(4, 22);
            this.mAudioTab.Name = "mAudioTab";
            this.mAudioTab.Padding = new System.Windows.Forms.Padding(3);
            this.mAudioTab.Size = new System.Drawing.Size(611, 420);
            this.mAudioTab.TabIndex = 1;
            this.mAudioTab.Text = "Audio";
            this.mAudioTab.UseVisualStyleBackColor = true;
            // 
            // m_txtBitDepth
            // 
            this.m_txtBitDepth.AccessibleName = "Bit Depth (fixed):";
            this.m_txtBitDepth.Location = new System.Drawing.Point(121, 124);
            this.m_txtBitDepth.Name = "m_txtBitDepth";
            this.m_txtBitDepth.ReadOnly = true;
            this.m_txtBitDepth.Size = new System.Drawing.Size(100, 20);
            this.m_txtBitDepth.TabIndex = 20;
            // 
            // label_BitDepth
            // 
            this.label_BitDepth.AutoSize = true;
            this.label_BitDepth.Location = new System.Drawing.Point(30, 124);
            this.label_BitDepth.Name = "label_BitDepth";
            this.label_BitDepth.Size = new System.Drawing.Size(85, 13);
            this.label_BitDepth.TabIndex = 19;
            this.label_BitDepth.Text = "&Bit Depth (fixed):";
            // 
            // m_txtChannels
            // 
            this.m_txtChannels.AccessibleName = "Project channels:";
            this.m_txtChannels.Location = new System.Drawing.Point(121, 91);
            this.m_txtChannels.Name = "m_txtChannels";
            this.m_txtChannels.ReadOnly = true;
            this.m_txtChannels.Size = new System.Drawing.Size(100, 20);
            this.m_txtChannels.TabIndex = 18;
            this.m_txtChannels.Visible = false;
            // 
            // m_txtSamplingRate
            // 
            this.m_txtSamplingRate.AccessibleName = "Project sample rate:";
            this.m_txtSamplingRate.Location = new System.Drawing.Point(121, 63);
            this.m_txtSamplingRate.Name = "m_txtSamplingRate";
            this.m_txtSamplingRate.ReadOnly = true;
            this.m_txtSamplingRate.Size = new System.Drawing.Size(100, 20);
            this.m_txtSamplingRate.TabIndex = 17;
            this.m_txtSamplingRate.Visible = false;
            // 
            // comboChannels
            // 
            this.comboChannels.AccessibleName = "Default channels:";
            this.comboChannels.AllowDrop = true;
            this.comboChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboChannels.FormattingEnabled = true;
            this.comboChannels.Location = new System.Drawing.Point(121, 91);
            this.comboChannels.Name = "comboChannels";
            this.comboChannels.Size = new System.Drawing.Size(484, 21);
            this.comboChannels.TabIndex = 16;
            // 
            // labelChannels
            // 
            this.labelChannels.AutoSize = true;
            this.labelChannels.Location = new System.Drawing.Point(30, 94);
            this.labelChannels.Name = "labelChannels";
            this.labelChannels.Size = new System.Drawing.Size(83, 13);
            this.labelChannels.TabIndex = 15;
            this.labelChannels.Text = "Audio &channels:";
            // 
            // comboSampleRate
            // 
            this.comboSampleRate.AccessibleName = "Default sample rate:";
            this.comboSampleRate.AllowDrop = true;
            this.comboSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSampleRate.FormattingEnabled = true;
            this.comboSampleRate.Location = new System.Drawing.Point(121, 63);
            this.comboSampleRate.Name = "comboSampleRate";
            this.comboSampleRate.Size = new System.Drawing.Size(484, 21);
            this.comboSampleRate.TabIndex = 14;
            // 
            // labelSampleRate
            // 
            this.labelSampleRate.AutoSize = true;
            this.labelSampleRate.Location = new System.Drawing.Point(47, 66);
            this.labelSampleRate.Name = "labelSampleRate";
            this.labelSampleRate.Size = new System.Drawing.Size(66, 13);
            this.labelSampleRate.TabIndex = 13;
            this.labelSampleRate.Text = "&Sample rate:";
            // 
            // Preferences
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(643, 504);
            this.Controls.Add(this.mTab);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(8, 173);
            this.Name = "Preferences";
            this.Text = "Edit preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.mTab.ResumeLayout(false);
            this.mProjectTab.ResumeLayout(false);
            this.mProjectTab.PerformLayout();
            this.mAudioTab.ResumeLayout(false);
            this.mAudioTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mDirectoryBox;
        private System.Windows.Forms.Button mBrowseButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label labelInputDeviceName;
        private System.Windows.Forms.ComboBox comboInputDevice;
        private System.Windows.Forms.Label labelOutputDeviceName;
        private System.Windows.Forms.ComboBox comboOutputDevice;
        private System.Windows.Forms.TabControl mTab;
        private System.Windows.Forms.TabPage mProjectTab;
        private System.Windows.Forms.TabPage mAudioTab;
        private System.Windows.Forms.Label labelSampleRate;
        private System.Windows.Forms.ComboBox comboSampleRate;
        private System.Windows.Forms.ComboBox comboChannels;
        private System.Windows.Forms.Label labelChannels;
        private System.Windows.Forms.CheckBox mLastOpenCheckBox;
        private System.Windows.Forms.TextBox mExportBox;
        private System.Windows.Forms.Button mBrowseExportButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox mTooltipsCheckBox;
        private System.Windows.Forms.TextBox m_txtChannels;
        private System.Windows.Forms.TextBox m_txtSamplingRate;
        private System.Windows.Forms.Label label_BitDepth;
        private System.Windows.Forms.TextBox m_txtBitDepth;
    }
}