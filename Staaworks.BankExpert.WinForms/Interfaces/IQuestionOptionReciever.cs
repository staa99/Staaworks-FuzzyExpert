using Staaworks.BankExpert.Shared.Models;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms.Interfaces
{
    interface IDataReciever
    {
        void AddControl(Control control);
        void RemoveControl(Control control);
        void ClearControls();

        void LoadTask(string task);
    }


    interface IQuestionOptionReciever : IDataReciever
    {
        void SetQuestionValue(Question question, Option option);
    }
}
