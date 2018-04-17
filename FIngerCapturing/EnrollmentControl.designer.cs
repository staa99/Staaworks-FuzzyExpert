using DPCtlUruNet;

//! @cond
namespace FingerCapturing
{
    partial class EnrollmentControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pbFingerprint = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbFingerprint)).BeginInit();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtMessage.Location = new System.Drawing.Point(614, 4);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(344, 215);
            this.txtMessage.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(592, 250);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(129, 33);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(592, 291);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(129, 35);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Save and Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pbFingerprint
            // 
            this.pbFingerprint.Location = new System.Drawing.Point(729, 228);
            this.pbFingerprint.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbFingerprint.Name = "pbFingerprint";
            this.pbFingerprint.Size = new System.Drawing.Size(230, 209);
            this.pbFingerprint.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFingerprint.TabIndex = 0;
            this.pbFingerprint.TabStop = false;
            // 
            // EnrollmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(965, 446);
            this.Controls.Add(this.pbFingerprint);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtMessage);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(983, 493);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(983, 493);
            this.Name = "EnrollmentControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enrollment";
            this.Closed += new System.EventHandler(this.frmEnrollment_Closed);
            this.Load += new System.EventHandler(this.EnrollmentControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbFingerprint)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pbFingerprint;
    }
}
//! @endcond