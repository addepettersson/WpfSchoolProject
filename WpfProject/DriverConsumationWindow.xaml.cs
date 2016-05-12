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
    /// Interaction logic for DriverConsumationWindow.xaml
    /// </summary>
    public partial class DriverConsumationWindow : Window
    {
        private readonly UserModel _user;
        public DriverConsumationWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            GetRegnr(_user).Wait();
        }
        public async Task GetRegnr(UserModel user)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetRegnrOnDriver", user).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var regnr = await response.Content.ReadAsStringAsync();
                List<JournalModel> regnrList = JsonConvert.DeserializeObject<List<JournalModel>>(regnr).ToList();
                
                foreach (var journal in regnrList)
                {
                    cmbRegnr.Items.Add(new JournalModel { Regnr = journal.Regnr, CarId = journal.CarId });                    
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        private void cmbRegnr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var regnr = cmbRegnr.SelectedItem as JournalModel;
            var carid = regnr.CarId;
            JournalModel journal = new JournalModel();
            journal.CarId = carid;            
            journal.DriverId = _user.UserId;
            GetAllConsumations(journal).Wait();
            GetConsumationOnLastDrive(journal).Wait();
            if (datepicker.SelectedDate.HasValue)
            {
                datepicker_SelectedDateChanged(datepicker, e);
            }
        }

        public async Task GetAllConsumations(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllJournalsDriver", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationAll.Text = consumation.Consumation.ToString("n2") + " liter";
                txtTotalCostAll.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        public async Task GetConsumationOnMonth(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationOnMonth", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationMonth.Text = consumation.Consumation.ToString("n2") + " liter";
                txtTotalCostMonth.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Finns inga journaler på vald på månad!");
            }
        }

        public async Task GetConsumationOnYear(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationPerYear", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationYear.Text = consumation.Consumation.ToString("n2") + " liter";
                txtTotalCostYear.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        public async Task GetConsumationOnLastDrive(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationOnLastDrive", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationOnLastDrive.Text = consumation.Consumation.ToString("n2") + " liter";
                txtTotalCostOnLastDrive.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationOnLastDrive.Clear();
                txtTotalCostOnLastDrive.Clear();
            }
        }

        private void datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datepicker.SelectedDate != null)
            {
                var regnr = cmbRegnr.SelectedItem as JournalModel;
                if (regnr != null)
                {
                    var carid = regnr.CarId;
                    var userId = _user.UserId;
                    JournalModel journal = new JournalModel
                    {
                        CarId = carid,
                        DriverId = userId,
                        Date = datepicker.SelectedDate.Value
                    };
                    GetConsumationOnMonth(journal).Wait();
                    GetConsumationOnYear(journal).Wait();
                }
                else
                {
                    MessageBox.Show("Du måste välja en bil innan du kan välja månad!");
                }
            }
        }
    }
}
