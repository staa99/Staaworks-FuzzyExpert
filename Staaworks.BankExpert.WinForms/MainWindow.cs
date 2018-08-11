using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;
using Staaworks.BankExpert.WinForms.Registration;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using Staaworks.BankExpert.WinForms.Authentication;
using Staaworks.BankExpert.WinForms.Workflows;

namespace Staaworks.BankExpert.WinForms
{
    partial class MainWindow : Form, IQuestionOptionReciever
    {
        public IDictionary<string, Action> Tasks { get; }
        public Dictionary<string, IViewDataHolder> Data { get; }
        public bool LastAuthenticationAttempt { get; private set; }

        public MainWindow ()
        {
            Data = new Dictionary<string, IViewDataHolder>
            {
                ["QuestionAndAnswer"] = new QuestionAndAnswerViewDataHolder(this),
                ["Registration"] = new UserCreatorData(this),
                ["Authentication"] = new UserAuthenticator(this)
            };

            Tasks = new Dictionary<string, Action>
            {
                ["QuestionAndAnswer"] = InitializeQuestionView,
                ["Registration"] = LoadRegistrationForm
            };

            InitializeComponent();
        }



        public void InitializeQuestionView ()
        {
            ClearControls();
            (Data["QuestionAndAnswer"] as QuestionAndAnswerViewDataHolder).PopulateControl("LoanInvestments");
        }


        public void LoadRegistrationForm ()
        {
            ClearControls();
            (Data["Registration"] as UserCreatorData).PopulateControl();
        }


        public void LoadAuthenticationForm (Action<bool> complete, params UserAuthenticationScheme[] schemes)
        {
            ClearControls();
            var authenticator = Data["Authentication"] as UserAuthenticator;
            if (!schemes.Any())
            {
                // show dialog for selecting type
                var authSchemeSelector = new AuthSchemeDialog();
                authSchemeSelector.ShowDialog();
                schemes = authSchemeSelector.Schemes;
            }

            authenticator.PopulateControl(schemes);
            authenticator.OnComplete = (success) =>
            {
                LastAuthenticationAttempt = success;
                complete(success);
            };
        }


        public void LoadTask (string task) => Tasks[task].Invoke();


        public void SetQuestionValue (Question question, Option option)
        {
            var holder = Data["QuestionAndAnswer"] as QuestionAndAnswerViewDataHolder;

            void treatLastQuestion ()
            {
                // Last Question treated
                var qs = holder.Questions.Where (_q => _q.Key.Source != "user").Select(_q => _q.Key).ToArray();

                foreach (var q in qs)
                {
                    holder.Questions[q] = new Option
                    {
                        Value = float.Parse(Shared.Cache.SystemGeneratedSourceCache.Data[q.Source.Trim()].ToString())
                    };
                }

                var (text, context) = QuestionRepository.TreatUserInput(holder.Questions);
                if (text != null)
                {
                    holder.QuestionView.DisplayFinalResult(text);
                }
                else if (context != null)
                {
                    holder.CurrentContext = context;
                    holder.UpdateQuestionView();
                }
            }


            if (question == null || option == null)
            {
                treatLastQuestion();
                return;
            }

            holder.Questions[question] = option;
            if (holder.Questions.Any(p => p.Value == null && p.Key.Source == "user"))
            {
                holder.LoadNextQuestion();
            }
            else
            {
                treatLastQuestion();
            }
        }


        public void AddControl (Control control) =>
            ControlHost.Controls.Add(control);

        public void RemoveControl (Control control) =>
            ControlHost.Controls.Remove(control);

        public void ClearControls () =>
            ControlHost.Controls.Clear();

        private void RegisterUserTextbox_Click (object sender, EventArgs e) =>
            LoadTask("Registration");

        private void AuthenticateUserTextbox_Click (object sender, EventArgs e) =>
            LoadAuthenticationForm(success => Console.WriteLine("Auth result: {0}", success));

        // replace this
        private void TestReplayAttackTextbox_Click (object sender, EventArgs e) =>
            MessageBox.Show("Authentication failed, replay attack detected");

        private void LoanAndInvestmentsTextbox_Click (object sender, EventArgs e) =>
            LoanInvestments.Perform(this);

        private void ManageFinancesMenuItem_Click (object sender, EventArgs e) => LoadAuthenticationForm(success =>
        {
            if (success)
            {
                var user = (Data["Authentication"] as UserAuthenticator).User;
                var financialDataForm = new FinancialDataForm(user, FinancialDataFormMode.edit);
                financialDataForm.ShowDialog();
            }
            else
            {
                Console.Beep();
            }
        });
    }
}