using Newtonsoft.Json.Linq;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Staaworks.BankExpert.Shared.Cache;

namespace Staaworks.BankExpert.WinForms.Registration
{
    public partial class FinancialDataForm : Form
    {
        private User User { get; }
        private Dictionary<string, object> Data { get; set; }
        public FinancialDataFormMode Mode { get; }

        public FinancialDataForm (User user, FinancialDataFormMode mode)
        {
            InitializeComponent();
            User = user ?? throw new ArgumentNullException(nameof(user));
            Mode = mode;
            Data = SystemGeneratedSourceCache.Data;
            LoadColumnNames();
            DiaplayColumnNames();
            if (mode == FinancialDataFormMode.view)
            {
                SaveButton.Text = "Continue";
            }
        }

        private void LoadColumnNames ()
        {
            var file = $"{Environment.CurrentDirectory}\\Config\\Input\\LoanInvestment\\{User.Email}\\input.json";
            if (!File.Exists(file))
            {
                file = Environment.CurrentDirectory + "\\Config\\Input\\LoanInvestment\\input.json";
            }
            var content = File.ReadAllText(file);
            var jtokens = JObject.Parse(content);

            foreach (var jtoken in jtokens)
            {
                // set default values from central
                Data[jtoken.Key] = jtoken.Value.ToString();
            }
        }


        private void DiaplayColumnNames ()
        {
            var row = 0;
            Table.RowCount = 0;
            Table.ColumnCount = 2;

            foreach (var key in Data.Keys)
            {
                var labelFont = new Font("Lucida Sans", 9, FontStyle.Bold, GraphicsUnit.Point);
                var textboxFont = new Font(labelFont, FontStyle.Regular);

                var label = new Label
                {
                    Text = key,
                    Font = labelFont,
                    Name = $"label{row}",
                    Anchor = AnchorStyles.Left,
                    Margin = new Padding(3, 5, 3, 5),
                    AutoSize = true,
                    Width = 150
                };

                var textbox = new TextBox
                {
                    Font = textboxFont,
                    Name = $"textbox{row}",
                    Anchor = AnchorStyles.Left,
                    Margin = new Padding(3, 5, 3, 5),
                    AutoSize = true,
                    Width = 150,
                    Enabled = Mode == FinancialDataFormMode.edit,
                    Text = Data[key].ToString()
                };
                

                Table.RowCount++;
                Table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                Table.Controls.Add(label, 0, row);
                Table.Controls.Add(textbox, 1, row);
                row++;
            }
        }


        private void SaveInputData ()
        {
            if (Mode == FinancialDataFormMode.view)
            {
                Close();
                return;
            }

            var unfilledExists = false;
            var data = new Dictionary<string, string>();

            foreach (var c in Table.Controls)
            {
                if (c is Label)
                {
                    var label = c as Label;

                    var row = int.Parse(label.Name.Substring(5));
                    if (data.ContainsKey(row.ToString()))
                    {
                        data[$"{row.ToString()}:{label.Text}"] = data[row.ToString()];
                        data.Remove(row.ToString());
                    }
                    else
                    {
                        data[$"{row.ToString()}:{label.Text}"] = null;
                    }
                }

                else if (c is TextBox)
                {
                    var tb = c as TextBox;
                    unfilledExists = tb.Enabled && tb.Text.Trim().Length == 0;

                    if (unfilledExists)
                    {
                        data.Clear();
                        Console.Beep();
                        return;
                    }

                    var row = int.Parse(tb.Name.Substring(7));
                    var fk = data.FirstOrDefault(p => p.Key.Split(':')[0] == row.ToString());

                    if (fk.Key == null)
                    {
                        data[row.ToString()] = tb.Text;
                    }
                    else
                    {
                        data[fk.Key] = tb.Text;
                    }
                }
            }

            var allKeys = data.Keys.ToArray();
            foreach (var key in allKeys)
            {
                data[key.Substring(key.IndexOf(':') + 1)] = double.Parse(data[key]).ToString();
                Data[key.Substring(key.IndexOf(':') + 1)] = double.Parse(data[key]);
            }

            var keys = data.Keys.Where(p => p.Contains(":")).ToArray();
            foreach(var k in keys)
            {
                data.Remove(k);
            }

            Directory.CreateDirectory($"{Environment.CurrentDirectory}\\Config\\Input\\LoanInvestment\\{User.Email}\\");

            var file = $"{Environment.CurrentDirectory}\\Config\\Input\\LoanInvestment\\{User.Email}\\input.json";
            var content = JObject.FromObject(data);
            var json = content.ToString();
            File.WriteAllText(file, json);

            Close();
        }
        


        private void SaveButton_Click (object sender, EventArgs e) =>
            SaveInputData();
    }


    public enum FinancialDataFormMode
    {
        view,
        edit
    }
}
