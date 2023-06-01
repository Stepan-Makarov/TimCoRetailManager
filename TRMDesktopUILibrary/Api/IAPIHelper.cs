using System.Threading.Tasks;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public interface IAPIHelper
    {
        HttpClient? ApiClient { get; }
        Task<AuthenticatedUserModel> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
    }
}