using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MediaCap.Properties;

namespace MediaCap
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private IContainer components = null;

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
            this.VideoDeviceCB = new System.Windows.Forms.ComboBox();
            this.AudioDeviceCB = new System.Windows.Forms.ComboBox();
            this.RecPictureBox = new System.Windows.Forms.PictureBox();
            this.CaptureCheckBox = new System.Windows.Forms.CheckBox();
            this.PreviewCheckBox = new System.Windows.Forms.CheckBox();
            this.SnapshotButton = new System.Windows.Forms.Button();
            this.PreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.VideoCompressorCB = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.RecPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // VideoDeviceCB
            // 
            this.VideoDeviceCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoDeviceCB.FormattingEnabled = true;
            this.VideoDeviceCB.Location = new System.Drawing.Point(107, 499);
            this.VideoDeviceCB.Name = "VideoDeviceCB";
            this.VideoDeviceCB.Size = new System.Drawing.Size(201, 21);
            this.VideoDeviceCB.TabIndex = 3;
            this.VideoDeviceCB.SelectedIndexChanged += new System.EventHandler(this.VideoDeviceCB_SelectedIndexChanged);
            // 
            // AudioDeviceCB
            // 
            this.AudioDeviceCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AudioDeviceCB.FormattingEnabled = true;
            this.AudioDeviceCB.Location = new System.Drawing.Point(107, 523);
            this.AudioDeviceCB.Name = "AudioDeviceCB";
            this.AudioDeviceCB.Size = new System.Drawing.Size(201, 21);
            this.AudioDeviceCB.TabIndex = 4;
            // 
            // RecPictureBox
            // 
            this.RecPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.RecPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RecPictureBox.Image = global::MediaCap.Properties.Resources._59537bae5d80f15cee1b212f1;
            this.RecPictureBox.Location = new System.Drawing.Point(12, 12);
            this.RecPictureBox.Name = "RecPictureBox";
            this.RecPictureBox.Size = new System.Drawing.Size(640, 480);
            this.RecPictureBox.TabIndex = 11;
            this.RecPictureBox.TabStop = false;
            // 
            // CaptureCheckBox
            // 
            this.CaptureCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.CaptureCheckBox.BackgroundImage = global::MediaCap.Properties.Resources.capture;
            this.CaptureCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CaptureCheckBox.Enabled = false;
            this.CaptureCheckBox.Location = new System.Drawing.Point(424, 509);
            this.CaptureCheckBox.Name = "CaptureCheckBox";
            this.CaptureCheckBox.Size = new System.Drawing.Size(49, 48);
            this.CaptureCheckBox.TabIndex = 10;
            this.CaptureCheckBox.UseVisualStyleBackColor = true;
            this.CaptureCheckBox.CheckedChanged += new System.EventHandler(this.CaptureCheckBox_CheckedChanged);
            // 
            // PreviewCheckBox
            // 
            this.PreviewCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.PreviewCheckBox.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.PreviewCheckBox.BackgroundImage = global::MediaCap.Properties.Resources.if_32_171485;
            this.PreviewCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PreviewCheckBox.Location = new System.Drawing.Point(314, 509);
            this.PreviewCheckBox.Name = "PreviewCheckBox";
            this.PreviewCheckBox.Size = new System.Drawing.Size(49, 48);
            this.PreviewCheckBox.TabIndex = 5;
            this.PreviewCheckBox.UseVisualStyleBackColor = false;
            this.PreviewCheckBox.CheckedChanged += new System.EventHandler(this.PreviewCheckBox_CheckedChanged);
            this.PreviewCheckBox.Click += new System.EventHandler(this.PreviewCheckBox_Click);
            // 
            // SnapshotButton
            // 
            this.SnapshotButton.BackgroundImage = global::MediaCap.Properties.Resources.photo_camera;
            this.SnapshotButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.SnapshotButton.Location = new System.Drawing.Point(369, 509);
            this.SnapshotButton.Name = "SnapshotButton";
            this.SnapshotButton.Size = new System.Drawing.Size(49, 48);
            this.SnapshotButton.TabIndex = 7;
            this.SnapshotButton.UseVisualStyleBackColor = true;
            this.SnapshotButton.Click += new System.EventHandler(this.SnapshotButton_Click);
            // 
            // PreviewPictureBox
            // 
            this.PreviewPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.PreviewPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PreviewPictureBox.Location = new System.Drawing.Point(12, 12);
            this.PreviewPictureBox.Name = "PreviewPictureBox";
            this.PreviewPictureBox.Size = new System.Drawing.Size(640, 480);
            this.PreviewPictureBox.TabIndex = 6;
            this.PreviewPictureBox.TabStop = false;
            // 
            // VideoCompressorCB
            // 
            this.VideoCompressorCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoCompressorCB.FormattingEnabled = true;
            this.VideoCompressorCB.Location = new System.Drawing.Point(107, 547);
            this.VideoCompressorCB.Name = "VideoCompressorCB";
            this.VideoCompressorCB.Size = new System.Drawing.Size(201, 21);
            this.VideoCompressorCB.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 502);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Video source";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 527);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Audio source";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 550);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Video compressor";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(479, 522);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Select Directory";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 573);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VideoCompressorCB);
            this.Controls.Add(this.RecPictureBox);
            this.Controls.Add(this.CaptureCheckBox);
            this.Controls.Add(this.AudioDeviceCB);
            this.Controls.Add(this.VideoDeviceCB);
            this.Controls.Add(this.PreviewCheckBox);
            this.Controls.Add(this.SnapshotButton);
            this.Controls.Add(this.PreviewPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "MediaCap";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.RecPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ComboBox VideoDeviceCB;
        private ComboBox AudioDeviceCB;
        private CheckBox PreviewCheckBox;
        private PictureBox PreviewPictureBox;
        private Button SnapshotButton;
        private CheckBox CaptureCheckBox;
        private PictureBox RecPictureBox;
        private ComboBox VideoCompressorCB;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button button1;
        private FolderBrowserDialog FolderBrowserDialog;
    }
}

