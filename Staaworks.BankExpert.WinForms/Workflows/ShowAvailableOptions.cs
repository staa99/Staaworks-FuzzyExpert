using System;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms.Workflows
{
    public partial class ShowAvailableOptions : Form
    {
        public ShowAvailableOptions ()
        {
            InitializeComponent();
            DialogResult = DialogResult.No;
        }

        private void BtnLoanInvestments_Click (object sender, EventArgs e) =>
            DialogResult = DialogResult.Yes;
    }
}
