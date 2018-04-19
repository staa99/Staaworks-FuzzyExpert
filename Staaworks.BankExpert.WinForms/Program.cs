using Staaworks.BankExpert.Shared.Cache;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Staaworks.BankExpert.WinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SystemGeneratedSourceCache.Data = new Dictionary<string, object>
            {
                ["user.balanceAfter"] = 300000
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}