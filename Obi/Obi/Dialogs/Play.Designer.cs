namespace Obi.Dialogs
{
    partial class Play
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
            this.components = new System.ComponentModel.Container();
            this.txtDisplayAsset = new System.Windows.Forms.TextBox();
            this.txtDisplayTime = new System.Windows.Forms.TextBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tmUpdateCurrentTime = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // txtDisplayAsset
            // 
            this.txtDisplayAsset.Location = new System.Drawing.Point(20, 20);
            this.txtDisplayAsset.Name = "txtDisplayAsset";
            this.txtDisplayAsset.ReadOnly = true;
            this.txtDisplayAsset.Size = new System.Drawing.Size(228, 20);
            this.txtDisplayAsset.TabIndex = 0;
            // 
            // txtDisplayTime
            // 
            this.txtDisplayTime.Location = new System.Drawing.Point(20, 110);
            this.txtDisplayTime.Name = "txtDisplayTime";
            this.txtDisplayTime.ReadOnly = true;
            this.txtDisplayTime.Size = new System.Drawing.Size(100, 20);
            this.txtDisplayTime.TabIndex = 4;
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(20, 50);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 2;
            this.btnPlay.Text = "&Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(20, 80);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "&Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tmUpdateCurrentTime
            // 
            this.tmUpdateCurrentTime.Interval = 1000;
            this.tmUpdateCurrentTime.Tick += new System.EventHandler(this.tmUpdateCurrentTime_Tick);
            // 
            // Play
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.txtDisplayTime);
            this.Controls.Add(this.txtDisplayAsset);
            this.Name = "Play";
            this.Text = "Play";
            this.Load += new System.EventHandler(this.Play_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDisplayAsset;
        private System.Windows.Forms.TextBox txtDisplayTime;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Timer tmUpdateCurrentTime;
    }
}