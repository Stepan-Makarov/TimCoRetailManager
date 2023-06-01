using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Models;
using TRMDesktopUIwpf.Helpers;
using TRMDesktopUIwpf.ViewModels;

namespace TRMDesktopUIwpf
{
    public class BootStrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();
        public BootStrapper()
        {
            Initialize();

            //Things for PasswordBoxHelper
            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        private IConfiguration AddConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        protected override void Configure()
        {
            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IAPIHelper, APIHelper>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>();

            _container.RegisterInstance(typeof(IConfiguration), "IConfiguration", AddConfiguration());

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
