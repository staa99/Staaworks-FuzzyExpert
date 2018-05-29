using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms
{
    class QuestionAndAnswerViewDataHolder : IViewDataHolder
    {
        public Control ViewDataControl
        {
            get => QuestionView;
            set => QuestionView = (value as QuestionView);
        }

        public Dictionary<Question, Option> Questions { get; set; }
        public QuestionView QuestionView { get; set; }
        public Context CurrentContext { get; set; }
        private IQuestionOptionReciever Reciever { get; }

        public QuestionAndAnswerViewDataHolder(IQuestionOptionReciever reciever)
        {
            ViewDataControl = QuestionView;
            Reciever = reciever;
        }


        public void PopulateControl (params object[] parameters)
        {
            var name = parameters.Any() ? parameters[0].ToString() : "RootContext";
            var dotIndex = name.LastIndexOfAny(new char[]{'.'});
            var contextName = parameters[0].ToString().Substring(0, dotIndex > 0? dotIndex: name.Length);
            QuestionView = new QuestionView(Reciever);
            CurrentContext = QuestionRepository.LoadContext(contextName);
            UpdateQuestionView();
        }


        public void UpdateQuestionView()
        {
            Questions = CurrentContext.Questions.ToDictionary<Question, Question, Option>(q => q, q => null);
            LoadNextQuestion();
            Reciever.AddControl(QuestionView);
        }






        public void LoadNextQuestion ()
        {
            if (Questions.Any (p => p.Value == null && p.Key.Source == "user"))
            {
                QuestionView.SetCurrentQuestion (Questions.First (p => p.Value == null && p.Key.Source == "user").Key, Questions.Count (p => p.Value == null) <= 1);
            }
        }
    }
}
