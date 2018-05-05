using Staaworks.BankExpert.Shared.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Staaworks.BankExpert.Shared.Interfaces.FaceRecognitionSchemes;

namespace Staaworks.BankExpert.FaceRecognition
{
    static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main ()
        {
            LoadAssemblies();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var recognitionForm = new RecognitionForm(10, (AuthenticationScheme, new RecognitionSchemeData { Email = "malik@mail.domain" }));
            recognitionForm.OnComplete = () =>
            {
                //bool saved = recognitionForm.SchemeInfo.data.Successful;
                //if (saved)
                //{
                //    Console.WriteLine("The picture data of a@b.c was saved");
                //}
                //else
                //{
                //    Console.WriteLine("The picture data of a@b.c was saved");
                //}

                var label = recognitionForm.GetResolvedLabel();
            };
            Application.Run(recognitionForm);
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