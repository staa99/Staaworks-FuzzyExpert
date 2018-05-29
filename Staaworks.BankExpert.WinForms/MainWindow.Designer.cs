namespace Staaworks.BankExpert.WinForms
{
    partial class MainWindow
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
            this.ControlHost = new System.Windows.Forms.Panel();
            this.Header = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ControlHost
            // 
            this.ControlHost.Location = new System.Drawing.Point(12, 73);
            this.ControlHost.Name = "ControlHost";
            this.ControlHost.Size = new System.Drawing.Size(460, 450);
            this.ControlHost.TabIndex = 0;
            // 
            // Header
            // 
            this.Header.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Header.AutoSize = true;
            this.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Header.Location = new System.Drawing.Point(77, 30);
            this.Header.Name = "Header";
            this.Header.Padding = new System.Windows.Forms.Padding(3);
            this.Header.Size = new System.Drawing.Size(327, 23);
            this.Header.TabIndex = 1;
            this.Header.Text = "Fuzzy-based expert system + Authenticator";
            this.Header.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(484, 536);
            this.Controls.Add(this.Header);
            this.Controls.Add(this.ControlHost);
            this.Location = new System.Drawing.Point(800, 0);
            this.MaximumSize = new System.Drawing.Size(500, 575);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Staaworks BankExpert";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel ControlHost;
        private System.Windows.Forms.Label Header;
    }
}

