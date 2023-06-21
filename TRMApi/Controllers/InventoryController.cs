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
    public class InventoryController : ControllerBase
    {
        private readonly SqlData _db;

        public InventoryController(SqlData db)
        {
            _db = db;
        }

        // GET: api/InventoryController
        [HttpGet]
        public List<InventoryModel> Get()
        {
            var output = _db.GetInventory();

            return output;
        }

        // POST: api/InventoryController
        [HttpPost]
        public void Post(InventoryModel item)
        {
            _db.InsertInventory(item);
        }
    }
}
