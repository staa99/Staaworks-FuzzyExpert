using DPUruNet;
using FingerCapturing;
using FIngerCapturing.Common;
using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.FaceRecognition;
using Staaworks.BankExpert.Shared.Interfaces;
using Staaworks.BankExpert.Shared.Models;
using Staaworks.BankExpert.WinForms.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static Staaworks.BankExpert.Shared.Interfaces.FaceRecognitionSchemes;
using static Staaworks.BankExpert.WinForms.Authentication.UserAuthenticationScheme;

namespace Staaworks.BankExpert.WinForms.Authentication
{
    partial class UserAuthenticator : UserControl, IViewDataHolder
    {
        public AuthenticationData[] AuthenticationData { get; set; }
        private Dictionary<UserAuthenticationScheme, Action> SchemeMap { get; }
        public User User { get; set; }
        public Control ViewDataControl { get; set; }
        public IDataReciever Reciever { get; set; }
        public Action<bool> OnComplete { get; set; }
        private bool UserExists { get; set; }

        public UserAuthenticator (IDataReciever reciever)
        {
            InitializeComponent();
            Reciever = reciever;
            SchemeMap = new Dictionary<UserAuthenticationScheme, Action>
            {
                [BasicAuthentication] = DoBasicAuth,
                [FaceAuthentication] = DoFaceAuth,
                [FingerprintAuthentication] = DoFingerprintAuth
            };
        }



        public void Authenticate ()
        {
            User = UserRepository.GetUser(EmailTextBox.Text);
            if (User == null)
            {
                User = new User
                {
                    Email = EmailTextBox.Text
                };
                UserExists = false;
            }
            else
            {
                UserExists = true;
            }

            foreach (var data in AuthenticationData)
            {
                SchemeMap[data.Scheme]();
            }
            OnComplete(AuthenticationData.All(data => data.Authenticated));
        }


        private void DoBasicAuth ()
        {
            var basicAuthForm = new Forms.BasicAuthForm(User);
            basicAuthForm.ShowDialog();
            if (UserExists)
            {
                AuthenticationData.Single(d => d.Scheme == BasicAuthentication).Authenticated = basicAuthForm.Successful;
            }
        }


        private void DoFaceAuth ()
        {
            var recognitionSchemeData = new RecognitionSchemeData();
            var recognitionForm = new RecognitionForm(10, (AuthenticationScheme, recognitionSchemeData));
            if (UserExists)
            {
                recognitionForm.OnComplete = () =>
                {
                    AuthenticationData.Single(d => d.Scheme == FaceAuthentication).Authenticated = recognitionForm.GetResolvedLabel() == EmailTextBox.Text;
                };
            }
            else
            {
                recognitionForm.OnComplete = () => Console.WriteLine("Not authenticated");
            }
            recognitionForm.ShowDialog();
        }


        private void DoFingerprintAuth ()
        {
            Verification verificationForm = null;
            var mainObject = new MainObject();
            var readerSelectionForm = new ReaderSelection
            {
                Currentinstance = mainObject
            };

            readerSelectionForm.ShowDialog();

            if (UserExists)
            {
                //uncomment to bypass fingerprint in the absence of scanner
                //AuthenticationData.Single(d => d.Scheme == FingerprintAuthentication).Authenticated = true;
                //return;
                if (verificationForm == null)
                {
                    verificationForm = new Verification
                        (
                            successful =>
                            {
                                successful = successful && UserRepository.LogSuccessfulEntry(verificationForm.SuccessReading, User.Email);

                                AuthenticationData.Single(d => d.Scheme == FingerprintAuthentication).Authenticated = successful;
                            }
                        )
                    {
                        Currentinstance = mainObject
                    };
                }

                var strFmds = User.Snapshots.SelectMany(s => s.Fingerprints).Select(s => s.Fmd);
                if (strFmds.Any())
                {
                    var fmds = new List<Fmd>();
                    foreach (var strFmd in strFmds)
                    {
                        var fmd = Fmd.DeserializeXml(strFmd);
                        fmds.Add(fmd);
                    }
                    verificationForm.database = fmds;

                    verificationForm.ShowDialog();
                }
            }
        }

        public void PopulateControl (params object[] parameters)
        {
            var schemes = parameters.Where(p => p is IEnumerable<UserAuthenticationScheme>).Single();
            AuthenticationData = (schemes as IEnumerable<UserAuthenticationScheme>).Select(scheme => new AuthenticationData
            {
                Scheme = scheme,
                Authenticated = false
            }).ToArray();

            ViewDataControl = this;
            Reciever.AddControl(this);
        }

        private void ContinueAuthButton_Click (object sender, EventArgs e) =>
            Authenticate();
    }
}
