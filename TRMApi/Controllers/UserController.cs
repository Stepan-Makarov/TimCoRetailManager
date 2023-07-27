using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(SqlData db, ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<UserController> logger)
        {
            _db = db;
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: api/<UserController>
        [HttpGet]
        public UserModel GetUserById()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var output = _db.GetUserById(userId);

            return output;
        }

        // GET: api/<UserController>/Admin/GetAllUsers
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
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

        // GET: api/<UserController>/Admin/GetAllRoles
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public List<ApplicationRoleModel> GetAllRoles()
        {
            List<ApplicationRoleModel> output = new List<ApplicationRoleModel>();

            var roles = _context.Roles.ToList();

            foreach ( var role in roles )
            {
                ApplicationRoleModel r = new ApplicationRoleModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    NormalizedName = role.NormalizedName,
                    ConcurrencyStamp = role.ConcurrencyStamp
                };

                output.Add(r);
            }

            return output;
        }

        // POST: api/<UserController>/Admin/PostRole
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pair)
        {
            string? loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FindAsync(pair.UserId);

            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
                            loggedInUserId, user.Id, pair.RoleName);

            await _userManager.AddToRoleAsync(user, pair.RoleName);
        }

        // POST: api/<UserController>/Admin/RemoveRole
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pair)
        {
            string? loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FindAsync(pair.UserId);

            _logger.LogInformation("Admin {Admin} remove user {User} from role {Role}",
                            loggedInUserId, user.Id, pair.RoleName);

            await _userManager.RemoveFromRoleAsync(user, pair.RoleName);
        }
    }
}
