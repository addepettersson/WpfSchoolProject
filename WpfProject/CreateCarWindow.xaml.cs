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
    /// Interaction logic for CreateCarWindow.xaml
    /// </summary>
    public partial class CreateCarWindow : Window
    {
        private UserModel _user;
        public CreateCarWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            GetColors().Wait();
            GetCarType().Wait();
            GetYear().Wait();
            GetFuelType().Wait();
        }
        public async Task GetColors()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);
            var response = client.GetAsync("api/Car/GetColors").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var colors = await response.Content.ReadAsStringAsync();
                List<ColorModel> colorList = JsonConvert.DeserializeObject<List<ColorModel>>(colors).ToList();
                foreach (var item in colorList)
                {
                    cmbColour.Items.Add(new ColorModel { Color = item.Color, Id = item.Id });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        public async Task GetCarType()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Car/GetCarType").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var cartype = await response.Content.ReadAsStringAsync();
                List<CarType> carTypeList = JsonConvert.DeserializeObject<List<CarType>>(cartype).ToList();
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
        public async Task GetYear()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);
            var response = client.GetAsync("api/Car/GetYearToShow").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var years = await response.Content.ReadAsStringAsync();
                List<YearModel> yearList = JsonConvert.DeserializeObject<List<YearModel>>(years).ToList();
                foreach (var item in yearList)
                {
                    cmbYear.Items.Add(new YearModel { Year = item.Year, Id = item.Id });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        public async Task GetFuelType()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Car/GetFuelTypeToShow").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var fuelTypes = await response.Content.ReadAsStringAsync();
                List<FuelTypeModel> fuelTypeList = JsonConvert.DeserializeObject<List<FuelTypeModel>>(fuelTypes).ToList();
                foreach (var item in fuelTypeList)
                {
                    cmbFuelType.Items.Add(new FuelTypeModel { FuelType = item.FuelType, Id = item.Id });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        public async Task CreateCar()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var selectedColour = cmbColour.SelectedItem as ColorModel;
            var selectedCarType = cmbCarType.SelectedItem as CarType;
            var selectedFuelType = cmbFuelType.SelectedItem as FuelTypeModel;
            var selectedYear = cmbYear.SelectedItem as YearModel;


            Regex regexObjRegNr = new Regex(@"^[A-Z]{3}\d{3}$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var foundMatch = regexObjRegNr.IsMatch(txtRegNr.Text);

            Regex regexKm = new Regex(@"[\d]");
            var foundmatchKm = regexKm.IsMatch(txtKm.Text);

            if (selectedFuelType == null || (selectedCarType == null || (foundMatch != true ||
                string.IsNullOrWhiteSpace(txtRegNr.Text) || string.IsNullOrWhiteSpace(txtKm.Text)) || foundmatchKm != true))
            {
                MessageBox.Show("Något obligatoriskt värde är ej ifyllt eller fel format på!");
            }
            else
            {
                var car = new CarModel
                {
                    Regnr = txtRegNr.Text,
                    OriginalMileage = Convert.ToInt32(txtKm.Text),
                    CarType_Id = selectedCarType.Id,
                    FuelType_Id = selectedFuelType.Id,
                    Description = txtDescript.Text,
                };
                if (selectedColour != null)
                {
                    car.Colour_Id = selectedColour.Id;
                }
                if (selectedYear != null)
                {
                    car.Year_Id = selectedYear.Id;
                }

                var response = client.PostAsJsonAsync("api/Car/CreateNewCar", car).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show("Bilen är skapad!");

                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Registreringsnumret finns redan!");
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("Servern ligger nere för tillfället!");
                }
            }
        }
        private void btnCreateCar_Click(object sender, RoutedEventArgs e)
        {
            var isCertain = MessageBox.Show("Är du säker att du vill skapa denna bil?", "Varning", MessageBoxButton.YesNo);
            if (isCertain == MessageBoxResult.Yes)
            {
                CreateCar().Wait();
            }
        }
    }
}
