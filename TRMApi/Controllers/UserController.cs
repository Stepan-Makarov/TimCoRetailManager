using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRMApi.Data;
using TRMApi.Models;
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
        private readonly ApplicationDbContext _context;

        public UserController(SqlData db, ApplicationDbContext context)
        {
            _db = db;
            _context = context;
        }

        // GET: api/<UserController>
        [HttpGet]
        public UserModel GetUserById()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var output = _db.GetUserById(userId);

            return output;
        }

        // GET: api/<UserController>/Admin/GetAll
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAll")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            foreach ( var user in users )
            {
                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };

                u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(key => key.RoleId, val => val.Name);

                output.Add(u);
            }

            return output;
        }
    }
}
