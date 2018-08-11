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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.createAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testAuthenticatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testReplayShieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loanAndInvestmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ManageFinances = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ControlHost
            // 
            this.ControlHost.Location = new System.Drawing.Point(63, 74);
            this.ControlHost.Name = "ControlHost";
            this.ControlHost.Size = new System.Drawing.Size(460, 450);
            this.ControlHost.TabIndex = 0;
            // 
            // Header
            // 
            this.Header.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Header.AutoSize = true;
            this.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Header.Location = new System.Drawing.Point(225, 48);
            this.Header.Name = "Header";
            this.Header.Padding = new System.Windows.Forms.Padding(3);
            this.Header.Size = new System.Drawing.Size(158, 23);
            this.Header.TabIndex = 1;
            this.Header.Text = "Bank Expert System";
            this.Header.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Menu
            // 
            this.Menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createAccountToolStripMenuItem,
            this.testAuthenticatorToolStripMenuItem,
            this.testReplayShieldToolStripMenuItem,
            this.loanAndInvestmentsToolStripMenuItem,
            this.ManageFinances});
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(61, 20);
            this.Menu.Text = "Options";
            // 
            // createAccountToolStripMenuItem
            // 
            this.createAccountToolStripMenuItem.Name = "createAccountToolStripMenuItem";
            this.createAccountToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.createAccountToolStripMenuItem.Text = "Create account";
            this.createAccountToolStripMenuItem.Click += new System.EventHandler(this.RegisterUserTextbox_Click);
            // 
            // testAuthenticatorToolStripMenuItem
            // 
            this.testAuthenticatorToolStripMenuItem.Name = "testAuthenticatorToolStripMenuItem";
            this.testAuthenticatorToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.testAuthenticatorToolStripMenuItem.Text = "Test authenticator";
            this.testAuthenticatorToolStripMenuItem.Click += new System.EventHandler(this.AuthenticateUserTextbox_Click);
            // 
            // testReplayShieldToolStripMenuItem
            // 
            this.testReplayShieldToolStripMenuItem.Name = "testReplayShieldToolStripMenuItem";
            this.testReplayShieldToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.testReplayShieldToolStripMenuItem.Text = "Test replay shield";
            this.testReplayShieldToolStripMenuItem.Click += new System.EventHandler(this.TestReplayAttackTextbox_Click);
            // 
            // loanAndInvestmentsToolStripMenuItem
            // 
            this.loanAndInvestmentsToolStripMenuItem.Name = "loanAndInvestmentsToolStripMenuItem";
            this.loanAndInvestmentsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
            this.loanAndInvestmentsToolStripMenuItem.Text = "Loan and investments";
            this.loanAndInvestmentsToolStripMenuItem.Click += new System.EventHandler(this.LoanAndInvestmentsTextbox_Click);
            // 
            // ManageFinances
            // 
            this.ManageFinances.Name = "ManageFinances";
            this.ManageFinances.Size = new System.Drawing.Size(190, 22);
            this.ManageFinances.Text = "Manage Finances";
            this.ManageFinances.Click += new System.EventHandler(this.ManageFinancesMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(584, 536);
            this.Controls.Add(this.Header);
            this.Controls.Add(this.ControlHost);
            this.Controls.Add(this.menuStrip1);
            this.Location = new System.Drawing.Point(800, 0);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 575);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Staaworks BankExpert";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel ControlHost;
        private System.Windows.Forms.Label Header;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Menu;
        private System.Windows.Forms.ToolStripMenuItem createAccountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testAuthenticatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testReplayShieldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loanAndInvestmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ManageFinances;
    }
}

