using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;
using Staaworks.BankExpert.WinForms.Registration;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;

namespace Staaworks.BankExpert.WinForms
{
    public partial class MainWindow : Form, IQuestionOptionReciever
    {
        public IDictionary<string, Action> Tasks { get; }
        public Dictionary<string, IViewDataHolder> Data { get; }

        public MainWindow()
        {
            Data = new Dictionary<string, IViewDataHolder>
            {
                ["QuestionAndAnswer"] = new QuestionAndAnswerViewDataHolder(this),
                ["Registration"] = new UserCreatorData(this)
            };

            Tasks = new Dictionary<string, Action>
            {
                ["QuestionAndAnswer"] = InitializeQuestionView,
                ["Registration"] = LoadRegistrationForm
            };

            InitializeComponent();
            LoadTask("QuestionAndAnswer");
        }



        public void InitializeQuestionView()
        {
            ClearControls();
            (Data["QuestionAndAnswer"] as QuestionAndAnswerViewDataHolder).PopulateControl();
        }


        public void LoadRegistrationForm()
        {
            ClearControls();
            (Data["Registration"] as UserCreatorData).PopulateControl();
        }

        public void LoadTask(string task)
        {
            Tasks[task].Invoke();
        }

        public void SetQuestionValue(Question question, Option option)
        {
            var holder = Data["QuestionAndAnswer"] as QuestionAndAnswerViewDataHolder;
            holder.Questions[question] = option;
            if (holder.Questions.Any(p => p.Value == null))
            {
                holder.LoadNextQuestion();
            }
            else
            {
                // Last Question treated
                var response = QuestionRepository.TreatUserInput(holder.Questions);
                if (response.text != null)
                {
                    holder.QuestionView.DisplayFinalResult(response.text);
                }
                else if (response.context != null)
                {
                    holder.CurrentContext = response.context;
                    holder.UpdateQuestionView();
                }
            }
        }

        public void AddControl(Control control)
        {
            ControlHost.Controls.Add(control);
        }

        public void RemoveControl(Control control)
        {
            ControlHost.Controls.Remove(control);
        }

        public void ClearControls()
        {
            ControlHost.Controls.Clear();
        }
    }
}
