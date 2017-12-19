using System;
using System.Windows.Forms;
using CaptureMe;
using System.Drawing;

namespace MediaCap
{
    public partial class MainForm : Form
    {
        public static System.Drawing.Drawing2D.GraphicsPath BuildTransparencyPath(Image im)
        {
            int x;
            int y;
            Bitmap bmp = new Bitmap(im);
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            Color mask = bmp.GetPixel(0, 0);

            for (x = 0; x <= bmp.Width - 1; x++)
            {
                for (y = 0; y <= bmp.Height - 1; y++)
                {
                    if (!bmp.GetPixel(x, y).Equals(mask))
                    {
                        gp.AddRectangle(new Rectangle(x, y, 1, 1));
                    }
                }
            }
            bmp.Dispose();
            return gp;
        }

        private CaptureClass _captureClass;

        private bool _isPreviewStarted;

        private bool _isCaptureStarted;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = BuildTransparencyPath(RecPictureBox.Image);
            RecPictureBox.Region = new Region(gp);
            _captureClass = new CaptureClass();
            string[] videoDevices = _captureClass.GetVideoDevices();
            string[] audioDevices = _captureClass.GetAudioDevices();
            string[] videoCompressors = _captureClass.GetVideoCompressors();
            foreach (string videoDevice in videoDevices)
            {
                VideoDeviceCB.Items.Add(videoDevice);
            }
            foreach (string audioDevice in audioDevices)
            {
                AudioDeviceCB.Items.Add(audioDevice);
            }
            foreach (string videoCompressor in videoCompressors)
            {
                VideoCompressorCB.Items.Add(videoCompressor);
            }
            RecPictureBox.Visible = false;
            PreviewCheckBox.Enabled = false;
            //CaptureCheckBox.Enabled = false;
            SnapshotButton.Enabled = false;
            VideoDeviceCB.SelectedIndex = 0;
            AudioDeviceCB.SelectedIndex = 0;
            VideoCompressorCB.SelectedIndex = 0;
        }

        private void PreviewCheckBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (VideoDeviceCB.SelectedIndex == 0)
                {
                    PreviewCheckBox.Checked = false;
                }
                else
                {
                    _captureClass.SetVideoSource(VideoDeviceCB.SelectedIndex - 1);
                    if(AudioDeviceCB.SelectedIndex != 0)
                        _captureClass.SetAudioSource(AudioDeviceCB.SelectedIndex - 1);
                    if (VideoCompressorCB.SelectedIndex != 0)
                        _captureClass.SetVideoCompressor(VideoCompressorCB.SelectedIndex - 1);
                    if (!_isPreviewStarted)
                    {
                        _captureClass.StartPreview(ref PreviewPictureBox);
                        SnapshotButton.Enabled = true;
                        CaptureCheckBox.Enabled = true;
                        VideoDeviceCB.Enabled = false;
                        AudioDeviceCB.Enabled = false;
                        VideoCompressorCB.Enabled = false;
                        _isPreviewStarted = !_isPreviewStarted;
                    }
                    else
                    {
                        _captureClass.StopPreview();
                        SnapshotButton.Enabled = false;
                        CaptureCheckBox.Enabled = false;
                        VideoDeviceCB.Enabled = true;
                        AudioDeviceCB.Enabled = true;
                        VideoCompressorCB.Enabled = true;
                        _isPreviewStarted = !_isPreviewStarted;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
        }

        private void SnapshotButton_Click(object sender, EventArgs e)
        {
            _captureClass.SaveImage(ref PreviewPictureBox);
        }

        private void CaptureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {            
                if (!_isCaptureStarted)
                {
                    _captureClass.SetVideoSource(VideoDeviceCB.SelectedIndex - 1);
                    if (AudioDeviceCB.SelectedIndex != 0)
                        _captureClass.SetAudioSource(AudioDeviceCB.SelectedIndex - 1);
                    if (VideoCompressorCB.SelectedIndex != 0)
                        _captureClass.SetVideoCompressor(VideoCompressorCB.SelectedIndex - 1);
                    _captureClass.StartCapture();
                    RecPictureBox.Visible = true;
                    _isCaptureStarted = !_isCaptureStarted;
                }
                else
                {
                
                    _captureClass.StopCapture();
                    RecPictureBox.Visible = false;
                    _isCaptureStarted = !_isCaptureStarted;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
            }
        }

        private void VideoDeviceCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VideoDeviceCB.SelectedIndex != 0)
                PreviewCheckBox.Enabled = true;
            else
            {
                PreviewCheckBox.Enabled = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void PreviewCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _captureClass.SetUserPath(FolderBrowserDialog.SelectedPath);
            }
        }
    }
}
