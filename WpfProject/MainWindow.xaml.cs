using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfProject.Models;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoginButton.IsEnabled = false;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Login().Wait();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Url:n till API:et är felaktig. Var god skriv rätt! " + ex.Message, "Varning", MessageBoxButton.OK);
            }
        }
        public async Task Login()
        {
            HttpClient client = HelperMethods.GetClient(txtPassword.Password, txtUserName.Text);

            var user = new UserModel
            {
                UserName = txtUserName.Text,
                Password = txtPassword.Password
            };

            var response = client.PostAsJsonAsync("api/User/Login", user).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var apiUser = response.Content.ReadAsAsync<UserModel>().Result;

                if (apiUser.IsAdmin)
                {
                    AdminWindow awindow = new AdminWindow(apiUser);
                    awindow.Show();
                }
                else
                {
                    DriverWindow dwindow = new DriverWindow(apiUser);
                    dwindow.Show();
                }
                Close();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                MessageBox.Show("Fel inloggnings uppgifter!");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginButtonIsEnabled();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginButtonIsEnabled();
        }

        public void LoginButtonIsEnabled()
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text) || txtPassword.Password.Length == 0)
            {
                LoginButton.IsEnabled = false;
            }
            else
            {
                LoginButton.IsEnabled = true;
            }
        }
    }
}
