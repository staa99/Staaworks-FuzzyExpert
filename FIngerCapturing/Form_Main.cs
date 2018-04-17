using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;
using DPUruNet;
using FIngerCapturing.Common;

namespace FingerCapturing
{
    public partial class Form_Main : Form
    {
        MainObject mobj = new MainObject();
        /// <summary>
        /// Holds fmds enrolled by the enrollment GUI.
        /// </summary>
        public Dictionary<int, Fmd> Fmds
        {
            get { return fmds; }
            set { fmds = value; }
        }
        private Dictionary<int, Fmd> fmds = new Dictionary<int, Fmd>();

        public Form_Main()
        {
            InitializeComponent();
        }

        // When set by child forms, shows s/n and enables buttons.
        public Reader CurrentReader
        {
            get { return currentReader; }
            set
            {
                currentReader = value;
                SendMessage(Action.UpdateReaderState, value);
            }
        }
        private Reader currentReader;

        private ReaderSelection _readerSelection;
        private void btnReaderSelect_Click(System.Object sender, System.EventArgs e)
        {
            if (_readerSelection == null)
            {
                _readerSelection = new ReaderSelection();
               // _readerSelection.Sender = this;
                _readerSelection.Currentinstance = mobj;
            }

            _readerSelection.ShowDialog();
        }

        // Whether streaming or capture was chosen.
        public bool streamingOn;

        private Capture_Stream _captureStream;
        private void btnCapture_Click(System.Object sender, System.EventArgs e)
        {
            streamingOn = false;

            if (_captureStream == null)
            {
                _captureStream = new Capture_Stream();
                _captureStream.Sender = this;
            }

            _captureStream.ShowDialog();

            _captureStream.Dispose();
            _captureStream = null;
        }

        private Verification _verification;
        private void btnVerify_Click(System.Object sender, System.EventArgs e)
        {
            if (_verification == null)
            {
                _verification = new Verification();
               // _verification.Sender = mobj;
            }

            _verification.ShowDialog();
        }

        private Identification _identification;
        private void btnIdentify_Click(System.Object sender, System.EventArgs e)
        {
            if (_identification == null)
            {
                _identification = new Identification();
                _identification.Sender = this;
            }

            _identification.ShowDialog();
        }

        private Enrollment _enrollment;
        private void btnEnroll_Click(System.Object sender, System.EventArgs e)
        {
            if (_enrollment == null)
            {
                _enrollment = new Enrollment();
                _enrollment.Sender = this;
            }

            _enrollment.ShowDialog();
        }

        private void btnStreaming_Click(System.Object sender, System.EventArgs e)
        {
            streamingOn = true;

            if (_captureStream == null)
            {
                _captureStream = new Capture_Stream();
                _captureStream.Sender = this;
            }

            _captureStream.ShowDialog();
            
            _captureStream.Dispose();
            _captureStream = null;
        }

        
        /// <summary>
        /// Function to capture a finger. Always get status first and calibrate or wait if necessary.  Always check status and capture errors.
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public bool CaptureFinger(ref Fid fid)
        {
            try
            {
                Constants.ResultCode result = currentReader.GetStatus();

                if ((result != Constants.ResultCode.DP_SUCCESS))
                {
                    MessageBox.Show("Get Status Error:  " + result);
                    if (CurrentReader != null)
                    {
                        CurrentReader.Dispose();
                        CurrentReader = null;
                    }
                    return false;
                }

                if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
                {
                    Thread.Sleep(50);
                    return true;
                }
                else if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
                {
                    currentReader.Calibrate();
                }
                else if ((currentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
                {
                    MessageBox.Show("Get Status:  " + currentReader.Status.Status);
                    if (CurrentReader != null)
                    {
                        CurrentReader.Dispose();
                        CurrentReader = null;
                    }
                    return false;
                }

                CaptureResult captureResult = currentReader.Capture(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, 5000, currentReader.Capabilities.Resolutions[0]);

                if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    MessageBox.Show("Error:  " + captureResult.ResultCode);
                    if (CurrentReader != null)
                    {
                        CurrentReader.Dispose();
                        CurrentReader = null;
                    }
                    return false;
                }

                if (captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_CANCELED)
                {
                    return false;
                }

                if ((captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_NO_FINGER || captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_TIMED_OUT))
                {
                    return true;
                }

                if ((captureResult.Quality == Constants.CaptureQuality.DP_QUALITY_FAKE_FINGER))
                {
                    MessageBox.Show("Quality Error:  " + captureResult.Quality);
                    return true;
                }

                fid = captureResult.Data;

                return true;
            }
            catch
            {
                MessageBox.Show("An error has occurred.");
                if (CurrentReader != null)
                {
                    CurrentReader.Dispose();
                    CurrentReader = null;
                }
                return false;
            }
        }

        EnrollmentControl enrollmentControl;
        private void btnEnrollmentControl_Click(object sender, EventArgs e)
        {
            if (enrollmentControl == null)
            {
                enrollmentControl = new EnrollmentControl();
                // enrollmentControl.Sender = new MainObject(); // this;
              
                mobj.CurrentReader = this.CurrentReader;
                enrollmentControl.Currentinstance = mobj; // this;
            }

            enrollmentControl.ShowDialog();
        }

        IdentificationControl identificationControl;
        private void btnIdentificationControl_Click(object sender, EventArgs e)
        {
            if (identificationControl == null)
            {
                identificationControl = new IdentificationControl();
                identificationControl.Sender = this;
            }

            identificationControl.ShowDialog();
        }

        private enum Action
        {
           UpdateReaderState 
        }

        private delegate void SendMessageCallback(Action state, object payload);

        private void SendMessage(Action state, object payload)
        {
            if (this.txtReaderSelected.InvokeRequired)
            {
                SendMessageCallback d = new SendMessageCallback(SendMessage);
                this.Invoke(d, new object[] { state, payload });
            }
            else
            {
                switch (state)
                {
                    case Action.UpdateReaderState:
                        if ((Reader)payload != null)
                        {
                            txtReaderSelected.Text = ((Reader)payload).Description.SerialNumber;
                            btnCapture.Enabled = true;
                            btnStreaming.Enabled = true;
                            btnVerify.Enabled = true;
                            btnIdentify.Enabled = true;
                            btnEnroll.Enabled = true;
                            btnEnrollmentControl.Enabled = true;
                            if (fmds.Count > 0)
                            {
                                btnIdentificationControl.Enabled = true;
                            }
                        }
                        else
                        {
                            txtReaderSelected.Text = String.Empty;
                            btnCapture.Enabled = false;
                            btnStreaming.Enabled = false;
                            btnVerify.Enabled = false;
                            btnIdentify.Enabled = false;
                            btnEnroll.Enabled = false;
                            btnEnrollmentControl.Enabled = false;
                            btnIdentificationControl.Enabled = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}