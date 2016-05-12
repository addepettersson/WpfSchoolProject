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
using WpfProject.Models;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        private UserModel _user;
        public CreateUserWindow(UserModel user)
        {
            InitializeComponent();
            _user = user;
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            var isCertain = MessageBox.Show("Är du säker att du vill skapa en ny användare?", "Är du säker?", MessageBoxButton.YesNo);
            if (isCertain == MessageBoxResult.Yes)
            {
                Register().Wait();
            }
        }

        public async Task Register()
        {
            HttpClient client = HelperMethods.GetClient(_user.Password, _user.UserName);

            var user = new UserModel
            {
                UserName = txtUserName.Text,
                Password = txtPassword.Password,
                IsAdmin = (bool)chbIsAdmin.IsChecked

            };

            var response = client.PostAsJsonAsync("api/User/Register", user).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                MessageBox.Show("Användaren är skapad");
                txtUserName.Text = "";
                txtPassword.Password = "";
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                MessageBox.Show("Användarnamnet finns redan!");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("Uppkoppling till servern misslyckades!");
            }
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CreateUserButtonEnabled();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CreateUserButtonEnabled();
        }

        public void CreateUserButtonEnabled()
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text) || txtPassword.Password.Length == 0)
            {
                CreateUser.IsEnabled = false;
            }
            else
            {
                CreateUser.IsEnabled = true;
            }
        }
    }
}
