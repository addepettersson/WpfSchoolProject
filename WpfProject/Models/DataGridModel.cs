using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProject.Models
{
    public class DataGridModel
    {
        public int CarId { get; set; }
        public string Regnr { get; set; }
        public string TypeOfCar { get; set; }
        public int Mileage { get; set; }
        public decimal Consumation { get; set; }
        public decimal TotalFuelPrice { get; set; }
        public string FuelType { get; set; }
    }
}
