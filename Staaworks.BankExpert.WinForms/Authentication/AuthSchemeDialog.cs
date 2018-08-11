using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms.Authentication
{
    public partial class AuthSchemeDialog : Form
    {
        internal UserAuthenticationScheme[] Schemes { get; private set; }
        public AuthSchemeDialog ()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click (object sender, EventArgs e)
        {
            var code = SchemeInputTextBox.Text.Trim();
            const string connector = "->";
            if (code.StartsWith(connector) || code.EndsWith(connector))
            {
                Console.Beep();
                return;
            }

            var keys = code.Split(new[] {connector}, StringSplitOptions.RemoveEmptyEntries).Distinct();
            Schemes = keys.Select(key =>
            {
                switch(key)
                {
                    case "face":
                        return UserAuthenticationScheme.FaceAuthentication;
                    case "fing":
                        return UserAuthenticationScheme.FingerprintAuthentication;
                    case "pass":
                    default:
                        return UserAuthenticationScheme.BasicAuthentication;
                }
            }).ToArray();
            Close();
        }
    }
}
