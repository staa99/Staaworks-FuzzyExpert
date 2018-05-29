using Staaworks.BankExpert.Shared.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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


            //try
            //{
            //    var fromAddress = new MailAddress("olanloyeam@gmail.com", "Staa99");
            //    var toAddress = new MailAddress("staaspecial@google.com", null);
            //    const string fromPassword = "P@$$w0rd";
            //    var subject = "Test message from smtp";
            //    var body = "This is from staa99. Test message from SMTP";

            //    var smtp = new SmtpClient
            //    {
            //        Host = "smtp.gmail.com",
            //        Port = 587,
            //        EnableSsl = true,
            //        DeliveryMethod = SmtpDeliveryMethod.Network,
            //        UseDefaultCredentials = false,
            //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            //    };
            //    using (var message = new MailMessage(fromAddress, toAddress)
            //    {
            //        Subject = subject,
            //        Body = body
            //    })
            //    {
            //        Console.WriteLine("Sending");
            //        smtp.Send(message);
            //        Console.WriteLine("Sent");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("An error occurred\n{0}\n{1}", ex.Message, ex.StackTrace);
            //}
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