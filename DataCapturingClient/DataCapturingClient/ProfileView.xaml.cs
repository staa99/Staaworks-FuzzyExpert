using DataCapturingClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataCapturingClient
{
    /// <summary>
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : Window
    {
        public ProfileView()
        {
            //InitializeComponent();
        }
        //public ProfileView(SearchViewModel svm)
        //{

        //    InitializeComponent();
        //    lblName.Content = svm.Name;
        //    lblPersonId.Content = svm.PersonID;
        //}

        public ProfileView(ProfileViewModel pvm)
        {
            InitializeComponent();
            lblName.Content = pvm.Surname+" "+pvm.Firstname;
            lblPersonId.Content = pvm.PersonId;
            LblDepartmentCode.Content = pvm.Deptment;
            LblDesignation.Content = pvm.Designation;


        }

        private void btnTakePhoto_Click(object sender, RoutedEventArgs e)
        {
            PictureCaptureView pcv = new PictureCaptureView();
            pcv.ShowDialog();
        }
    }
}
