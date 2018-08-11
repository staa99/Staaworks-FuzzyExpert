using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using DPUruNet;

namespace FingerCapturing
{
    public partial class Enrollment : Form
    {
        private bool reset = false;

        private Thread enrollThreadHandle;
        
        private Form_Main _sender;

        public Form_Main Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public Enrollment()
        {
            InitializeComponent();
        }

        int count = 0;
        private IEnumerable<Fmd> CaptureAndExtractFmd()
        {
            while (!reset)
            {
                DataResult<Fmd> resultConversion;

                try
                {
                    if (count >= 8)
                    {
                        SendMessage("Enrollment was unsuccessful.  Please try again.");
                        count = 0;
                        break;
                    }

                    Fid fid = null;
                    if (!_sender.CaptureFinger(ref fid))
                    {
                        break;
                    }

                    if (fid == null)
                    {
                        continue;
                    }

                    count++;

                    resultConversion = FeatureExtraction.CreateFmdFromFid(fid, Constants.Formats.Fmd.ANSI);

                    SendMessage("A finger was captured.  \r\nCount:  " + (count));

                    if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }

                yield return resultConversion.Data;
            }
        }

        /// <summary>
        /// Open a reader and begin capturing fingers. After four captures, start attempting an enroll.
        /// Display when a successful enrollment occurred.
        /// </summary>
        private void EnrollThread()
        {
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

            result = _sender.CurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result);
                if (_sender.CurrentReader != null)
                {
                    _sender.CurrentReader.Dispose();
                    _sender.CurrentReader = null;
                }
                return;
            }

            count = 0;
            while (!reset)
            {
                if (_sender.CurrentReader == null)
                {
                    break;
                }

                SendMessage("Place a finger on the reader.");

                DataResult<Fmd> resultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.ANSI, CaptureAndExtractFmd());

                if (resultEnrollment.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    SendMessage("An enrollment FMD was successfully created.");
                    count = 0;
                }
            }

            if (_sender.CurrentReader != null)
                _sender.CurrentReader.Dispose();
        }

        private delegate void SendMessageCallback(string payload);

        private void SendMessage(string payload)
        {
            if (txtEnroll.InvokeRequired)
            {
                var d = new SendMessageCallback(SendMessage);
                Invoke(d, new object[] { payload });
            }
            else
            {
                txtEnroll.Text += payload + "\r\n\r\n";
                txtEnroll.SelectionStart = txtEnroll.TextLength;
                txtEnroll.ScrollToCaret();
            }
        }

        /// <summary>
        /// Cancel, wait for the thread to die, and dispose of the reader.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack_Click (object sender, EventArgs e) => Close();

        /// <summary>
        /// Start an enrollment thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enrollment_Load(object sender, EventArgs e)
        {
            reset = false;

            enrollThreadHandle = new Thread(EnrollThread);
            enrollThreadHandle.IsBackground = true;
            enrollThreadHandle.Start();
        }

        private void Enrollment_Closed(object sender, EventArgs e)
        {
            if (_sender.CurrentReader != null)
            {
                reset = true;
                _sender.CurrentReader.CancelCapture();

                if (enrollThreadHandle != null)
                {
                    enrollThreadHandle.Join(5000);
                }
            }

            txtEnroll.Text = string.Empty;
        }
    }
}
