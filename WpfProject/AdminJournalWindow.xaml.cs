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
    /// Interaction logic for AdminJournalWindow.xaml
    /// </summary>
    public partial class AdminJournalWindow : Window
    {
        private readonly UserModel _user;
        public AdminJournalWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            GetCars().Wait();
            GetDrivers().Wait();
        }
        public async Task GetCars()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Car/GetCars").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var cars = await response.Content.ReadAsStringAsync();
                List<CarModel> carList = JsonConvert.DeserializeObject<List<CarModel>>(cars).ToList();
                foreach (var item in carList)
                {
                    cmbCars.Items.Add(new CarModel { Regnr = item.Regnr, Id = item.Id });
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
                    cmbUser.Items.Add(new UserModel { UserName = item.UserName, UserId = item.UserId });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        public async Task GetFuelType(int carId)
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Car/GetFuelTypeToShow").Result;
            var responseCars = client.GetAsync("api/Car/GetCars").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var fuelTypes = await response.Content.ReadAsStringAsync();
                List<FuelTypeModel> fuelTypeList = JsonConvert.DeserializeObject<List<FuelTypeModel>>(fuelTypes).ToList();

                var carsList = await responseCars.Content.ReadAsStringAsync();
                List<CarModel> carList = JsonConvert.DeserializeObject<List<CarModel>>(carsList).ToList();
                CarModel carModel = new CarModel();
                foreach (var cars in carList)
                {
                    if (cars.Id == carId)
                    {
                        carModel = cars;
                    }
                }
                foreach (var item in fuelTypeList)
                {
                    if (carModel.FuelType_Id == 6)
                    {
                        cmbFuelType.Items.Add(new FuelTypeModel { FuelType = "Etanol", Id = 8 });
                        cmbFuelType.Items.Add(new FuelTypeModel { FuelType = "Bensin", Id = 5 });
                        break;
                    }
                    if (item.Id == carModel.FuelType_Id)
                    {
                        cmbFuelType.Items.Add(new FuelTypeModel { FuelType = item.FuelType, Id = item.Id });
                    }
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        private void cmbCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbFuelType.Items.Clear();
            var carId = cmbCars.SelectedItem as CarModel;
            if (carId != null)
            {
                GetFuelType(carId.Id).Wait();
            }
        }

        private void CountFuel(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (!string.IsNullOrWhiteSpace(txtmass.Text) && !string.IsNullOrWhiteSpace(txtUnitPrice.Text))
            {
                decimal mass = 0;
                decimal price = 0;
                if (decimal.TryParse(txtmass.Text, out mass) && decimal.TryParse(txtUnitPrice.Text, out price))
                {
                    var totalPrice = mass * price;
                    txtTotalPrice.Text = totalPrice.ToString();
                }
                else
                {
                    MessageBox.Show("Felaktig inmatning, får endast vara nummer");
                }
            }
            else
            {
                txtTotalPrice.Text = "";
            }
        }

        public async Task CreateJournal()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var selectedCar = cmbCars.SelectedItem as CarModel;
            var selectedFuelType = cmbFuelType.SelectedItem as FuelTypeModel;
            var selectedUser = cmbUser.SelectedItem as UserModel;
            int mileage;

            if (selectedCar != null && selectedFuelType != null && int.TryParse(txtMileage.Text, out mileage) &&
                selectedUser != null && !string.IsNullOrWhiteSpace(datepicker.Text))
            {
                JournalModel journal = null;
                if (selectedFuelType.FuelType == "El")
                {
                    journal = new JournalModel
                    {
                        CarId = selectedCar.Id,
                        FuelTypeId = selectedFuelType.Id,
                        Date = Convert.ToDateTime(datepicker.Text),
                        DriverId = selectedUser.UserId,
                        FuelAmount = 0,
                        PricePerUnit = 0,
                        TotalPrice = 0,
                        MileAge = mileage
                    };
                }
                else
                {
                    decimal fuelamount;
                    decimal pricePerUnit;
                    decimal totalPrice;

                    if (decimal.TryParse(txtmass.Text, out fuelamount) &&
                        decimal.TryParse(txtUnitPrice.Text, out pricePerUnit) &&
                        decimal.TryParse(txtTotalPrice.Text, out totalPrice))
                    {
                        journal = new JournalModel
                        {
                            CarId = selectedCar.Id,
                            FuelTypeId = selectedFuelType.Id,
                            Date = Convert.ToDateTime(datepicker.Text),
                            DriverId = selectedUser.UserId,
                            FuelAmount = fuelamount,
                            PricePerUnit = pricePerUnit,
                            TotalPrice = totalPrice,
                            MileAge = mileage
                        };
                    }
                    else
                    {
                        MessageBox.Show("Något värde är inte ifyllt!");
                    }
                }

                if (journal != null)
                {
                    var response = client.PostAsJsonAsync("api/Journal/CreateJournal", journal).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        MessageBox.Show("Journalen är skapad!");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotModified)
                    {
                        MessageBox.Show("Någon har redan skapat en journal med denna mätarställning och bensintyp!");
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        MessageBox.Show("Uppkoppling till servern misslyckades!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Något värde är inte ifyllt!");
            }
        }

        private void btncreateJournal_Click(object sender, RoutedEventArgs e)
        {
            CreateJournal().Wait();
        }

        private void cmbFuelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fueltype = cmbFuelType.SelectedItem as FuelTypeModel;
            if (fueltype != null && fueltype.FuelType == "El")
            {
                txtmass.IsEnabled = false;
                txtTotalPrice.IsEnabled = false;
                txtUnitPrice.IsEnabled = false;
            }
            else
            {
                txtmass.IsEnabled = true;
                txtTotalPrice.IsEnabled = true;
                txtUnitPrice.IsEnabled = true;
            }
        }
    }
}
