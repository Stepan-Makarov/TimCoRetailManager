using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRMDataManagerLibrary.Data;
using TRMDataManagerLibrary.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly SqlData _db;

        public SaleController(SqlData db)
        {
            _db = db;
        }

        [HttpPost]
        [Authorize(Roles = "Cashier")]
        public void PostSale(SaleModel sale)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _db.SaveSale(sale, userId);
        }

        // GET: api/Sale/GetSalesReport
        [HttpGet]
        [Route("GetSalesReport")]
        [Authorize(Roles = "Admin,Manager")]
        public List<SaleReportModel> GetSalesReport()
        {
            var output = _db.GetSalesReport();

            return output;
        }

        // GET: api/Sale/GetTaxRate
        [HttpGet]
        [Route("GetTaxRate")]
        [AllowAnonymous]
        public decimal GetTaxRate()
        {
            var output = _db.GetTaxRate();

            return output;
        }
    }
}
