namespace MediaCap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.deviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VideoDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AudioDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainWindowMenuStrip = new System.Windows.Forms.MenuStrip();
            this.VideoDeviceCB = new System.Windows.Forms.ComboBox();
            this.AudioDeviceCB = new System.Windows.Forms.ComboBox();
            this.PreviewCheckBox = new System.Windows.Forms.CheckBox();
            this.PreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.SnapshotButton = new System.Windows.Forms.Button();
            this.MainWindowMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).BeginInit();
            this.SuspendLayout();
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
            this.MainWindowMenuStrip.Size = new System.Drawing.Size(664, 24);
            this.MainWindowMenuStrip.TabIndex = 0;
            this.MainWindowMenuStrip.Text = "menuStrip1";
            // 
            // VideoDeviceCB
            // 
            this.VideoDeviceCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoDeviceCB.FormattingEnabled = true;
            this.VideoDeviceCB.Location = new System.Drawing.Point(99, 528);
            this.VideoDeviceCB.Name = "VideoDeviceCB";
            this.VideoDeviceCB.Size = new System.Drawing.Size(201, 21);
            this.VideoDeviceCB.TabIndex = 3;
            // 
            // AudioDeviceCB
            // 
            this.AudioDeviceCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AudioDeviceCB.FormattingEnabled = true;
            this.AudioDeviceCB.Location = new System.Drawing.Point(99, 555);
            this.AudioDeviceCB.Name = "AudioDeviceCB";
            this.AudioDeviceCB.Size = new System.Drawing.Size(201, 21);
            this.AudioDeviceCB.TabIndex = 4;
            // 
            // PreviewCheckBox
            // 
            this.PreviewCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.PreviewCheckBox.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.PreviewCheckBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PreviewCheckBox.BackgroundImage")));
            this.PreviewCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.PreviewCheckBox.Location = new System.Drawing.Point(306, 528);
            this.PreviewCheckBox.Name = "PreviewCheckBox";
            this.PreviewCheckBox.Size = new System.Drawing.Size(49, 48);
            this.PreviewCheckBox.TabIndex = 5;
            this.PreviewCheckBox.UseVisualStyleBackColor = false;
            this.PreviewCheckBox.Click += new System.EventHandler(this.PreviewCheckBox_Click);
            // 
            // PreviewPictureBox
            // 
            this.PreviewPictureBox.Location = new System.Drawing.Point(12, 36);
            this.PreviewPictureBox.Name = "PreviewPictureBox";
            this.PreviewPictureBox.Size = new System.Drawing.Size(640, 480);
            this.PreviewPictureBox.TabIndex = 6;
            this.PreviewPictureBox.TabStop = false;
            // 
            // SnapshotButton
            // 
            this.SnapshotButton.BackgroundImage = global::MediaCap.Properties.Resources.photo_camera;
            this.SnapshotButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SnapshotButton.Location = new System.Drawing.Point(361, 528);
            this.SnapshotButton.Name = "SnapshotButton";
            this.SnapshotButton.Size = new System.Drawing.Size(49, 48);
            this.SnapshotButton.TabIndex = 7;
            this.SnapshotButton.UseVisualStyleBackColor = true;
            this.SnapshotButton.Click += new System.EventHandler(this.SnapshotButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 588);
            this.Controls.Add(this.AudioDeviceCB);
            this.Controls.Add(this.VideoDeviceCB);
            this.Controls.Add(this.PreviewCheckBox);
            this.Controls.Add(this.SnapshotButton);
            this.Controls.Add(this.PreviewPictureBox);
            this.Controls.Add(this.MainWindowMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.MainWindowMenuStrip;
            this.Name = "MainForm";
            this.Text = "MediaCap";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MainWindowMenuStrip.ResumeLayout(false);
            this.MainWindowMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem VideoDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AudioDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.MenuStrip MainWindowMenuStrip;
        public System.Windows.Forms.ToolStripMenuItem deviceToolStripMenuItem;
        private System.Windows.Forms.ComboBox VideoDeviceCB;
        private System.Windows.Forms.ComboBox AudioDeviceCB;
        private System.Windows.Forms.CheckBox PreviewCheckBox;
        private System.Windows.Forms.PictureBox PreviewPictureBox;
        private System.Windows.Forms.Button SnapshotButton;
    }
}

