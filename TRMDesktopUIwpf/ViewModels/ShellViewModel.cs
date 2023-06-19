using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Events;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUIwpf.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private readonly IEventAggregator _events;
        private readonly SalesViewModel _salesVM;
        private readonly ILoggedInUserModel _user;
        private readonly IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user, IAPIHelper apiHelper)
        {
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;
            _events.SubscribeOnPublishedThread(this);
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task ExitApplication()
        {
            await TryCloseAsync();
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
            await ActivateItemAsync(_salesVM);

            NotifyOfPropertyChange(() => IsLogIn);
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
