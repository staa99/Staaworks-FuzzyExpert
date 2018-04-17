using System.Threading;
using System.Windows.Forms;
using DPUruNet;
using FIngerCapturing.Common;
using System.Collections.Generic;

namespace FingerCapturing
{
    public partial class Verification : Form
    {
        private const int PROBABILITY_ONE = 0x7fffffff;

        private bool reset = false;

        private Thread verifyThreadHandle;

        //private Form_Main _sender;

        //public Form_Main Sender
        //{
        //    get { return _sender; }
        //    set { _sender = value; }
        //}

        private MainObject currentinstance;
        public MainObject Currentinstance
        {
            get { return currentinstance; }
            set { currentinstance = value; }
        }
        public List<Fmd> database = new List<Fmd>();
        public Verification()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open the reader and capture two fingers to compare.
        /// </summary>
        private void VerifyThread()
        {
           
           
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;
            result = currentinstance.CurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result);
                if (currentinstance.CurrentReader != null)
                {
                    currentinstance.CurrentReader.Dispose();
                    currentinstance.CurrentReader = null;
                }
                return;
            }
          
            Fmd fmd1 = null;
          //  Fmd fmd2 = null;

            SendMessage("Place a finger on the reader.");

            int count = 0;
            while (!reset)
            {
                Fid fid = null;

                if (!currentinstance.CaptureFinger(ref fid))
                {
                    break;
                }

                if (fid == null)
                {
                    continue;
                }

                SendMessage("A finger was captured.");
                
                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(fid, Constants.Formats.Fmd.ANSI);

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
                bool pass_threshold = false;
                foreach (Fmd fd  in database)
                {
                    CompareResult compareResult = Comparison.Compare(fmd1, 0, fd, 0);
                    if (compareResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        break;
                    }
                     pass_threshold = compareResult.Score < (PROBABILITY_ONE / 100000);
                    SendMessage("Comparison resulted in a dissimilarity score of " + compareResult.Score.ToString() + (pass_threshold ? " (fingerprints matched)" : " (fingerprints did not match)"));
                    //SendMessage("Place a finger on the reader.");
                    if (pass_threshold)
                    {
                        MessageBox.Show("Verification Successful","Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                    }
                   
                }
                if (!pass_threshold)
                {
                    MessageBox.Show("Verification Failed!!!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (currentinstance.CurrentReader != null)
                currentinstance.CurrentReader.Dispose();
        }

        private delegate void SendMessageCallback(string payload);

        private void SendMessage(string payload)
        {
            if (this.txtVerify.InvokeRequired)
            {
                SendMessageCallback d = new SendMessageCallback(SendMessage);
                this.Invoke(d, new object[] { payload });
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
        private void btnBack_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Start a verification thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Verification_Load(object sender, System.EventArgs e)
        {
            reset = false;

            verifyThreadHandle = new Thread(VerifyThread);
            verifyThreadHandle.IsBackground = true;
            verifyThreadHandle.Start();
        }

        private void Verification_Closed(object sender, System.EventArgs e)
        {
            if (currentinstance.CurrentReader != null)
            {
                reset = true;
                currentinstance.CurrentReader.CancelCapture();

                if (verifyThreadHandle != null)
                {
                    verifyThreadHandle.Join(5000);
                }
            }

            txtVerify.Text = string.Empty;
        }
    }
}