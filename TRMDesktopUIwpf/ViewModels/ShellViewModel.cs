using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Events;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUIwpf.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>, IHandle<SalesViewEvent>
    {
        private readonly IEventAggregator _events;
        private readonly ILoggedInUserModel _user;
        //private readonly UserUIModel _userEF;
        private readonly IAuthenticationEndpoint _authentication;

        public ShellViewModel(IEventAggregator events,
                              ILoggedInUserModel user,
                              //UserUIModel userEF,
                              IAuthenticationEndpoint authentication)
        {
            _events = events;
            _user = user;
            //_userEF = userEF;
            _authentication = authentication;
            _events.SubscribeOnPublishedThread(this);
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task ExitApplication()
        {
            await TryCloseAsync();
        }

        public async Task UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>());
        }

        public async Task LogOut()
        {
            _user.ResetUserModel();
            _authentication.LogOffUser();

            await ActivateItemAsync(IoC.Get<LoginViewModel>());

            NotifyOfPropertyChange(() => IsLogIn);
            NotifyOfPropertyChange(() => IsLogOut);
        }

        public async Task LogIn()
        {
            await ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);

            NotifyOfPropertyChange(() => IsLogIn);
            NotifyOfPropertyChange(() => IsLogOut);
        }

        public async Task HandleAsync(SalesViewEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
        }

        public bool IsLogIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }

                return output;
            }
        }

        public bool IsLogOut
        {
            get
            {
                return !IsLogIn;
            }
        }
    }
}
