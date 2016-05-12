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
    /// Interaction logic for AdminConsumationWindow.xaml
    /// </summary>
    public partial class AdminConsumationWindow : Window
    {
        private readonly UserModel _user;
        public AdminConsumationWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            cmbDriver.Items.Add(new UserModel { UserName = "", UserId = 0 });
            cmbRegNr.Items.Add(new JournalModel { Regnr = "", CarId = 0 });
            cmbCarType.Items.Add(new CarType { Type = "", Id = 0 });
            GetRegnr().Wait();
            GetDrivers().Wait();
            GetCarTypes().Wait();
            datepicker.IsEnabled = false;
        }

        public async Task GetCarTypes()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Car/GetCarType").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var carType = await response.Content.ReadAsStringAsync();
                List<CarType> carTypeList = JsonConvert.DeserializeObject<List<CarType>>(carType).ToList();

                foreach (var item in carTypeList)
                {
                    cmbCarType.Items.Add(new CarType { Type = item.Type, Id = item.Id });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        public async Task GetRegnr()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Car/GetCars").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var regnrs = await response.Content.ReadAsStringAsync();
                List<CarModel> regnrList = JsonConvert.DeserializeObject<List<CarModel>>(regnrs).ToList();


                foreach (var item in regnrList)
                {
                    cmbRegNr.Items.Add(new JournalModel { Regnr = item.Regnr, CarId = item.Id });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        public async Task GetDrivers()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/User/GetDrivers").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var drivers = await response.Content.ReadAsStringAsync();
                List<UserModel> driverList = JsonConvert.DeserializeObject<List<UserModel>>(drivers).ToList();

                foreach (var item in driverList)
                {
                    cmbDriver.Items.Add(new UserModel { UserName = item.UserName, UserId = item.UserId });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        public void ClearAllTextboxes()
        {
            txtConsumationAll.Clear();
            txtTotalCostAll.Clear();
            txtConsumationYear.Clear();
            txtTotalCostYear.Clear();
            txtConsumationMonth.Clear();
            txtTotalCostMonth.Clear();
            txtallcostonregnryear.Clear();
            txtTotalCostOnRegNrMonth.Clear();
        }
        private void cmbRegNr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var regnr = cmbRegNr.SelectedItem as JournalModel;
            if (regnr != null)
            {
                var carid = regnr.CarId;
                JournalModel journal = new JournalModel();
                journal.CarId = carid;
                if (carid == 0)
                {
                    ClearAllTextboxes();
                    cmbCarType.IsEnabled = true;
                    cmbDriver.IsEnabled = true;
                    cbAllCars.IsEnabled = true;
                    datepicker.IsEnabled = false;
                }
                else
                {
                    cmbCarType.IsEnabled = false;
                    cmbDriver.IsEnabled = false;
                    cbAllCars.IsEnabled = false;
                    datepicker.IsEnabled = true;
                    GetAllConsumationsOnCarId(journal).Wait();
                }

                if (datepicker.SelectedDate.HasValue)
                {
                    if (carid != 0)
                    {
                        datepicker_SelectedDateChanged(datepicker, e);
                    }
                }
            }
        }
        private void cmbDriver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var user = cmbDriver.SelectedItem as UserModel;
            if (user != null)
            {
                var userId = user.UserId;
                JournalModel journal = new JournalModel();
                journal.DriverId = userId;
                if (userId == 0)
                {
                    ClearAllTextboxes();
                    cmbCarType.IsEnabled = true;
                    cmbRegNr.IsEnabled = true;
                    cbAllCars.IsEnabled = true;
                    datepicker.IsEnabled = false;
                }
                else
                {
                    cmbCarType.IsEnabled = false;
                    cmbRegNr.IsEnabled = false;
                    cbAllCars.IsEnabled = false;
                    datepicker.IsEnabled = true;
                    GetAllConsumationsOnDriverId(journal).Wait();
                }


                if (datepicker.SelectedDate.HasValue)
                {
                    if (userId != 0)
                    {
                        datepicker_SelectedDateChanged(datepicker, e);
                    }
                }
            }
        }
        private void cmbCarType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var carType = cmbCarType.SelectedItem as CarType;
            if (carType != null)
            {
                var carTypeId = carType.Id;

                if (carTypeId == 0)
                {
                    ClearAllTextboxes();
                    cmbDriver.IsEnabled = true;
                    cmbRegNr.IsEnabled = true;
                    cbAllCars.IsEnabled = true;
                    datepicker.IsEnabled = false;
                }
                else
                {
                    cmbDriver.IsEnabled = false;
                    cmbRegNr.IsEnabled = false;
                    cbAllCars.IsEnabled = false;
                    datepicker.IsEnabled = true;
                    JournalModel journal = new JournalModel();
                    journal.CarTypeId = carTypeId;
                    if (carTypeId != 0)
                    {
                        GetConsumationAllTimeOnCarType(journal).Wait();
                    }
                }

                if (datepicker.SelectedDate.HasValue)
                {
                    if (carType.Id != 0)
                    {
                        datepicker_SelectedDateChanged(datepicker, e);
                    }
                }
            }
        }

        private void cbAllCars_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAllCars.IsChecked != null && (bool)cbAllCars.IsChecked)
            {
                GetConsumationOnallCars().Wait();
                cmbCarType.IsEnabled = false;
                cmbDriver.IsEnabled = false;
                cmbRegNr.IsEnabled = false;
                datepicker.IsEnabled = true;
            }
            else
            {
                ClearAllTextboxes();
                cmbCarType.IsEnabled = true;
                cmbDriver.IsEnabled = true;
                cmbRegNr.IsEnabled = true;
                datepicker.IsEnabled = false;
            }
        }
        public async Task GetAllConsumationsOnCarId(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllJournalsOnCarId", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationAll.Text = consumation.Consumation.ToString("n2");
                txtTotalCostAll.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationAll.Clear();
                txtTotalCostAll.Clear();
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationsOnMonthOnCarId(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetJournalsOnMonthOnCarId", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationMonth.Text = consumation.Consumation.ToString("n2");
                txtTotalCostMonth.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Fanns inga körjournaler på denna bil denna månad!");
            }
            else
            {
                txtConsumationAll.Clear();
                txtTotalCostAll.Clear();
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationsOnYearOnCarId(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetJournalsOnYearOnCarId", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationYear.Text = consumation.Consumation.ToString("n2");
                txtTotalCostYear.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Fanns inga körjournaler på denna bil detta år!");
            }
            else
            {
                txtConsumationAll.Clear();
                txtTotalCostAll.Clear();
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetAllConsumationsOnDriverId(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetSpecificDriverAllJournals", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationAll.Text = consumation.Consumation.ToString("n2");
                txtTotalCostAll.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationAll.Clear();
                txtTotalCostAll.Clear();
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationOnMonthOnDriverId(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationOnMonthAllCarsOnDriverId", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationMonth.Text = consumation.Consumation.ToString("n2");
                txtTotalCostMonth.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationOnYearOnDriverId(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationPerYearOnDriverId", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationYear.Text = consumation.Consumation.ToString("n2");
                txtTotalCostYear.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        private void datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datepicker.SelectedDate != null)
            {
                var regnr = cmbRegNr.SelectedItem as JournalModel;
                var user = cmbDriver.SelectedItem as UserModel;
                var carType = cmbCarType.SelectedItem as CarType;
                if (regnr != null && regnr.CarId != 0)
                {
                    var carid = regnr.CarId;
                    JournalModel journal = new JournalModel
                    {
                        CarId = carid,
                        Date = datepicker.SelectedDate.Value
                    };
                    if (carid != 0)
                    {
                        GetAllCostOnRegNr(journal).Wait();
                        GetConsumationsOnMonthOnCarId(journal).Wait();
                        GetConsumationsOnYearOnCarId(journal).Wait();
                    }
                }
                else if (user != null && user.UserId != 0)
                {
                    var userId = user.UserId;
                    JournalModel journal = new JournalModel
                    {
                        DriverId = userId,
                        Date = datepicker.SelectedDate.Value
                    };
                    if (userId != 0)
                    {
                        GetConsumationOnMonthOnDriverId(journal).Wait();
                        GetConsumationOnYearOnDriverId(journal).Wait();
                    }
                }
                else if (carType != null && carType.Id != 0)
                {

                    JournalModel journal = new JournalModel
                    {
                        CarTypeId = carType.Id,
                        Date = datepicker.SelectedDate.Value
                    };
                    if (carType.Id != 0)
                    {
                        GetConsumationOnYearOnCarType(journal).Wait();
                        GetConsumationOnMonthOnCarType(journal).Wait();
                    }
                }
                else if (cbAllCars.IsChecked != null && (bool)cbAllCars.IsChecked)
                {
                    JournalModel journal = new JournalModel
                    {
                        Date = datepicker.SelectedDate.Value
                    };
                    GetConsumationOnMonthAllCars(journal).Wait();
                    GetConsumationOnYearAllCars(journal).Wait();
                }
            }
        }

        public async Task GetConsumationOnallCars()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Journal/GetConsumationsOnAllCars").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationAll.Text = consumation.Consumation.ToString("n2");
                txtTotalCostAll.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {

                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationOnMonthAllCars(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetConsumationOnMonthAllCars", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationMonth.Text = consumation.Consumation.ToString("n2");
                txtTotalCostMonth.Text = consumation.Cost.ToString("n2");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationOnYearAllCars(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllJournalsOnYearOnAllCars", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationYear.Text = consumation.Consumation.ToString("n2");
                txtTotalCostYear.Text = consumation.Cost.ToString("n2");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationOnYearOnCarType(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllJournalsOnYearOnCarType", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationYear.Text = consumation.Consumation.ToString("n2");
                txtTotalCostYear.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationOnMonthOnCarType(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllJournalsOnMonthOnCarType", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationMonth.Text = consumation.Consumation.ToString("n2");
                txtTotalCostMonth.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetConsumationAllTimeOnCarType(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllJournalsAllTimeOnCarType", journal).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                ConsumationModel consumation = JsonConvert.DeserializeObject<ConsumationModel>(journals);
                txtConsumationAll.Text = consumation.Consumation.ToString("n2");
                txtTotalCostAll.Text = consumation.Cost.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                txtConsumationAll.Clear();
                txtTotalCostAll.Clear();
                MessageBox.Show("Finns inga journaler på vald biltyp!");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }

        public async Task GetAllCostOnRegNr(JournalModel journal)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.PostAsJsonAsync("api/Journal/GetAllCostOnRegNr", journal).Result;

            if (response.IsSuccessStatusCode)
            {
                var costs = await response.Content.ReadAsStringAsync();
                TotalCostModel cost = JsonConvert.DeserializeObject<TotalCostModel>(costs);
                txtallcostonregnryear.Text = cost.CostYear.ToString("n2");
                txtTotalCostOnRegNrMonth.Text = cost.CostMonth.ToString("n2");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
            else
            {
                txtConsumationMonth.Clear();
                txtTotalCostMonth.Clear();
                txtTotalCostYear.Clear();
                txtConsumationYear.Clear();
            }
        }
    }
}