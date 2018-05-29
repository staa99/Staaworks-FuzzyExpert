namespace Staaworks.BankExpert.WinForms.Authentication
{
    partial class UserAuthenticator
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.EmailTextBox = new System.Windows.Forms.TextBox();
            this.EmailLabel = new System.Windows.Forms.Label();
            this.ContinueAuthButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.Location = new System.Drawing.Point(195, 32);
            this.EmailTextBox.MinimumSize = new System.Drawing.Size(150, 30);
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Size = new System.Drawing.Size(150, 30);
            this.EmailTextBox.TabIndex = 12;
            // 
            // EmailLabel
            // 
            this.EmailLabel.AutoSize = true;
            this.EmailLabel.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailLabel.Location = new System.Drawing.Point(12, 32);
            this.EmailLabel.Margin = new System.Windows.Forms.Padding(3);
            this.EmailLabel.MaximumSize = new System.Drawing.Size(150, 30);
            this.EmailLabel.MinimumSize = new System.Drawing.Size(150, 30);
            this.EmailLabel.Name = "EmailLabel";
            this.EmailLabel.Padding = new System.Windows.Forms.Padding(3);
            this.EmailLabel.Size = new System.Drawing.Size(150, 30);
            this.EmailLabel.TabIndex = 11;
            this.EmailLabel.Text = "Email";
            this.EmailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ContinueAuthButton
            // 
            this.ContinueAuthButton.Location = new System.Drawing.Point(144, 96);
            this.ContinueAuthButton.Name = "ContinueAuthButton";
            this.ContinueAuthButton.Size = new System.Drawing.Size(75, 23);
            this.ContinueAuthButton.TabIndex = 13;
            this.ContinueAuthButton.Text = "Continue";
            this.ContinueAuthButton.UseVisualStyleBackColor = true;
            this.ContinueAuthButton.Click += new System.EventHandler(this.ContinueAuthButton_Click);
            // 
            // UserAuthenticator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ContinueAuthButton);
            this.Controls.Add(this.EmailTextBox);
            this.Controls.Add(this.EmailLabel);
            this.Name = "UserAuthenticator";
            this.Size = new System.Drawing.Size(377, 136);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox EmailTextBox;
        private System.Windows.Forms.Label EmailLabel;
        private System.Windows.Forms.Button ContinueAuthButton;
    }
}
