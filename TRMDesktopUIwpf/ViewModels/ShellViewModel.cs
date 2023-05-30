using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Events;

namespace TRMDesktopUIwpf.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private readonly IEventAggregator _events;
        private readonly SimpleContainer _container;
        private readonly SalesViewModel _salesVM;

        public ShellViewModel(IEventAggregator events, SimpleContainer container, SalesViewModel salesVM)
        {
            _container = container;
            _events = events;
            _salesVM = salesVM;
            _events.SubscribeOnPublishedThread(this);
            ActivateItemAsync(_container.GetInstance<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
        }
    }
}
