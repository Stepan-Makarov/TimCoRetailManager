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
        private readonly IAPIHelper _apiHelper;
        private readonly UserUIModel _userEF;

        public ShellViewModel(IEventAggregator events,
                              ILoggedInUserModel user,
                              IAPIHelper apiHelper,
                              UserUIModel userEF)
        {
            _events = events;
            _user = user;
            _apiHelper = apiHelper;
            _userEF = userEF;
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
            _apiHelper.LogOffUser();

            await ActivateItemAsync(IoC.Get<LoginViewModel>());

            NotifyOfPropertyChange(() => IsLogIn);
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>());

            NotifyOfPropertyChange(() => IsLogIn);
        }

        public async Task HandleAsync(SalesViewEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>());
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
    }
}
