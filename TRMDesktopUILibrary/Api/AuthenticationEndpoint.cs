using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public class AuthenticationEndpoint : IAuthenticationEndpoint
    {
        private readonly IAPIHelper _apiHelper;
        private readonly ILoggedInUserModel _loggedUser;

        public AuthenticationEndpoint(IAPIHelper apiHelper, ILoggedInUserModel loggedUser)
        {
            _apiHelper = apiHelper;
            _loggedUser = loggedUser;
        }
        public async Task<AuthenticatedUserModel> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                //new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsync("/Token", data);
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

        public void LogOffUser()
        {
            _apiHelper.ApiClient.DefaultRequestHeaders.Clear();
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiHelper.ApiClient.DefaultRequestHeaders.Clear();
            _apiHelper.ApiClient.DefaultRequestHeaders.Accept.Clear();
            _apiHelper.ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiHelper.ApiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User");
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
