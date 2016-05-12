using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Newtonsoft.Json;
using WpfProject.Models;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly UserModel _user;
        public AdminWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            GetBestDriver().Wait();
            GetBestCar().Wait();
        }

        private async Task GetBestDriver()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Journal/GetBestDriverLastMonth").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var best = response.Content.ReadAsAsync<BestValueModel>().Result;
                if (best != null)
                {                    
                    txtBestDriver.Content = best.Username + "  Snittförbrukning: " + best.Value.ToString("n2");
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        private async Task GetBestCar()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Journal/GetBestCarLastMonth").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var best = response.Content.ReadAsAsync<BestValueModel>().Result;
                if (best != null)
                {                    
                    txtBestCar.Content = best.Regnr + "  Snittförbrukning: " + best.Value.ToString("n2");
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            CreateUserWindow createuser = new CreateUserWindow(_user);
            createuser.Show();
        }

        private void btnCreateCar_Click(object sender, RoutedEventArgs e)
        {
            var createcarwindow = new CreateCarWindow(_user);
            createcarwindow.Show();
        }

        private void btnCreateCarCosts_Click(object sender, RoutedEventArgs e)
        {
            CreateCostOnCarWindow createCostWindow = new CreateCostOnCarWindow(_user);
            createCostWindow.Show();
        }

        private void btnCreateDriverJournal_Click(object sender, RoutedEventArgs e)
        {
            AdminJournalWindow journalWindow = new AdminJournalWindow(_user);
            journalWindow.Show();
        }

        private void btnSeeConsumations_Click(object sender, RoutedEventArgs e)
        {
            AdminConsumationWindow consumationWindow = new AdminConsumationWindow(_user);
            consumationWindow.Show();
        }

        private void btnChangeApiConnection_Click(object sender, RoutedEventArgs e)
        {
            AppConfigWindow config = new AppConfigWindow();
            config.Show();
        }

        private void btnDeeperAnalysis_Click(object sender, RoutedEventArgs e)
        {
            DeeperAnalysisWindow window = new DeeperAnalysisWindow(_user);
            window.Show();
        }
    }
}
