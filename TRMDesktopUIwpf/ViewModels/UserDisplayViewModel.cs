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
using TRMDesktopUILibrary.Events;
using TRMDesktopUILibrary.Models;
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
        private readonly IEventAggregator _events;

        public UserDisplayViewModel(IUserEndPoint userEndpoint, IConfiguration config, IMapper mapper, 
                                    StatusInfoViewModel status, IWindowManager window,
                                    IEventAggregator events)
        {
            _userEndpoint = userEndpoint;
            _config = config;
            _mapper = mapper;
            _status = status;
            _window = window;
            _events = events;
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

                await _events.PublishOnUIThreadAsync(new SalesViewEvent());
                await TryCloseAsync();
            }
        }

        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAllUsers();
            var users = _mapper.Map<List<UserDisplayModel>>(userList);
            Users = new BindingList<UserDisplayModel>(users);
        }

        private BindingList<UserDisplayModel>? _users = new();
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

        private UserDisplayModel? _selectedUser;
        public UserDisplayModel? SelectedUser
        {
            get 
            { 
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                UserRoles?.Clear();
                DisplayRoleNames();
                //TODO Make method or event for this
                LoadRoles();
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }


        private BindingList<RoleUIModel>? _userRoles = new();
        public BindingList<RoleUIModel>? UserRoles
        {
            get
            {
                return _userRoles;
            }
            set
            {
                _userRoles = value;
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private BindingList<RoleUIModel>? _avaliableRoles = new();
        public BindingList<RoleUIModel>? AvaliableRoles
        {
            get
            {
                return _avaliableRoles;
            }
            set
            {
                _avaliableRoles = value;
                NotifyOfPropertyChange(() => AvaliableRoles);
            }
        }

        private RoleUIModel? _selectedUserRole;
        public RoleUIModel? SelectedUserRole
        {
            get
            {
                return _selectedUserRole;
            }
            set
            {
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);
                NotifyOfPropertyChange(() => CanRemoveRole);
            }
        }

        private RoleUIModel? _selectedAvaliableRole;
        public RoleUIModel? SelectedAvaliableRole
        {
            get
            {
                return _selectedAvaliableRole;
            }
            set
            {
                _selectedAvaliableRole = value;
                NotifyOfPropertyChange(() => SelectedAvaliableRole);
                NotifyOfPropertyChange(() => CanAddRole);
            }
        }

        private BindingList<RoleUIModel>? DisplayRoleNames()
        {
            if (SelectedUser != null)
            {
                foreach (var role in SelectedUser.Roles)
                {
                    RoleUIModel? Role = new RoleUIModel
                    {
                        Id = role.Key,
                        Name = role.Value
                    };

                    UserRoles?.Add(Role);
                }
            }

            return UserRoles;
        }

        private async Task LoadRoles()
        {
            AvaliableRoles?.Clear();

            var roles = await _userEndpoint.GetAllRoles();

            foreach (var role in roles)
            {
                List<string>? roleNames = UserRoles.Select(x => x.Name).ToList();

                if (roleNames.Contains(role.Name) == false)
                {
                    AvaliableRoles?.Add(role);
                }
            }
        }

        public bool CanRemoveRole
        {
            get
            {
                bool output = false;

                if (SelectedUserRole != null && SelectedUser != null)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task RemoveRole()
        {
            await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole.Name);

            var roleToDelete = SelectedUser.Roles.FirstOrDefault(x => x.Value == SelectedUserRole.Name);
            SelectedUser.Roles.Remove(roleToDelete.Key);

            AvaliableRoles?.Add(SelectedUserRole);
            UserRoles?.Remove(SelectedUserRole);


            NotifyOfPropertyChange(() => Users);
            //NotifyOfPropertyChange(() => AvaliableRoles);
        }

        public bool CanAddRole
        {
            get
            {
                bool output = false;

                if (SelectedAvaliableRole != null && SelectedUser != null)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task AddRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvaliableRole.Name);

            SelectedUser.Roles.Add(SelectedAvaliableRole.Id, SelectedAvaliableRole.Name);

            UserRoles?.Add(SelectedAvaliableRole);
            AvaliableRoles?.Remove(SelectedAvaliableRole);

            NotifyOfPropertyChange(() => Users);
            //NotifyOfPropertyChange(() => AvaliableRoles);
        }
    }
}
