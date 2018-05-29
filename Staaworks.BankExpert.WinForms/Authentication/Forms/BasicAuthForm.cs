using Staaworks.BankExpert.Shared.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms.Authentication.Forms
{
    partial class BasicAuthForm : Form
    {
        public bool Successful { get; private set; }
        public User User { get; }
        public BasicAuthForm (User user)
        {
            InitializeComponent();

            User = user;
            EmailTextBox.Text = user.Email;
        }

        private void SignInButton_Click (object sender, EventArgs e)
        {
            Successful = User.PasswordHash == BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text + User.HashSalt)));
            Close();
        }
    }
}
