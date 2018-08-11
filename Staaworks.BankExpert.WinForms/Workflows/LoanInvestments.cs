using Newtonsoft.Json.Linq;
using Staaworks.BankExpert.Shared.Cache;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Authentication;
using Staaworks.BankExpert.WinForms.Registration;
using System;
using System.IO;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms.Workflows
{
    static class LoanInvestments
    {
        private static bool proceedToLoanAndInvestment = false;
        private static MainWindow Main { get; set; }

        public static void Perform(MainWindow main)
        {
            // read data from file into global store                        todo====================================
            // do basic authentication                                      done
            // display available investments and loans                      todo====================================
            // prompt for biometric                                         done
            // if successful: show liability and asset position accross financial institution
            // use rules defined for spending pattern vs financial position to suggest investment or borrow
            // investment branch to new rule based on balance and spending pattern and investment benefits
            // request amount
            // if balance >= amount
            // display prompt to confirm amount
            // end of investment
            // loan: check if user not blacklisted and has good credit report
            // calculate lodgement for past three months, if above 100 proceeds else end
            // system analyses spending pattern and suggests loan amount and tenor and allows loan amount to be entered by user
            // if monthly repayment < 0.3 * lodgement, proceed else terminate

            // expected input
            // liability
            // asset
            // spending pattern
            // financial position
            // balance
            // userBlacklisted
            // lodgement

            
            main.LoadAuthenticationForm(ContinueFromBasicAuthentication, UserAuthenticationScheme.BasicAuthentication);
            

            Main = main;
        }


        private static void ContinueFromBasicAuthentication(bool success)
        {
            // successful Authentication
            if (success)
            {
                ShowAvailableInvestmentsAndLoans();

                if (proceedToLoanAndInvestment)
                {
                    Main.LoadAuthenticationForm(ContinueFromBiometricAuthentication, UserAuthenticationScheme.FingerprintAuthentication);
                }
                else
                {
                    Terminate("Choose enquiry", Main);
                }
            }
            else
            {
                Terminate("Basic authentication failed", Main);
            }
        }

        private static void ContinueFromBiometricAuthentication(bool success)
        {
            if (success)
            {
                var user = (Main.Data["Authentication"] as UserAuthenticator).User;
                LoadFileData(user);
                ShowFinPosition(user);
                Main.LoadTask("QuestionAndAnswer");
            }
            else
            {
                Terminate("Choose enquiry (Biometric failed)", Main);
            }
        }


        private static void LoadFileData(User user)
        {
            var email = user.Email;
            var file = $"{Environment.CurrentDirectory}\\Config\\Input\\LoanInvestment\\{user.Email}\\input.json";
            if (!File.Exists(file))
            {
                file = Environment.CurrentDirectory + "\\Config\\Input\\LoanInvestment\\input.json";
            }
            
            var content = File.ReadAllText(file);
            var jtokens = JObject.Parse(content);
            
            foreach(var jtoken in jtokens)
            {
                SystemGeneratedSourceCache.Data[jtoken.Key] = jtoken.Value.ToString();
            }
        }


        private static void ShowAvailableInvestmentsAndLoans()
        {
            var result = new ShowAvailableOptions().ShowDialog();
            switch(result)
            {
                case DialogResult.Yes:
                    proceedToLoanAndInvestment = true;
                    break;
            }
        }

        private static void Terminate (string reason, MainWindow main) =>
            MessageBox.Show($"Flow terminated\nCause: {reason}");
        //main.Close();


        private static void ShowFinPosition(User user)
        {
            var finDataForm = new FinancialDataForm(user, FinancialDataFormMode.view);
            finDataForm.ShowDialog();
        }
    }
}