using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public interface ISaleEndPoint
    {
        Task<decimal> GetTaxRate();
        Task PostSale(SaleUIModel sale);
    }
}