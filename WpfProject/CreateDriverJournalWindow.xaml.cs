using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CreateDriverJournalWindow.xaml
    /// </summary>
    public partial class CreateDriverJournalWindow : Window
    {
        private UserModel _user;
        public CreateDriverJournalWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            GetCars().Wait();
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
            var mileage = 0;
            if (selectedCar != null && selectedFuelType != null && int.TryParse(txtMileage.Text, out mileage))
            {
                JournalModel journal;
                if (selectedFuelType.FuelType == "El")
                {
                    journal = new JournalModel
                    {
                        CarId = selectedCar.Id,
                        FuelTypeId = selectedFuelType.Id,
                        Date = DateTime.Now,
                        DriverId = _user.UserId,
                        FuelAmount = 0,
                        PricePerUnit = 0,
                        TotalPrice = 0,
                        MileAge = mileage
                    };
                }
                else
                {

                    journal = new JournalModel
                    {
                        CarId = selectedCar.Id,
                        FuelTypeId = selectedFuelType.Id,
                        Date = DateTime.Now,
                        DriverId = _user.UserId,
                        FuelAmount = Convert.ToDecimal(txtmass.Text),
                        PricePerUnit = Convert.ToDecimal(txtUnitPrice.Text),
                        TotalPrice = Convert.ToDecimal(txtTotalPrice.Text),
                        MileAge = mileage
                    };
                }
                var response = client.PostAsJsonAsync("api/Journal/CreateJournal", journal).Result;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        MessageBox.Show("Journalen är skapad!");
                        break;
                    case HttpStatusCode.NotModified:
                        MessageBox.Show("Någon har redan skapat en journal med denna mätarställning och bensintyp!");
                        break;
                    case HttpStatusCode.InternalServerError:
                        MessageBox.Show("Uppkoppling till servern misslyckades!");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Något värde är inte ifyllt!");
            }
        }

        private void btncreateJournal_Click(object sender, RoutedEventArgs e)
        {
            var isCertain = MessageBox.Show("Är du säker att du vill skapa en journal", "Varning", MessageBoxButton.YesNo);
            if (isCertain == MessageBoxResult.Yes)
            {
                CreateJournal().Wait();
            }
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
