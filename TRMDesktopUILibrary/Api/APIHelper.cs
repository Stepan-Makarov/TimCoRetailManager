using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient? _apiClient;
        private readonly ILoggedInUserModel _loggedUser;
        private readonly IConfiguration _config;

        public APIHelper(ILoggedInUserModel loggedUser, IConfiguration config)
        {
            _config = config;
            _loggedUser = loggedUser;
            InitializeClient();
        }

        public HttpClient? ApiClient
        {
            get
            {
                return _apiClient;
            }
        }

        private void InitializeClient()
        {
            string? api = _config.GetValue<string>("api");

            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUserModel> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                //new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (HttpResponseMessage response = await _apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<AuthenticatedUserModel>();
                    return result;
                }

                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/User"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();

                    _loggedUser.Id = result.Id;
                    _loggedUser.FirstName = result.FirstName;
                    _loggedUser.LastName = result.LastName;
                    _loggedUser.EmailAddress = result.EmailAddress;
                    _loggedUser.CreateDate = result.CreateDate;
                    _loggedUser.Token = token;
                }

                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
