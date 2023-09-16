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
    }
}
