using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms
{
    public interface IViewDataHolder
    {
        Control ViewDataControl { get; set; }
        void PopulateControl(params object[] parameters);
    }
}
