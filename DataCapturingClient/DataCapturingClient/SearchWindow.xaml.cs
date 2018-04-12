using DataCapturingClient.ViewModels;
using IdentityBusinessLayer.Models;
using IdentityCommons.Models;
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
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

        //public object MessageBoxButtons { get; private set; }
        
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //string idtty=null;
           
            SearchPeople repo = new SearchPeople();
            //PersonRepository repo1 = new PersonRepository();
            //bool found = false;
            string message = null;
            string caption = "Important Message!"; ;
            if( String.IsNullOrEmpty(txtIdNumber.Text.Trim()) && (String.IsNullOrEmpty(txtSurname.Text.Trim())) && (String.IsNullOrEmpty(txtFirstName.Text.Trim())) )
            {
                message = "Search Fields Cannot be Empty. Kindly Provide ANY Valid Search Value!";
                MessageBox.Show(message, caption);
                
            }
            else {
                // check with at least one searchVariables
               /* if ((txtIdNumber.Text.Length > 0) || !String.IsNullOrEmpty(txtIdNumber.Text.Trim()))
                {

                    idtty = txtIdNumber.Text;
                }
                else if ((txtSurname.Text.Length > 0) || !String.IsNullOrEmpty(txtSurname.Text.Trim()))
                {

                    idtty = txtSurname.Text;
                }
                else {
                    idtty = txtFirstName.Text;
                }
                */
                List<Person> searchresults = repo.filterPeople(txtIdNumber.Text.ToString(), txtSurname.Text.ToString(), txtFirstName.Text.ToString());
                if (searchresults.Count() == 0 && chkSearchSource.IsChecked==true)
                {
                    searchresults.Add(repo.filterStudentPersonById(txtIdNumber.Text, chkSearchSource.IsChecked==true));
                    repo = new SearchPeople();
                }
                //List<IPerson> searchresults = repo.getPersons(idtty);

                //IEnumerable<StaffPerson> searchresult1 = (IEnumerable <StaffPerson>) repo1.getStaffPersonByID(txtIdNumber.Text);
                //IEnumerable<StaffPerson> searchresultList = (IEnumerable<StaffPerson>)searchresult1;
                
                IEnumerable<SearchViewModel> searchviewresults = from a in searchresults select new SearchViewModel { PersonID=a.PersonID,Name=a.Surname+" "+a.Firstname, Phone=a.Phone, Sex= a.Sex.ToString() , NextOfKinName=a.NextOfKin==null?"":a.NextOfKin.Surname+""+a.NextOfKin.Firstname };
                dataGrid.ItemsSource = searchviewresults;
                
                IEnumerable<ProfileViewModel> profileresults = from pvi in searchresults select new ProfileViewModel { PersonId = pvi.PersonID, Surname = pvi.Surname, Middlename=pvi.Middlename, Gender=pvi.Sex, Phone=pvi.Phone, Address=pvi.Address, HealthId = pvi.HCN, BloodGrp=pvi.BG, Deptment ="", Next_of_KinName = pvi.NextOfKin == null ? "" : pvi.NextOfKin.Surname + "" + pvi.NextOfKin.Firstname, Next_of_KinPhone=pvi.NextOfKin.Phone, Next_of_KinAddress = pvi.NextOfKin.Address};
                //IEnumerable<ProfileViewModel> profileresults = from pvi in searchresultList select new ProfileViewModel { PersonId = pvi.PersonID, Surname = pvi.Surname, Middlename = pvi.Middlename, Gender = pvi.Sex, Phone = pvi.Phone, Address = pvi.Address, HealthId = pvi.HCN, BloodGrp = pvi.BG, Deptment = "", Next_of_KinName = pvi.NextOfKin == null ? "" : pvi.NextOfKin.Surname + "" + pvi.NextOfKin.Firstname, Next_of_KinPhone = pvi.NextOfKin.Phone, Next_of_KinAddress = pvi.NextOfKin.Address };
            }
            
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try {
                DataGrid dg = (DataGrid)sender;
                if (dg.SelectedItem != null)
                {
                    ProfileViewModel pvm = new ProfileViewModel((SearchViewModel)dg.SelectedItem,chkSearchSource.IsChecked==true);
                    Profile pf = new Profile(pvm);
                    pf.ShowDialog();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataGrid dg = (DataGrid)sender;
                if (dg.SelectedItem != null)
                {
                    ProfileViewModel pvm = new ProfileViewModel((SearchViewModel)dg.SelectedItem,chkSearchSource.IsChecked==true);
                    Profile pf = new Profile(pvm);
                    pf.ShowDialog();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void txtIdNumber_KeyDown(object sender, KeyEventArgs e)
        {
           /* if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                //you may pass the parameters if you need
               
            }*/
        }
    }
}
