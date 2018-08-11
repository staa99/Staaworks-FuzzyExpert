using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using DPUruNet;
using FIngerCapturing.Common;

namespace FingerCapturing
{
    public partial class EnrollmentControl : Form
    {
        private DPCtlUruNet.EnrollmentControl _enrollmentControl;

        public delegate void saveandclose();
        public event saveandclose SCevent;
        public MainObject Currentinstance { get; set; }

        public EnrollmentControl()
        {
            InitializeComponent();
        }

        private void enrollment_OnCancel(DPCtlUruNet.EnrollmentControl enrollmentControl, Constants.ResultCode result, int fingerPosition)
        {
            if (enrollmentControl.Reader != null)
            {
                ShowMessage("OnCancel:  " + enrollmentControl.Reader.Description.Name + ", finger " + fingerPosition);
            }
            else
            {
                ShowMessage("OnCancel:  No Reader Connected, finger " + fingerPosition);
            }

            btnCancel.Enabled = false;
        }

        private void enrollment_OnCaptured(DPCtlUruNet.EnrollmentControl enrollmentControl, CaptureResult captureResult, int fingerPosition)
        {
            if (enrollmentControl.Reader != null)
            {
                ShowMessage("OnCaptured:  " + enrollmentControl.Reader.Description.Name + ", finger " + fingerPosition + ", quality " + captureResult.Quality.ToString());
            }
            else
            {
                ShowMessage("OnCaptured:  No Reader Connected, finger " + fingerPosition);
            }

            if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
            {
                if (Currentinstance.CurrentReader != null)
                {
                    Currentinstance.CurrentReader.Dispose();
                    Currentinstance.CurrentReader = null;
                }

                // Disconnect reader from enrollment control.
                _enrollmentControl.Reader = null; 
                MessageBox.Show("Error:  " + captureResult.ResultCode);
                btnCancel.Enabled = false;
               
            }
            else
            {
                if (pbFingerprint.Image != null)
                {
                    pbFingerprint.Image.Dispose();
                    pbFingerprint.Image = null;
                }

                foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    
                    pbFingerprint.Image = CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height);
                    
                }
               
                if (Currentinstance.Fids.ContainsKey(fingerPosition))
                {
                    List<Fid> fds = Currentinstance.Fids[fingerPosition];
                    fds.Add(captureResult.Data);
                  //  MessageBox.Show("Add New FID for finger "+fingerPosition+" Total count:"+fds.Count );
                    
                }
                else
                {

                    List<Fid> fds = new List<Fid>();
                    fds.Add(captureResult.Data);
                    Currentinstance.Fids.Add(fingerPosition, fds);
                  //  MessageBox.Show("Add New FID for finger " + fingerPosition + " Total count:" + fds.Count);
                }
               
            }
        }

        private void enrollment_OnDelete(DPCtlUruNet.EnrollmentControl enrollmentControl, Constants.ResultCode result, int fingerPosition)
        {
            if (enrollmentControl.Reader != null)
            {
                ShowMessage("OnDelete:  " + enrollmentControl.Reader.Description.Name + ", finger " + fingerPosition);
            }
            else
            {
                ShowMessage("OnDelete:  No Reader Connected, finger " + fingerPosition);
            }

            Currentinstance.Fmds.Remove(fingerPosition);
            Currentinstance.Fids.Remove(fingerPosition);

            if (Currentinstance.Fmds.Count == 0) { }
               // currentinstance.btnIdentificationControl.Enabled = false;
        }

        private void enrollment_OnEnroll(DPCtlUruNet.EnrollmentControl enrollmentControl, DataResult<Fmd> result, int fingerPosition)
        {
            if (enrollmentControl.Reader != null)
            {
                ShowMessage("OnEnroll:  " + enrollmentControl.Reader.Description.Name + ", finger " + fingerPosition);
            }
            else
            {
                ShowMessage("OnEnroll:  No Reader Connected, finger " + fingerPosition);
            }

            if (result != null && result.Data != null)
            {
               // Fmd.SerializeXml(result.Data);
             
                Currentinstance.Fmds.Add(fingerPosition, result.Data);
            }

            btnCancel.Enabled = false;

           // currentinstance.btnIdentificationControl.Enabled = true;
        }

        private void enrollment_OnStartEnroll(DPCtlUruNet.EnrollmentControl enrollmentControl, Constants.ResultCode result, int fingerPosition)
        {
            if (enrollmentControl.Reader != null)
            {
                ShowMessage("OnStartEnroll:  " + enrollmentControl.Reader.Description.Name + ", finger " + fingerPosition);
            }
            else
            {
                ShowMessage("OnStartEnroll:  No Reader Connected, finger " + fingerPosition);
            }


            btnCancel.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show("Are you sure you want to cancel this enrollment?", "Are You Sure?", buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _enrollmentControl.Cancel();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // saveandclose sc = new saveandclose(this.closeform);
            // sc();
            if (SCevent != null)
            {
                SCevent();
            }
            this.Close();
        }
       
        private void frmEnrollment_Closed(object sender, EventArgs e)
        {
            _enrollmentControl.Cancel();
        }

        private void ShowMessage(string message)
        {
            txtMessage.Text += message + "\r\n\r\n";
            txtMessage.SelectionStart = txtMessage.TextLength;
            txtMessage.ScrollToCaret();
        }

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

        private void EnrollmentControl_Load(object sender, EventArgs e)
        {
            if (_enrollmentControl == null)
            {
                _enrollmentControl = new DPCtlUruNet.EnrollmentControl(Currentinstance.CurrentReader, Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
                _enrollmentControl.BackColor = System.Drawing.SystemColors.Window;
                _enrollmentControl.Location = new System.Drawing.Point(3, 3);
                _enrollmentControl.Name = "ctlEnrollmentControl";
                _enrollmentControl.Size = new System.Drawing.Size(482, 346);
                _enrollmentControl.TabIndex = 0;
                _enrollmentControl.OnCancel += new DPCtlUruNet.EnrollmentControl.CancelEnrollment(this.enrollment_OnCancel);
                _enrollmentControl.OnCaptured += new DPCtlUruNet.EnrollmentControl.FingerprintCaptured(this.enrollment_OnCaptured);
                _enrollmentControl.OnDelete += new DPCtlUruNet.EnrollmentControl.DeleteEnrollment(this.enrollment_OnDelete);
                _enrollmentControl.OnEnroll += new DPCtlUruNet.EnrollmentControl.FinishEnrollment(this.enrollment_OnEnroll);
                _enrollmentControl.OnStartEnroll += new DPCtlUruNet.EnrollmentControl.StartEnrollment(this.enrollment_OnStartEnroll);
            }
            else
            {
                _enrollmentControl.Reader = Currentinstance.CurrentReader;
            }

            this.Controls.Add(_enrollmentControl);
        }
    }
}
