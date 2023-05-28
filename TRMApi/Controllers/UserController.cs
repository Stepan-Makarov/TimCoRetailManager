using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRMDataManagerLibrary.Data;
using TRMDataManagerLibrary.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly SqlData _db;

        public UserController(SqlData db)
        {
            _db = db;
        }

        // GET: api/<UserController>
        [HttpGet]
        public UserModel GetUserById()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var output = _db.GetUserById(userId);

            return output;
        }
    }
}
