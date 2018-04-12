﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataCapturingClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
        

        private void mnuFetchRecord_Click(object sender, RoutedEventArgs e)
        {
            try {
                SearchWindow cw = new SearchWindow();
                cw.ShowInTaskbar = false;
                cw.Owner = Application.Current.MainWindow;
                cw.ShowDialog();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
