using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms
{
    public partial class QuestionView : UserControl
    {
        public Question CurrentQuestion { get; set; }
        private List<RadioButton> optionGroup = new List<RadioButton>();
        public Option CurrentValue { get; private set; }
        private IQuestionOptionReciever Reciever { get; }


        private bool valueValid = false;



        public QuestionView(IQuestionOptionReciever reciever)
        {
            InitializeComponent();
            OptionsContainer.HorizontalScroll.Enabled = false;
            OptionsContainer.AutoScroll = false;
            OptionsContainer.FlowDirection = FlowDirection.TopDown;
            OptionsContainer.WrapContents = true;

            Reciever = reciever;
        }


        public void SetCurrentQuestion(Question question, bool isLastQuestion)
        {
            CurrentQuestion = question;
            UpdateQuestionText();
            UpdateOptions();
            NextOrSubmitButton.Text = isLastQuestion ? "Submit" : "Next";
            CurrentValue = null;
            valueValid = false;
        }

        private void UpdateQuestionText()
        {
            QuestionTextLabel.Text = CurrentQuestion.UserText;
        }


        private void UpdateOptions()
        {
            OptionsContainer.Controls.Clear();
            Option freetextOption = CurrentQuestion.Options.SingleOrDefault(o => o.Type == OptionType.freetext);

            foreach (var option in CurrentQuestion.Options.Where(o => o.Type != OptionType.freetext))
            {
                var radioButton = new RadioButton
                {
                    Text = option.UserText,
                    Name = option.Value.ToString()
                };


                radioButton.CheckedChanged += ValueChanged;
                optionGroup.Add(radioButton);
                OptionsContainer.Controls.Add(radioButton);
            }

            if (freetextOption != null)
            {
                OptionsContainer.Controls.Add(new Label
                {
                    Name = "FreeTextOptionLabel",
                    Text = freetextOption.UserText
                });

                var textbox = new TextBox
                {
                    Width = 100,
                    Height = 50,
                    BackColor = Color.WhiteSmoke
                };

                textbox.TextChanged += ValueChanged;
                OptionsContainer.Controls.Add(textbox);
            }
        }


        public void DisplayFinalResult(string result)
        {
            QuestionTextLabel.Text = result;
            OptionsContainer.Controls.Clear();
            NextOrSubmitButton.Click -= Submit;
            NextOrSubmitButton.Click += (sender, e) =>
            {
                QuestionTextLabel.Text = "Finished";
            };
        }


        private void ValueChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                var radioButton = sender as RadioButton;
                if (radioButton.Checked)
                {
                    valueValid = true;
                    CurrentValue = CurrentQuestion.Options.Where(o => o.UserText == radioButton.Text).Single();
                }
                else
                {
                    valueValid = false;
                }
            }
            else if (sender is TextBox)
            {
                var textbox = sender as TextBox;
                if (float.TryParse(textbox.Text, out float value))
                {
                    CurrentValue = CurrentQuestion.Options.Where(o =>
                    {
                        return o.UserText == OptionsContainer.Controls.Find("FreeTextOptionLabel", true).Single().Text;
                    }).Single();
                    CurrentValue.Value = value;
                    valueValid = true;
                }
                else
                {
                    valueValid = false;
                }
            }
        }

        private void Submit(object sender, EventArgs e)
        {
            if (valueValid)
            {
                Reciever.SetQuestionValue(CurrentQuestion, CurrentValue);
            }
            else
            {
                Console.Beep();
            }
        }
    }
}