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
    public class ProductController : ControllerBase
    {
        private readonly SqlData _db;

        public ProductController(SqlData db)
        {
            _db = db;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public List<ProductModel> GetAllProducts()
        {
            var output = _db.GetAllProducts();

            return output;
        }
    }
}
