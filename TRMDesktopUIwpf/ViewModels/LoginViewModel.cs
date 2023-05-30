using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Events;

namespace TRMDesktopUIwpf.ViewModels
{
    public class LoginViewModel : Screen
    {
        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        private string? _userName;
        private string? _password;
        private readonly IAPIHelper _apiHelper;
        private readonly IEventAggregator _events;

        public string? UserName
		{
			get { return _userName; }
			set 
			{ 
				_userName = value; 
				NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
		}

        public string? Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorMessageVisible
        {
            get 
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                } 

                return output;
            }
        }

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorMessageVisible);
            }
        }


        public bool CanLogIn
        {
            get
            {
                bool output = false;

                if (UserName?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";

                var result = await _apiHelper.Authenticate(UserName, Password);

                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                await _events.PublishOnUIThreadAsync(new LogOnEvent());
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
            }
            
        }
    }
}
