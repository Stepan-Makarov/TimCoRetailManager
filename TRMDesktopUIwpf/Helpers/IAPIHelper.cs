using System.Threading.Tasks;
using TRMDesktopUIwpf.Models;

namespace TRMDesktopUIwpf.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}