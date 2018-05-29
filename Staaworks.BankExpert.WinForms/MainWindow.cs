using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;
using Staaworks.BankExpert.WinForms.Registration;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using Staaworks.BankExpert.WinForms.Authentication;

namespace Staaworks.BankExpert.WinForms
{
    partial class MainWindow : Form, IQuestionOptionReciever
    {
        public IDictionary<string, Action> Tasks { get; }
        public Dictionary<string, IViewDataHolder> Data { get; }


        public MainWindow()
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
            LoadTask("Registration");
            //LoadAuthenticationForm(UserAuthenticationScheme.BasicAuthentication, UserAuthenticationScheme.FaceAuthentication);
        }



        public void InitializeQuestionView()
        {
            ClearControls();
            (Data["QuestionAndAnswer"] as QuestionAndAnswerViewDataHolder).PopulateControl("ATM_Dispense_Error");
        }


        public void LoadRegistrationForm()
        {
            ClearControls();
            (Data["Registration"] as UserCreatorData).PopulateControl();
        }


        public void LoadAuthenticationForm(params UserAuthenticationScheme[] schemes)
        {
            ClearControls();
            var authenticator = Data["Authentication"] as UserAuthenticator;
            authenticator.PopulateControl(schemes);
            authenticator.OnComplete = (success) =>
            {
                MessageBox.Show(success.ToString());
            };
        }


        public void LoadTask (string task) => Tasks [task].Invoke ();


        public void SetQuestionValue(Question question, Option option)
        {
            var holder = Data["QuestionAndAnswer"] as QuestionAndAnswerViewDataHolder;
            holder.Questions[question] = option;
            if (holder.Questions.Any(p => p.Value == null && p.Key.Source == "user"))
            {
                holder.LoadNextQuestion();
            }
            else
            {
                // Last Question treated
                var qs = holder.Questions.Where (_q => _q.Key.Source != "user").Select(_q => _q.Key).ToArray();

                foreach (var q in qs)
                {
                    holder.Questions [q] = new Option
                    {
                        Value = float.Parse (Shared.Cache.SystemGeneratedSourceCache.Data [q.Source.Trim ()].ToString ())
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
        }


        public void AddControl (Control control) => ControlHost.Controls.Add (control);

        public void RemoveControl (Control control) => ControlHost.Controls.Remove (control);

        public void ClearControls () => ControlHost.Controls.Clear ();
    }
}