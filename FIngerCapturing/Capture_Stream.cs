using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System;
using DPUruNet;

namespace FingerCapturing
{
    public partial class Capture_Stream : Form
    {
        private bool backEnabled = false;

        private bool reset = false;

        private bool threadHandle_lock = false;

        private Thread threadHandle;

        private Form_Main _sender;
        public Form_Main Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        private bool streamingOn;

        public Capture_Stream()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the form and start either a capture or streaming thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Capture_Stream_Load(object sender, EventArgs e)
        {
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

            result = _sender.CurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result.ToString());
                if (_sender.CurrentReader != null)
                {
                    _sender.CurrentReader.Dispose();
                    _sender.CurrentReader = null;
                }
                return;
            }

            // Check if streaming or capture was chosen.
            streamingOn = _sender.streamingOn;

            pbFingerprint.Image = null;

            if (streamingOn)
            {
                if (!_sender.CurrentReader.Capabilities.CanStream)
                {
                    MessageBox.Show("This reader cannot stream in this environment.");

                    backEnabled = true;
                    
                    this.Close();

                    reset = true;

                    _sender.CurrentReader.Dispose();

                    return;
                }

                this.Text = "Streaming";
                threadHandle = new Thread(StreamThread);
                threadHandle.IsBackground = true;
                threadHandle.Start();
            }
            else
            {
                this.Text = "Capture";
                threadHandle = new Thread(CaptureThread);
                threadHandle.IsBackground = true;
                threadHandle.Start();
            }
        }

        /// <summary>
        /// Until a break occurs, capture finger and display the fid as a bitmap.
        /// </summary>
        private void CaptureThread()
        {
            reset = false;
            while (!reset)
            {
                Fid fid = null;

                if (!_sender.CaptureFinger(ref fid))
                {
                    break;
                }

                if (fid == null)
                {
                    continue;
                }

                foreach (Fid.Fiv fiv in fid.Views)
                {
                    SendMessage(CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                }
            }

            if (_sender.CurrentReader != null)
                _sender.CurrentReader.Dispose();
        }

        /// <summary>
        /// Check reader status.  If okay, stream an image and display as a bitmap.
        /// </summary>
        public void StreamThread()
        {
            // A lock to only allow one thread to one at a time.
            threadHandle_lock = true;

            _sender.CurrentReader.StartStreaming();

            // A flag to allow back to be pressed only after opening and starting a stream.
            backEnabled = true;

            // A reset flag to stop the stream image loop when the form is closed.
            reset = false;

            while ((!reset))
            {
                CaptureResult captureResult = null;

                Constants.ResultCode result = _sender.CurrentReader.GetStatus();

                #region Validate GetStatus
                if ((result != Constants.ResultCode.DP_SUCCESS))
                {
                    MessageBox.Show("Get Status Error:  " + result);
                    if (_sender.CurrentReader != null)
                    {
                        _sender.CurrentReader.Dispose();
                        _sender.CurrentReader = null;
                    }
                    reset = true;
                    continue;
                }

                if ((_sender.CurrentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
                {
                    Thread.Sleep(50);
                    continue;
                }
                else if ((_sender.CurrentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
                {
                    _sender.CurrentReader.Calibrate();
                }
                else if ((_sender.CurrentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
                {
                    MessageBox.Show("Get Status:  " + _sender.CurrentReader.Status.Status);
                    if (_sender.CurrentReader != null)
                    {
                        _sender.CurrentReader.Dispose();
                        _sender.CurrentReader = null;
                    }
                    reset = true;
                    continue;
                } 
                #endregion

                captureResult = _sender.CurrentReader.GetStreamImage(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, _sender.CurrentReader.Capabilities.Resolutions[0]);

                #region Validate GetStreamImage
                if (captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_TIMED_OUT)
                {
                    Thread.Sleep(50);
                    continue;
                }

                if ((captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS || captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_CANCELED))
                {
                    MessageBox.Show("Error:  " + captureResult.ResultCode);
                    if (_sender.CurrentReader != null)
                    {
                        _sender.CurrentReader.StopStreaming();
                        _sender.CurrentReader.Dispose();
                        _sender.CurrentReader = null;
                    }
                    reset = true;
                    continue;
                }

                if ((captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_NO_FINGER || captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_FAKE_FINGER))
                {
                    continue;
                } 
                #endregion

                foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    SendMessage(CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                }
            }

            if (_sender.CurrentReader != null)
            {
                Constants.ResultCode result2 = _sender.CurrentReader.StopStreaming();

                if (result2 != Constants.ResultCode.DP_SUCCESS && result2 != Constants.ResultCode.DP_NOT_IMPLEMENTED)
                {
                    MessageBox.Show("Error:  " + result2.ToString());
                }
            }

            if (_sender.CurrentReader != null)
                _sender.CurrentReader.Dispose();
            
            threadHandle_lock = false;
        }

        /// <summary>
        /// Create a bitmap from raw data in row/column format.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Bitmap CreateBitmap(byte[] bytes, int width, int height)
        {
            byte[] rgbBytes = new byte[bytes.Length * 3];

            for (var i = 0; i <= bytes.Length - 1; i++)
            {
                rgbBytes[(i * 3)] = bytes[i];
                rgbBytes[(i * 3) + 1] = bytes[i];
                rgbBytes[(i * 3) + 2] = bytes[i];
            }
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            for (var i = 0; i <= bmp.Height - 1; i++)
            {
                IntPtr p = new IntPtr(data.Scan0.ToInt32() + data.Stride * i);
                System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
            }

            bmp.UnlockBits(data);

            return bmp;
        }

        private delegate void SendMessageCallback(object payload);

        private void SendMessage(object payload)
        {
            try
            {
                if (this.pbFingerprint.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { payload });
                }
                else
                {
                    pbFingerprint.Image = (Bitmap)payload;
                    pbFingerprint.Refresh();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Call StopStreaming to stop a stream, and CancelCapture to stop a capture.  
        /// Wait unti the thread is no longer alive, then dispose the reader, which 
        /// closes the reader handle to unmanaged code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Capture_Stream_Closed(object sender, EventArgs e)
        {
            if (_sender.CurrentReader != null)
            {
                if (streamingOn)
                {
                    // Waits until reader is open and streaming is on before resetting.
                    var count = 0;
                    while (!backEnabled && count++ < 30)
                    {
                        Thread.Sleep(250);
                        Application.DoEvents();
                    }

                    reset = true;

                    // Waits until thread is unlocked before continuing.
                    count = 0;
                    while (threadHandle_lock && count++ < 100)
                    {
                        Thread.Sleep(50);
                        Application.DoEvents();
                    }
                }
                else
                {
                    reset = true;
                    _sender.CurrentReader.CancelCapture();

                    if (threadHandle != null)
                    {
                        threadHandle.Join(5000);
                    }
                }
            }

            // Disable flags in this thread.
            backEnabled = false;
            threadHandle_lock = false;
        }
    }
}