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
    public partial class SearchWindowDummy : Window
    {
        public SearchWindowDummy()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchPeople repo = new SearchPeople();
           List<Person> searchresults=  repo.filterPeople(txtSearchField.Text,"","",persontype.ALL);
            dataGrid.ItemsSource = searchresults;
            
        }
    }
}
