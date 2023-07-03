using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public interface IUserEndPoint
    {
        Task<List<UIProductModel>> GetAllProducts();
        Task<List<UserUIModel>> GetAllUsers();
    }
}