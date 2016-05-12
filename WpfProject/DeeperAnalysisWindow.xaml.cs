using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// Interaction logic for DeeperAnalysisWindow.xaml
    /// </summary>
    public partial class DeeperAnalysisWindow : Window
    {
        private readonly UserModel _user;
        public DeeperAnalysisWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
            FillDataGrid().Wait();
        }

        public async Task FillDataGrid()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var response = client.GetAsync("api/Analysis/FillDataGrid").Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var journals = await response.Content.ReadAsStringAsync();
                List<DataGridModel> journalList = JsonConvert.DeserializeObject<List<DataGridModel>>(journals);
                DataGridCars.ItemsSource = journalList;
                DataGridCars.IsReadOnly = true;
                DataGridCars.CanUserSortColumns = false;
            }
        }

        public void GetAnalysis()
        {
            decimal consumation = 0;
            int totalMilage = 0;
            decimal fuelPrice = 0;
            var selectedItems = DataGridCars.SelectedItems;
            var checkfueltype = new List<DataGridModel>();
            var individCarList = new List<DataGridModel>();
            int count = 0;
            if (selectedItems != null)
            {
                foreach (var item in selectedItems)
                {

                    var car = item as DataGridModel;
                    int carMileage = car.Mileage;
                    decimal carFuelPrice = car.TotalFuelPrice;
                    foreach (var fueltype in checkfueltype)
                    {
                        if (fueltype.FuelType != car.FuelType)
                        {
                            MessageBox.Show("Kan inte räkna ut pågrund av olika drivmedel");
                            lblAverageConsumaiton.Content = "";
                            lblFuelPriceOnKM.Content = "";
                            lblTotalFuelPrice.Content = "";
                            lblTotalMileage.Content = "";
                            count++;
                            break;
                        }
                    }
                    checkfueltype.Add(car);
                    if (count == 0)
                    {
                        consumation += car.Consumation;
                        totalMilage += car.Mileage;
                        fuelPrice += car.TotalFuelPrice;

                        DataGridModel gridModelToAdd = new DataGridModel
                        {
                            Regnr = car.Regnr,
                            Mileage = carMileage,
                            TotalFuelPrice = carFuelPrice
                        };
                        individCarList.Add(gridModelToAdd);
                    }
                }
                if (count == 0)
                {
                    var totalconsumation = consumation / DataGridCars.SelectedItems.Count;
                    var fuelPriceOnKM = fuelPrice / totalMilage;

                    lblAverageConsumaiton.Content = "Snittförbrukning på valda fordon: " + totalconsumation.ToString("n2");
                    lblFuelPriceOnKM.Content = "Bensinpris per KM: " + fuelPriceOnKM.ToString("n2");
                    lblTotalFuelPrice.Content = "Total bensinpris för valda fordon: " + fuelPrice.ToString("n2");
                    lblTotalMileage.Content = "Total körsträcka för valda fordon: " + totalMilage;
                    DataGridEachCar.ItemsSource = individCarList;
                }
            }
        }

        private void DataGridCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAnalysis();
        }
    }
}
