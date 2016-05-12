using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for AppConfigWindow.xaml
    /// </summary>
    public partial class AppConfigWindow : Window
    {
        public AppConfigWindow()
        {
            InitializeComponent();
        }
        public void UpdateConfigKey(string strKey, string newValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");

            if (!ConfigKeyExists(strKey))
            {
                throw new ArgumentNullException("Key", "<" + strKey + "> not find in the configuration.");
            }
            XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    childNode.Attributes["value"].Value = newValue;
            }
            xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");
            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            MessageBox.Show("Adressen har uppdaterats!");
        }
        public bool ConfigKeyExists(string strKey)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");

            XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    return true;
            }
            return false;
        }

        private void btnCreateNewApi_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewApiString.Text))
            {
                var response = MessageBox.Show("Är du säker att du vill ändra?", "Varning", MessageBoxButton.YesNo);
                if (response == MessageBoxResult.Yes)
                {
                    UpdateConfigKey("ApiKey", txtNewApiString.Text);
                    txtNewApiString.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Skriv in ett värde");
            }
        }
    }
}
