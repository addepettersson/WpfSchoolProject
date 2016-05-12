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
using WpfProject.Models;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for DriverWindow.xaml
    /// </summary>
    public partial class DriverWindow : Window
    {
        private readonly UserModel _user;
        public DriverWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
        }

        private void btnCreateJournal_Click(object sender, RoutedEventArgs e)
        {
            CreateDriverJournalWindow createJournalWindow = new CreateDriverJournalWindow(_user);
            createJournalWindow.Show();
        }

        private void btnShowJournal_Click(object sender, RoutedEventArgs e)
        {
            DriverConsumationWindow consumationwindow = new DriverConsumationWindow(_user);
            consumationwindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AllCarsConsumationWindow consumationWindow = new AllCarsConsumationWindow(_user);
            consumationWindow.Show();
        }
    }
}
