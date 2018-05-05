using DPUruNet;
using FingerCapturing;
using Staaworks.BankExpert.Core.Repositories;
using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
                   faceCaptureDone = false,
                       bio_altered = false;

        public UserCreator_BasicForm()
        {
            InitializeComponent();
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                basicAuthDone = true;

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
                            HashSalt = Guid.NewGuid().ToString(),
                            PasswordHash = BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(PasswordTextBox.Text)))
                        });
                        userSaved = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
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
                                bio_altered = false; MessageBox.Show("FingerPrint Saved Successully", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            // Registration complete
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

            var mail = new System.Net.Mail.MailAddress(EmailTextBox.Text);
            if (!Regex.IsMatch(EmailTextBox.Text, "^[\\w.-]+@(?=[a-z\\d][^.]*\\.)[a-z\\d.-]*(?<![.-])$"))
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
                CreatorData.FingerprintData.EnrollmentControl.Currentinstance = CreatorData.FingerprintData.MainObject; // this;

            }
            bio_altered = true;
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


        public byte[] ImageToByte(System.Drawing.Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        public byte[] ImageToByte2(System.Drawing.Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }


        private void EnrollFingerButton_Click (object sender, EventArgs e) => GoToFingerprintEnrollment();


        private void FaceRegButton_Click (object sender, EventArgs e)
        {
            var email = EmailTextBox.Text;
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, "^[\\w.-]+@(?=[a-z\\d][^.]*\\.)[a-z\\d.-]*(?<![.-])$"))
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


        public byte[] BufferFromImage(BitmapImage imageSource)
        {
            var stream = imageSource.StreamSource;
            byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (var br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((int) stream.Length);
                }
            }

            return buffer;
        }


        public static byte[] ImageToBytes(BitmapImage img)
        {
            using (var ms = new MemoryStream())
            {
                var btmMap = new WriteableBitmap(img);

                //  System.Windows.Media.Imaging.Extensions.SaveJpeg(btmMap, ms, img.PixelWidth, img.PixelHeight, 0, 100);
                img = null;
                return ms.ToArray();
            }
        }


        private void VerifyFingerprint(object sender, EventArgs e)
        {
            Verification verificationForm = null;
            if (CreatorData.FingerprintData.ReaderSelectionForm == null)
            {
                CreatorData.FingerprintData.ReaderSelectionForm = new ReaderSelection
                {
                    // CreatorData.FingerprintData.ReaderSelectionForm.Sender = this;
                    Currentinstance = CreatorData.FingerprintData.MainObject
                };
            }

            CreatorData.FingerprintData.ReaderSelectionForm.ShowDialog();

            if (verificationForm == null)
            {
                verificationForm = new Verification
                {
                    Currentinstance = CreatorData.FingerprintData.MainObject // this;
                };
            }

            var strFmds = CreatorData.User?.Snapshots?.SelectMany(s => s.Fingerprints)?.Select(s => s.Fmd);
            if (strFmds != null && strFmds.Any())
            {
                var fmds = new List<Fmd>();
                foreach (var strFmd in strFmds)
                {
                    var fmd = Fmd.DeserializeXml(strFmd);
                    fmds.Add(fmd);
                }
                verificationForm.database = fmds;
                // MessageBox.Show("Reader available:"+ (CreatorData.FingerprintData.MainObject.CurrentReader!=null).ToString());

                verificationForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No fingerprint found in database", "Alert");
            }
        }
    }
}
