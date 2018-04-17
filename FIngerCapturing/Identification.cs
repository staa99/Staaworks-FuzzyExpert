using System.Threading;
using System.Windows.Forms;
using DPUruNet;

namespace FingerCapturing
{
    public partial class Identification : Form
    {
        private const int DPFJ_PROBABILITY_ONE = 0x7fffffff;

        private bool reset = false;

        private Thread identifyThreadHandle;

        public Form_Main Sender { get; set; }

        public Identification()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open a reader and capture three fingers to identify with.
        /// </summary>
        private void IdentifyThread()
        {
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

            result = Sender.CurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                MessageBox.Show("Error:  " + result);
                if (Sender.CurrentReader != null)
                {
                    Sender.CurrentReader.Dispose();
                    Sender.CurrentReader = null;
                }
                return;
            }

            Fmd fmd1 = null;
            Fmd fmd2 = null;
            Fmd fmd3 = null;

            SendMessage("Place your right index finger on the reader.");

            int count = 0;
            while (!reset)
            {
                Fid fid = null;

                if (!Sender.CaptureFinger(ref fid))
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

                if (count == 0)
                {
                    fmd1 = resultConversion.Data;
                    count += 1;
                    SendMessage("Now place your right thumb on the reader.");
                }
                else if (count == 1)
                {
                    fmd2 = resultConversion.Data;
                    count += 1;
                    SendMessage("Now place any finger on the reader.");
                }
                else if (count == 2)
                {
                    fmd3 = resultConversion.Data;
                    Fmd[] fmds = new Fmd[2];
                    fmds[0] = fmd1;
                    fmds[1] = fmd2;

                    // See the SDK documentation for an explanation on threshold scores.
                    int thresholdScore = DPFJ_PROBABILITY_ONE * 1 / 100000; 

                    IdentifyResult identifyResult = Comparison.Identify(fmd3, 0, fmds, thresholdScore, 2); 

                    if (identifyResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        break; 
                    }

                    SendMessage("Identification resulted in the following number of matches: " + identifyResult.Indexes.Length.ToString());
                    SendMessage("Place your right index finger on the reader.");
                    count = 0;
                }
            }

            if (Sender.CurrentReader != null)
                Sender.CurrentReader.Dispose();
        }

        private delegate void SendMessageCallback(string payload);

        private void SendMessage(string payload)
        {
            if (txtIdentify.InvokeRequired)
            {
                SendMessageCallback d = new SendMessageCallback(SendMessage);
                this.Invoke(d, new object[] { payload });
            }
            else
            {
                txtIdentify.Text += payload +"\r\n\r\n";
                txtIdentify.SelectionStart = txtIdentify.TextLength;
                txtIdentify.ScrollToCaret();
            }
        }

        /// <summary>
        /// Cancel, wait for the thread to die, and dispose of the reader.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Identification_Load(object sender, System.EventArgs e)
        {
            reset = false;

            identifyThreadHandle = new Thread(IdentifyThread);
            identifyThreadHandle.IsBackground = true;
            identifyThreadHandle.Start();
        }

        private void Identification_Closed(object sender, System.EventArgs e)
        {
            if (Sender.CurrentReader != null)
            {
                reset = true;
                Sender.CurrentReader.CancelCapture();

                if (identifyThreadHandle != null)
                {
                    identifyThreadHandle.Join(5000);
                }
            }

            txtIdentify.Text = string.Empty;
        }
    }
}