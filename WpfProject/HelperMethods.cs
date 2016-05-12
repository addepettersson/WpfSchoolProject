using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WpfProject
{
    public static class HelperMethods
    {
        public static HttpClient GetClient(string password, string username)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiKey"]);

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            string credentials = username + ":" + password;
            var credentialBytes = Encoding.UTF8.GetBytes(credentials);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentialBytes));

            return client;
        }
    }
}
