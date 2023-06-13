using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public interface ISaleEndPoint
    {
        Task PostSale(SaleUIModel sale);
    }
}