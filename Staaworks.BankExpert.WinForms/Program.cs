using Staaworks.BankExpert.Shared.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            LoadAssemblies();

            SystemGeneratedSourceCache.Data = new Dictionary<string, object>
            {
                ["user.balanceAfter"] = 300000
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }


        private static void LoadAssemblies ()
        {
            var loaded = new[]
            {
                AssemblyPath("Emgu.CV"),
                AssemblyPath("Emgu.CV.UI"),
                AssemblyPath("Emgu.Util")
            };
            var files = Directory.EnumerateFiles(Application.StartupPath + "/../../lib/").Where(f => !loaded.Contains(f));

            Parallel.ForEach(files, (file) =>
            {
                var fileName = file.Split(new[] { '\\', '/' }).Last();
                File.Copy(file, Application.StartupPath + "/" + fileName, overwrite: true);
                Console.WriteLine("Copied {0}", fileName);
            });
        }

        private static string AssemblyPath (string assemblyName) =>
            string.Format(Application.StartupPath + "/../../lib/{0}.dll", assemblyName);
    }
}