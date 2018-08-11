using System.Threading;
using System.Windows.Forms;
using DPUruNet;
using FIngerCapturing.Common;
using System.Collections.Generic;
using System;

namespace FingerCapturing
{
    public partial class Verification : Form
    {
        private const int PROBABILITY_ONE = 0x7fffffff;

        private bool reset = false;

        private Thread verifyThreadHandle;
        public string SuccessReading { get; private set; }
        public MainObject Currentinstance { get; set; }
        public List<Fmd> database = new List<Fmd>();
        private Action<bool> OnComplete { get; }
        public Verification (Action<bool> onComplete)
        {
            InitializeComponent();
            OnComplete = onComplete;
        }

        /// <summary>
        /// Open the reader and capture two fingers to compare.
        /// </summary>
        private void VerifyThread ()
        {
            var result = Constants.ResultCode.DP_DEVICE_FAILURE;
            result = Currentinstance.CurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result);
                if (Currentinstance.CurrentReader != null)
                {
                    Currentinstance.CurrentReader.Dispose();
                    Currentinstance.CurrentReader = null;
                }
                return;
            }

            Fmd fmd1 = null;
            //  Fmd fmd2 = null;

            SendMessage("Place a finger on the reader.");
            while (!reset)
            {
                Fid fid = null;

                if (!Currentinstance.CaptureFinger(ref fid))
                {
                    break;
                }

                if (fid == null)
                {
                    continue;
                }

                SendMessage("A finger was captured.");

                var resultConversion = FeatureExtraction.CreateFmdFromFid(fid, Constants.Formats.Fmd.ANSI);

                if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    break;
                }

                /* if (count == 0)
                 {
                     fmd1 = resultConversion.Data;
                     count += 1;
                     SendMessage("Now place the same or a different finger on the reader.");
                 }
                 else if (count == 1)
                 {
                     fmd2 = resultConversion.Data;
                     CompareResult compareResult = Comparison.Compare(fmd1, 0, fmd2, 0);

                     if (compareResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                     {
                         break; 
                     }

                     SendMessage("Comparison resulted in a dissimilarity score of " + compareResult.Score.ToString() + (compareResult.Score < (PROBABILITY_ONE/100000) ? " (fingerprints matched)" : " (fingerprints did not match)"));
                     SendMessage("Place a finger on the reader.");
                     count = 0;
                 }*/

                fmd1 = resultConversion.Data;
                var pass_threshold = false;
                foreach (var fd in database)
                {
                    var compareResult = Comparison.Compare(fmd1, 0, fd, 0);
                    if (compareResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        break;
                    }
                    pass_threshold = compareResult.Score < (PROBABILITY_ONE / 100000);
                    SendMessage("Comparison resulted in a dissimilarity score of " + compareResult.Score.ToString() + (pass_threshold ? " (fingerprints matched)" : " (fingerprints did not match)"));
                    //SendMessage("Place a finger on the reader.");
                    if (pass_threshold)
                    {
                        break;
                    }
                }

                if (Currentinstance.CurrentReader != null)
                {
                    Currentinstance.CurrentReader.Dispose();
                    Currentinstance.CurrentReader = null;
                }

                FormClosed += (o, e) =>
                {
                    if (pass_threshold)
                    {
                        SuccessReading = Fmd.SerializeXml(fmd1);
                    }

                    OnComplete(pass_threshold);
                };

                verifyThreadHandle.Abort();
            }

            if (Currentinstance.CurrentReader != null)
            {
                Currentinstance.CurrentReader.Dispose();
                Currentinstance.CurrentReader = null;
            }
        }

        private delegate void SendMessageCallback (string payload);

        private void SendMessage (string payload)
        {
            if (txtVerify.InvokeRequired)
            {
                var d = new SendMessageCallback(SendMessage);
                Invoke(d, new object[] { payload });
            }
            else
            {
                txtVerify.Text += payload + "\r\n\r\n";
                txtVerify.SelectionStart = txtVerify.TextLength;
                txtVerify.ScrollToCaret();
            }
        }

        /// <summary>
        /// Cancel the capture and once thre verify thread dies, dispose of the reader and close the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack_Click (object sender, EventArgs e) => Close();

        /// <summary>
        /// Start a verification thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Verification_Load (object sender, System.EventArgs e)
        {
            reset = false;

            verifyThreadHandle = new Thread(VerifyThread)
            {
                IsBackground = true
            };
            verifyThreadHandle.Start();
        }

        private void Verification_Closed (object sender, System.EventArgs e)
        {
            if (Currentinstance.CurrentReader != null)
            {
                reset = true;
                Currentinstance.CurrentReader.CancelCapture();

                if (verifyThreadHandle != null)
                {
                    verifyThreadHandle.Join(5000);
                }
            }

            txtVerify.Text = string.Empty;
        }
    }
}