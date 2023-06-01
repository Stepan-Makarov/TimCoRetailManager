using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public interface IProductEndpoint
    {
        Task<List<UIProductModel>> GetAllProducts();
    }
}