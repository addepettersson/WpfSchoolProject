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
    /// Interaction logic for CreateCostOnCarWindow.xaml
    /// </summary>
    public partial class CreateCostOnCarWindow : Window
    {
        private readonly UserModel _user;
        public CreateCostOnCarWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            GetCars().Wait();
            GetTypeOfCost().Wait();
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
        public async Task GetTypeOfCost()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Car/GetCostType").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var costType = await response.Content.ReadAsStringAsync();
                List<TypeOfCostModel> costTypeList = JsonConvert.DeserializeObject<List<TypeOfCostModel>>(costType).ToList();
                foreach (var item in costTypeList)
                {
                    cmbCostType.Items.Add(new TypeOfCostModel { Type = item.Type, Id = item.Id });
                }
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }
        private void btnCreateCost_Click(object sender, RoutedEventArgs e)
        {
            var isCertain = MessageBox.Show("Är du säker att du vill skapa en kostnad?", "Varning", MessageBoxButton.YesNo);
            if (isCertain == MessageBoxResult.Yes)
            {
                CreateCost().Wait();
            }
        }

        public async Task CreateCost()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var selectedCar = cmbCars.SelectedItem as CarModel;
            var selectedCost = cmbCostType.SelectedItem as TypeOfCostModel;
            Regex regexCost = new Regex(@"^\d{1,14}(?:\.\d{1,2}){0,1}$");
            var foundmatchCost = regexCost.IsMatch(txtCost.Text);

            if (foundmatchCost && (selectedCar != null && selectedCost != null) && !string.IsNullOrWhiteSpace(datepicker.Text))
            {
                var cost = new CostModel
                {
                    CarId = selectedCar.Id,
                    Datepicker = Convert.ToDateTime(datepicker.Text),
                    TypeOfCost = selectedCost.Id,
                    Cost = Convert.ToDecimal(txtCost.Text),
                    Comment = txtComment.Text,
                };

                var response = client.PostAsJsonAsync("api/Car/CreateNewCost", cost).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    MessageBox.Show("Kostnaden är skapad!");
                }                
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Du har redan lagt till denna typ av kostnad på denna bil " + cost.Datepicker);
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("Uppkoppling till servern misslyckades!");
                }
            }
            else
            {
                MessageBox.Show("Något värde är inte ifyllt eller har felaktigt format!");
            }
            
        }
    }
}
