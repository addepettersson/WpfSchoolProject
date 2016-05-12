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
    /// Interaction logic for AllCarsConsumationWindow.xaml
    /// </summary>
    public partial class AllCarsConsumationWindow : Window
    {
        private UserModel _user;
        public AllCarsConsumationWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
        }

        private void datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datepicker.SelectedDate != null)
            {
                var userId = _user.UserId;
                JournalModel journal = new JournalModel
                {
                    DriverId = userId,
                    Date = datepicker.SelectedDate.Value
                };
                GetConsumationOnMonth(journal).Wait();
                GetConsumationOnYear(journal).Wait();
                GetAllConsumations(journal).Wait();
                GetConsumationOnLastDrive(journal).Wait();
            }
        }

        public async Task GetAllConsumations(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllJournalsAllCars", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel journalList = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationAll.Text = journalList.Consumation.ToString("n2") + " liter";
                txtTotalCostAll.Text = journalList.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        public async Task GetConsumationOnMonth(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationOnMonthAllCarsDriver", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationMonth.Text = consumation.Consumation.ToString("n2") + " liter";
                txtTotalCostMonth.Text = consumation.Cost.ToString("n2");
            }
            else
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        public async Task GetConsumationOnYear(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationPerYearAllCars", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationYear.Text = consumation.Consumation.ToString("n2") + " liter";
                txtTotalCostYear.Text = consumation.Cost.ToString("n2");
            }
            else
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        public async Task GetConsumationOnLastDrive(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationOnLastDriveAllCars", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationOnLastDrive.Text = consumation.Consumation.ToString("n2") + " liter";
                txtTotalCostOnLastDrive.Text = consumation.Cost.ToString("n2");
            }
            else
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }        
    }
}
