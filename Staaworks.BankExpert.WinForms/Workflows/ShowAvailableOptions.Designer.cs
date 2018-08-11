namespace Staaworks.BankExpert.WinForms.Workflows
{
    partial class ShowAvailableOptions
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
            this.BtnLoanInvestments = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnLoanInvestments
            // 
            this.BtnLoanInvestments.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnLoanInvestments.Location = new System.Drawing.Point(88, 93);
            this.BtnLoanInvestments.Name = "BtnLoanInvestments";
            this.BtnLoanInvestments.Size = new System.Drawing.Size(200, 30);
            this.BtnLoanInvestments.TabIndex = 0;
            this.BtnLoanInvestments.Text = "Loan and Investments";
            this.BtnLoanInvestments.UseVisualStyleBackColor = true;
            this.BtnLoanInvestments.Click += new System.EventHandler(this.BtnLoanInvestments_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(88, 148);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(200, 30);
            this.button2.TabIndex = 1;
            this.button2.Text = "Make Enquiries";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ShowAvailableOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.BtnLoanInvestments);
            this.MinimizeBox = false;
            this.Name = "ShowAvailableOptions";
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnLoanInvestments;
        private System.Windows.Forms.Button button2;
    }
}