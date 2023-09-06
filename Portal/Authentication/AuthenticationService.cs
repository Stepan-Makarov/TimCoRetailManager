﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Portal.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace Portal.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(HttpClient client,
                                     AuthenticationStateProvider authStateProvider,
                                     ILocalStorageService localStorage)
        {
            _client = client;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                //new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userForAuthentication.Email),
                new KeyValuePair<string, string>("password", userForAuthentication.Password)
            });

            var authResult = await _client.PostAsync("https://localhost:7296/Token", data);
            //var content = await authResult.Content.ReadAsStringAsync();

            if (authResult.IsSuccessStatusCode)
            {
                var authContent = await authResult.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(
                    authContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await _localStorage.SetItemAsync("authToken", result.Access_Token);

                ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Access_Token);

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Access_Token);

                return result;
            }

            else
            {
                return null;
                //throw new Exception(authResult.ReasonPhrase);
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
