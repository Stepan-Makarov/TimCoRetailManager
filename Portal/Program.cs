using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Portal;
using Portal.Authentication;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddSingleton<IAPIHelper, APIHelper>();
builder.Services.AddSingleton<ILoggedInUserModel, LoggedInUserModel>();

builder.Services.AddTransient<IProductEndpoint, ProductEndpoint>();
builder.Services.AddTransient<IUserEndPoint, UserEndPoint>();
builder.Services.AddTransient<ISaleEndPoint, SaleEndPoint>();
builder.Services.AddTransient<IAuthenticationEndpoint, AuthenticationEndpoint>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
