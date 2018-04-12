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
            this.SuspendLayout();
            // 
            // ControlHost
            // 
            this.ControlHost.Location = new System.Drawing.Point(12, 73);
            this.ControlHost.Name = "ControlHost";
            this.ControlHost.Size = new System.Drawing.Size(460, 450);
            this.ControlHost.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 536);
            this.Controls.Add(this.ControlHost);
            this.Location = new System.Drawing.Point(800, 0);
            this.Name = "MainWindow";
            this.Text = "Staaworks BankExpert";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ControlHost;
    }
}

