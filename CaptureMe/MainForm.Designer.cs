namespace CaptureMe
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.VideoPreviewPanel = new System.Windows.Forms.Panel();
            this.SnapshotButton = new System.Windows.Forms.Button();
            this.deviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VideoDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AudioDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainWindowMenuStrip = new System.Windows.Forms.MenuStrip();
            this.VideoDeviceCB = new System.Windows.Forms.ComboBox();
            this.AudioDeviceCB = new System.Windows.Forms.ComboBox();
            this.MainWindowMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // VideoPreviewPanel
            // 
            this.VideoPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VideoPreviewPanel.Location = new System.Drawing.Point(12, 27);
            this.VideoPreviewPanel.Name = "VideoPreviewPanel";
            this.VideoPreviewPanel.Size = new System.Drawing.Size(480, 360);
            this.VideoPreviewPanel.TabIndex = 1;
            // 
            // SnapshotButton
            // 
            this.SnapshotButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SnapshotButton.ForeColor = System.Drawing.Color.Red;
            this.SnapshotButton.Location = new System.Drawing.Point(219, 393);
            this.SnapshotButton.Name = "SnapshotButton";
            this.SnapshotButton.Size = new System.Drawing.Size(75, 49);
            this.SnapshotButton.TabIndex = 2;
            this.SnapshotButton.Text = "●";
            this.SnapshotButton.UseVisualStyleBackColor = true;
            this.SnapshotButton.Click += new System.EventHandler(this.SnapshotButton_Click);
            // 
            // deviceToolStripMenuItem
            // 
            this.deviceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VideoDeviceToolStripMenuItem,
            this.AudioDeviceToolStripMenuItem});
            this.deviceToolStripMenuItem.Name = "deviceToolStripMenuItem";
            this.deviceToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.deviceToolStripMenuItem.Text = "Device";
            // 
            // VideoDeviceToolStripMenuItem
            // 
            this.VideoDeviceToolStripMenuItem.Name = "VideoDeviceToolStripMenuItem";
            this.VideoDeviceToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.VideoDeviceToolStripMenuItem.Text = "Video device";
            // 
            // AudioDeviceToolStripMenuItem
            // 
            this.AudioDeviceToolStripMenuItem.Name = "AudioDeviceToolStripMenuItem";
            this.AudioDeviceToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.AudioDeviceToolStripMenuItem.Text = "Audio device";
            // 
            // captureToolStripMenuItem
            // 
            this.captureToolStripMenuItem.Name = "captureToolStripMenuItem";
            this.captureToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.captureToolStripMenuItem.Text = "Capture";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // MainWindowMenuStrip
            // 
            this.MainWindowMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deviceToolStripMenuItem,
            this.captureToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.MainWindowMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MainWindowMenuStrip.Name = "MainWindowMenuStrip";
            this.MainWindowMenuStrip.Size = new System.Drawing.Size(503, 24);
            this.MainWindowMenuStrip.TabIndex = 0;
            this.MainWindowMenuStrip.Text = "menuStrip1";
            // 
            // VideoDeviceCB
            // 
            this.VideoDeviceCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoDeviceCB.FormattingEnabled = true;
            this.VideoDeviceCB.Location = new System.Drawing.Point(25, 393);
            this.VideoDeviceCB.Name = "VideoDeviceCB";
            this.VideoDeviceCB.Size = new System.Drawing.Size(188, 21);
            this.VideoDeviceCB.TabIndex = 3;
            // 
            // AudioDeviceCB
            // 
            this.AudioDeviceCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AudioDeviceCB.FormattingEnabled = true;
            this.AudioDeviceCB.Location = new System.Drawing.Point(25, 421);
            this.AudioDeviceCB.Name = "AudioDeviceCB";
            this.AudioDeviceCB.Size = new System.Drawing.Size(188, 21);
            this.AudioDeviceCB.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 453);
            this.Controls.Add(this.AudioDeviceCB);
            this.Controls.Add(this.VideoDeviceCB);
            this.Controls.Add(this.SnapshotButton);
            this.Controls.Add(this.VideoPreviewPanel);
            this.Controls.Add(this.MainWindowMenuStrip);
            this.MainMenuStrip = this.MainWindowMenuStrip;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainWindowMenuStrip.ResumeLayout(false);
            this.MainWindowMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel VideoPreviewPanel;
        private System.Windows.Forms.Button SnapshotButton;
        private System.Windows.Forms.ToolStripMenuItem VideoDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AudioDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.MenuStrip MainWindowMenuStrip;
        public System.Windows.Forms.ToolStripMenuItem deviceToolStripMenuItem;
        private System.Windows.Forms.ComboBox VideoDeviceCB;
        private System.Windows.Forms.ComboBox AudioDeviceCB;
    }
}

