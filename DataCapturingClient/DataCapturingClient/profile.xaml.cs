using DataCapturingClient.ViewModels;
using DPUruNet;
using FingerCapturing;
using FIngerCapturing.Common;
using IdentityBusinessLayer.Repositories;
using IdentityCommons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DataCapturingClient
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Profile : Window
    {
        MainObject mobj = new MainObject();
        EnrollmentControl enrollmentControl;
        private ReaderSelection _readerSelection;
        private bool pic_altered=false;
        private bool bio_altered = false;
        private bool profile_altered = false;
        ProfileViewModel pvm;
        string hash = "";
        public Profile()
        {
            //InitializeComponent();
        }
        public Profile(ProfileViewModel pvm)
        {
            InitializeComponent();
            this.pvm = pvm;
            txtIdNumber.Text = pvm.PersonId;
            txtSurname.Text  = pvm.Surname;
            txtFirstname.Text = pvm.Firstname;
            txtOthername.Text = pvm.Middlename;
            txtCollege.Text = pvm.College;
            txtDepartment.Text = pvm.Deptment;
            txtHealthId.Text = pvm.HealthId;
            txtPhone.Text = pvm.Phone;
            txtEmail.Text = pvm.Email;
            txtLevel.Text = pvm.Level;
            txtSponsorName.Text = pvm.SponsorName;
            txtSponsorPhone.Text = pvm.SponsorPhone;
            txtSponsorAddress.Text = pvm.SponsorAddress;
            if (pvm.categoryid != 1)
            {
                txtSponsorName.IsEnabled = true;
                txtSponsorPhone.IsEnabled = true;
                txtSponsorAddress.IsEnabled = true;
            }
           // txtDesignation.Text = pvm.Designation;
            //cmbBloodGrp.SelectedValue = pvm.BloodGrp;
            if (pvm.Gender == 'M') {
                rbnSexMale.IsChecked = true;
            }
            else if (pvm.Gender == 'F')
            {
                rbnSexFemale.IsChecked = true;
            }
            imgboxCapture.Source =Profile.LoadImage(pvm.Photo);

            btnVerifyFingers.IsEnabled = pvm.Fmds!=null;
            // imagelogo.Source= (ImageSource)Resources["FUNAAB_Logo"];
            PersonRepository repo = new PersonRepository();
            List<string> bgrps = new List<string> { "select" };
            bgrps.AddRange(repo.getAllBG());
            cmbBloodGrp.ItemsSource = bgrps; 
            cmbBloodGrp.SelectedItem = pvm.BloodGrp==null?"select":pvm.BloodGrp.Trim();
            // cmbBloodGrp.
           hash= Util.getMd5Hash(extractUserInput());
        }
        private string extractUserInput()
        {
            string s = txtIdNumber.Text +
             txtSurname.Text +
            txtFirstname.Text +
            txtOthername.Text +
            txtCollege.Text +
            txtDepartment.Text +
            txtHealthId.Text +
            txtPhone.Text +
            txtEmail.Text +
            txtLevel.Text +
            txtSponsorName.Text +
            txtSponsorPhone.Text +
            txtSponsorAddress.Text +
             rbnSexMale.IsChecked.ToString() +
             rbnSexFemale.IsChecked.ToString() +
              cmbBloodGrp.SelectedItem.ToString();
            return s;
        }
        private void btnTakePhoto_Click(object sender, RoutedEventArgs e)
        {
            PictureCaptureView pcv = new PictureCaptureView(this);
            pcv.ShowDialog();
            pic_altered = true;
        }
        private void savebiometriconformclose()
        {
            savefingerprint();
        }
        

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (pic_altered)
            {
                savepic();
            }
            if (bio_altered)
            {
                savefingerprint();
            }
            if (Util.getMd5Hash(extractUserInput()) != hash)
            {

               // MessageBox.Show("Profile altered");
                PersonRepository repo = new PersonRepository();
                // StudentPerson newprofile = new StudentPerson();
               pvm.PersonId = txtIdNumber.Text;
               pvm.Surname = txtSurname.Text;
               pvm.Firstname = txtFirstname.Text;
               pvm.Middlename = txtOthername.Text;
               pvm.College = txtCollege.Text;
               pvm.Deptment = txtDepartment.Text;
               pvm.HealthId = txtHealthId.Text;
               pvm.Phone = txtPhone.Text;
               pvm.Email = txtEmail.Text;
               pvm.Level= txtLevel.Text;
               pvm.SponsorName= txtSponsorName.Text;
               pvm.SponsorPhone = txtSponsorPhone.Text;
               pvm.SponsorAddress = txtSponsorAddress.Text;
                
                if (rbnSexMale.IsChecked==true)
                {
                    pvm.Gender = 'M';
                }
                else if (rbnSexFemale.IsChecked == true)
                {
                    
                    pvm.Gender = 'F';
                }

                pvm.BloodGrp = cmbBloodGrp.SelectedItem.ToString();
                bool b = pvm.saveChanges();
                if (b) { profile_altered = false;hash= Util.getMd5Hash(extractUserInput()); MessageBox.Show("Profile Saved Successully", "Alert", MessageBoxButton.OK, MessageBoxImage.Information); }
                else
                {
                    MessageBox.Show("Error Saving Profile", "Alert");
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (pic_altered)
            {
             if(MessageBox.Show("Do you want save your changes to photo before closing?","Alert",MessageBoxButton.YesNo)== MessageBoxResult.Yes)
                {
                    //save changes;
                    savepic();
                  
                }
            }
            if (bio_altered)
            {
                if (MessageBox.Show("Do you want save your changes to biometric before closing?", "Alert", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //save changes;
                    savefingerprint();
                }
            }

        }

        private void btnEnrollFingers_Click(object sender, RoutedEventArgs e)
        {

            if (_readerSelection == null)
            {
                _readerSelection = new ReaderSelection();
                // _readerSelection.Sender = this;
                _readerSelection.Currentinstance = mobj;
            }

            _readerSelection.ShowDialog(); ;

            if (enrollmentControl == null)
            {
                enrollmentControl = new EnrollmentControl();
                enrollmentControl.Scevent += savebiometriconformclose;
                // enrollmentControl.Sender = new MainObject(); // this;

                // mobj.CurrentReader = mobj.CurrentReader;
                enrollmentControl.Currentinstance = mobj; // this;

            }
            bio_altered = true;
            enrollmentControl.ShowDialog();

        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
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
        private void savepic()
        {
            try {
                BitmapImage bi = (BitmapImage)imgboxCapture.Source;
                System.Drawing.Image img = new Bitmap(bi.StreamSource);
                byte[] bb = ImageToByte2(img);
                PersonRepository repo = new PersonRepository();
                bool b = repo.savePersonPic(bb, pvm.PersonId, null);
                if (b) { pic_altered = false; MessageBox.Show("Picture Saved Successully", "Alert",MessageBoxButton.OK,MessageBoxImage.Information); }
                else
                {
                    MessageBox.Show("Error Saving Picture", "Alert");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Saving Picture?", "Alert");
            }
        }
        private void savefingerprint()
        {
            try
            {
                Dictionary<int, string> Fmds = new Dictionary<int, string>();
                Dictionary<int, List<string>> Fids = new Dictionary<int, List<string>>();
                foreach(int finger in mobj.Fmds.Keys)
                {
                    Fmds.Add(finger, Fmd.SerializeXml(mobj.Fmds[finger]));
                    //MessageBox.Show("extracted fmd :" + Fmd.SerializeXml(mobj.Fmds[finger]), "Alert");
                    List<string> fds = new List<string>();
                    foreach(Fid fd in mobj.Fids[finger])
                    {
                        fds.Add(Fid.SerializeXml(fd));
                     //   MessageBox.Show("extracted fid :"+Fid.SerializeXml(fd), "Alert");
                    }
                    Fids.Add(finger, fds);
                }
                PersonRepository repo = new PersonRepository();
                bool b = repo.saveFingerPrint(Fmds,Fids, pvm.PersonId, null);
                if (b) { bio_altered = false; MessageBox.Show("FingerPrint Saved Successully", "Alert", MessageBoxButton.OK, MessageBoxImage.Information); } else
                {
                    MessageBox.Show("Error Saving FingerPrint. " , "Alert");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Saving FingerPrint. Details:"+ex.StackTrace, "Alert");
            }
        }
        public  byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        public  byte[] ImageToByte2(System.Drawing.Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public Byte[] BufferFromImage(BitmapImage imageSource)
        {
            Stream stream = imageSource.StreamSource;
            Byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }
           
            return buffer;
        }
        public static byte[] ImageToBytes(BitmapImage img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteableBitmap btmMap = new WriteableBitmap(img);
              
             //  System.Windows.Media.Imaging.Extensions.SaveJpeg(btmMap, ms, img.PixelWidth, img.PixelHeight, 0, 100);
                img = null;
                return ms.ToArray();
            }
        }

        private void btnVerifyFingers_Click(object sender, RoutedEventArgs e)
        {
            Verification vf = null;
            if (_readerSelection == null)
            {
                _readerSelection = new ReaderSelection();
                // _readerSelection.Sender = this;
                _readerSelection.Currentinstance = mobj;
            }

            _readerSelection.ShowDialog(); 

            if (vf == null)
            {
                vf = new Verification();
               
                vf.Currentinstance = mobj; // this;

            }
            if (pvm.Fmds != null)
            {
                List<Fmd> fmds = new List<Fmd>();
                foreach (int key in pvm.Fmds.Keys)
                {
                    Fmd fmd = Fmd.DeserializeXml(pvm.Fmds[key]);
                    fmds.Add(fmd);
                }
                vf.database = fmds;
               // MessageBox.Show("Reader available:"+ (mobj.CurrentReader!=null).ToString());

                vf.ShowDialog();
            }
            else
            {
                MessageBox.Show("No fingerprint found in database", "Alert");
            }
        }
    }
}
