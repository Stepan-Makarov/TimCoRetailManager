using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using TRMDesktopUILibrary.Api;

namespace Portal.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _config;
        private readonly IAuthenticationEndpoint _authenticationEndpoint;
        private readonly AuthenticationState _anonymous;
        private readonly string? _authTokenStorageKey;

        public AuthStateProvider(HttpClient client,
                                 ILocalStorageService localStorage,
                                 IConfiguration config,
                                 IAuthenticationEndpoint authenticationEndpoint)
        {
            _client = client;
            _localStorage = localStorage;
            _config = config;
            _authenticationEndpoint = authenticationEndpoint;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            _authTokenStorageKey = _config["authTokenStorageKey"];
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(_authTokenStorageKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            bool isUserAuthenticated = await IsAuthenticated(token);

            if (isUserAuthenticated == false)
            {
                return _anonymous;
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }

        public async Task NotifyUserAuthentication(string token)
        {
            try
            {
                await _authenticationEndpoint.GetLoggedInUserInfo(token);

                var authenticatedUser = new ClaimsPrincipal(
                        new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));

                var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
                NotifyAuthenticationStateChanged(authState);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                await NotifyUserLogout();
            }
        }

        public async Task NotifyUserLogout()
        {
            _authenticationEndpoint.LogOffUser();
            await _localStorage.RemoveItemAsync(_authTokenStorageKey);
            var authState = Task.FromResult(_anonymous);
            _client.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<bool> IsAuthenticated(string token)
        {
            bool output = false;
            try
            {
                await _authenticationEndpoint.GetLoggedInUserInfo(token);
                output = true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                await NotifyUserLogout();
                output = false;
            }
            return output;
        }
    }
}
