using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public void PostSale(SaleModel sale)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _db.SaveSale(sale, userId);
        }

        // GET: api/ProductController/GetSalesReport
        [HttpGet]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            var output = _db.GetSalesReport();

            return output;
        }
    }
}
