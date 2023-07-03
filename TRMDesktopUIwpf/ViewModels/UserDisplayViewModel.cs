using AutoMapper;
using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUILibrary.Api;
using TRMDesktopUIwpf.Models;

namespace TRMDesktopUIwpf.ViewModels
{
    internal class UserDisplayViewModel : Screen
    {
        private readonly IUserEndPoint _userEndpoint;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        private BindingList<UserDisplayModel>? _users;

        public BindingList<UserDisplayModel>? Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value; 
                NotifyOfPropertyChange(() => Users);
            }
        }


        public UserDisplayViewModel(IUserEndPoint userEndpoint, IConfiguration config, IMapper mapper, 
                                    StatusInfoViewModel status, IWindowManager window)
        {
            _userEndpoint = userEndpoint;
            _config = config;
            _mapper = mapper;
            _status = status;
            _window = window;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Forbidden")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to see roles. It is for admins");
                    await _window.ShowDialogAsync(_status, null, settings);
                }

                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                }

                await TryCloseAsync();
            }
        }

        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAllUsers();
            var users = _mapper.Map<List<UserDisplayModel>>(userList);
            Users = new BindingList<UserDisplayModel>(users);
        }
    }
}
