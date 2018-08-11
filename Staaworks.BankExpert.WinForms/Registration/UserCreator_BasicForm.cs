using DPUruNet;
using FingerCapturing;
using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Staaworks.BankExpert.WinForms.Registration
{
    partial class UserCreator_BasicForm : UserControl
    {
        public UserCreatorData CreatorData { get; set; }
        private bool basicAuthDone = false,
                fingerprintRegDone = false,
                   faceCaptureDone = false;

        public UserCreator_BasicForm()
        {
            InitializeComponent();
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                basicAuthDone = true;
                UseWaitCursor = true;
                Cursor = Cursors.WaitCursor;

                if (basicAuthDone && fingerprintRegDone && faceCaptureDone)
                {
                    var userSaved = false;
                    try
                    {
                        CreatorData.User = UserRepository.CreateUser(new User
                        {
                            Name = NameTextBox.Text,
                            Address = AddressTextBox.Text,
                            Phone = PhoneTextBox.Text,
                            Email = EmailTextBox.Text,
                            ZipOrPostalAddress = ZipOrPostalCodeTextBox.Text,
                            HashSalt = Guid.NewGuid().ToString()
                        });

                        CreatorData.User.PasswordHash = BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text + CreatorData.User.HashSalt)));
                        userSaved = true;
                    }
                    catch (Exception ex)
                    {
                        string addMessage (string message, Exception exception)
                        {
                            if (message == null)
                            {
                                message = string.Format("{0}\n", ex.Message);
                            }
                            else
                            {
                                message = string.Format("{0}\nCaused by {1}\n", message, ex.Message);
                            }

                            if (ex.InnerException != null)
                            {
                                return addMessage(message, ex.InnerException);
                            }
                            else
                            {
                                return message;
                            }
                        }
                        MessageBox.Show(addMessage(null, ex), "Error");
                    }

                    var fingerprintSaved = false;
                    if (userSaved)
                    {
                        try
                        {
                            var Fmds = new Dictionary<int, string>();
                            var Fids = new Dictionary<int, List<string>>();
                            foreach (var finger in CreatorData.FingerprintData.MainObject.Fmds.Keys)
                            {
                                Fmds.Add(finger, Fmd.SerializeXml(CreatorData.FingerprintData.MainObject.Fmds[finger]));
                                //MessageBox.Show("extracted fmd :" + Fmd.SerializeXml(CreatorData.FingerprintData.MainObject.Fmds[finger]), "Alert");
                                var fids = new List<string>();
                                foreach (var fid in CreatorData.FingerprintData.MainObject.Fids[finger])
                                {
                                    fids.Add(Fid.SerializeXml(fid));
                                    //   MessageBox.Show("extracted fid :"+Fid.SerializeXml(fd), "Alert");
                                }
                                Fids.Add(finger, fids);
                            }
                            var b = UserRepository.SaveFingerPrint(Fmds, Fids, CreatorData.User.Email, null);
                            if (b)
                            {
                                fingerprintSaved = true;
                                CreatorData.FingerprintData.MainObject.Fids.Clear();
                                CreatorData.FingerprintData.MainObject.Fmds.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Error Saving FingerPrint. ", "Alert");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error Saving FingerPrint. Details:" + ex.StackTrace, "Alert");
                        }

                        if (fingerprintSaved)
                        {
                            // Registration complete, display form to enter required input data
                            var financialDataForm = new FinancialDataForm(CreatorData.User, FinancialDataFormMode.edit);
                            financialDataForm.ShowDialog();
                            UseWaitCursor = false;
                            CreatorData.Reciever.LoadTask("QuestionAndAnswer");
                        }
                        else
                        {
                            Console.Beep();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error Saving User. ", "Alert");
                        Console.Beep();
                    }
                }
            }
            else
            {
                Console.Beep();
            }
        }


        private bool ValidateControls()
        {
            if (NameTextBox.Text == string.Empty ||
                PhoneTextBox.Text == string.Empty ||
                EmailTextBox.Text == string.Empty ||
                AddressTextBox.Text == string.Empty ||
                ZipOrPostalCodeTextBox.Text == string.Empty ||
                PasswordTextBox.Text == string.Empty ||
                PasswordTextBox.Text != ConfirmPasswordTextBox.Text)
            {
                return false;
            }

            return true;
        }


        private void GoToFingerprintEnrollment()
        {
            //TODO: Load the Enrollment Form
            if (CreatorData.FingerprintData.ReaderSelectionForm == null)
            {
                CreatorData.FingerprintData.ReaderSelectionForm = new ReaderSelection
                {
                    // CreatorData.FingerprintData.ReaderSelectionForm.Sender = this;
                    Currentinstance = CreatorData.FingerprintData.MainObject
                };
            }

            CreatorData.FingerprintData.ReaderSelectionForm.ShowDialog();

            if (CreatorData.FingerprintData.EnrollmentControl == null)
            {
                CreatorData.FingerprintData.EnrollmentControl = new EnrollmentControl();
                CreatorData.FingerprintData.EnrollmentControl.SCevent += SaveFingerprint;
                CreatorData.FingerprintData.EnrollmentControl.Currentinstance = CreatorData.FingerprintData.MainObject;
            }
            CreatorData.FingerprintData.EnrollmentControl.ShowDialog();
        }


        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }


        private void SaveFingerprint () => fingerprintRegDone = true;


        private void EnrollFingerButton_Click (object sender, EventArgs e) => GoToFingerprintEnrollment();


        private void FaceRegButton_Click (object sender, EventArgs e)
        {
            var email = EmailTextBox.Text;
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.Beep();
                return;
            }

            CreatorData.FaceData.RecognitionSchemeData.Email = email;
            CreatorData.FaceData.RecognitionForm.OnComplete = () =>
            {
                if (CreatorData.FaceData.RecognitionSchemeData.Successful)
                {
                    faceCaptureDone = true;
                    CreatorData.FaceData.RecognitionSchemeData = null;
                    CreatorData.FaceData.RecognitionForm.Close();
                }
                else
                {
                    Console.Beep();
                }
            };

            CreatorData.FaceData.RecognitionForm.ShowDialog();
        }
    }
}
