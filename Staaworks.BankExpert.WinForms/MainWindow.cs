using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms
{
    public partial class MainWindow : Form, IQuestionOptionReciever
    {
        private const string ROOT_CONTEXT_NAME = "RootContext";
        private Dictionary<Question, Option> Questions { get; set; }
        private QuestionView QuestionView { get; set; }
        private Context CurrentContext { get; set; }
        

        public MainWindow()
        {
            InitializeComponent();
            InitializeQuestionView();
        }


        public void InitializeQuestionView()
        {
            QuestionView = new QuestionView(this);
            CurrentContext = QuestionRepository.LoadContext(ROOT_CONTEXT_NAME);
            UpdateQuestionView();
        }


        private void UpdateQuestionView()
        {
            Questions = CurrentContext.Questions.ToDictionary<Question, Question, Option>(q => q, q => null);
            LoadNextQuestion();
            ControlHost.Controls.Add(QuestionView);
        }


        public void SetQuestionValue(Question question, Option option)
        {
            Questions[question] = option;
            if (Questions.Any(p => p.Value == null))
            {
                LoadNextQuestion();
            }
            else
            {
                // Last Question treated
                var response = QuestionRepository.TreatUserInput(Questions);
                if (response.text != null)
                {
                    QuestionView.DisplayFinalResult(response.text);
                }
                else if (response.context != null)
                {
                    CurrentContext = response.context;
                    UpdateQuestionView();
                }
            }
        }


        private void LoadNextQuestion()
        {
            QuestionView.SetCurrentQuestion(Questions.First(p => p.Value == null).Key, Questions.Count(p => p.Value == null) <= 1);
        }
    }
}
