namespace Staaworks.BankExpert.WinForms.Authentication
{
    partial class AuthSchemeDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
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
        private void InitializeComponent ()
        {
            this.SchemeInputTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SchemeInputTextBox
            // 
            this.SchemeInputTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SchemeInputTextBox.Location = new System.Drawing.Point(12, 34);
            this.SchemeInputTextBox.Name = "SchemeInputTextBox";
            this.SchemeInputTextBox.Size = new System.Drawing.Size(150, 20);
            this.SchemeInputTextBox.TabIndex = 0;
            this.SchemeInputTextBox.Text = "pass->fing->face";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(12, 60);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(260, 189);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Enter the code for the auth scheme selection and ordering.\r\nExample: pass->fing->" +
    "face\r\n\r\nLegend\r\npass:- Basic authentication\r\nfing:- Fingerprint authentication\r\n" +
    "face:- Facial authentication";
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(197, 34);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitButton.TabIndex = 2;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // AuthSchemeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.SchemeInputTextBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 300);
            this.MinimizeBox = false;
            this.Name = "AuthSchemeDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter schemes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SchemeInputTextBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SubmitButton;
    }
}