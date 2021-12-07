using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/Users")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo; 
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthModel model)
        {
            var user = _userRepo.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Username Or Password is Incorrect ! " });
            }

            return Ok(user);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthModel model)
        {
            bool ifUserUnique = _userRepo.IsUniqeUser(model.Username);
            if (!ifUserUnique)
            {
                return BadRequest(new { message = "Username Already Existes ! " });
            }
            var user = _userRepo.Register(model.Username, model.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Error While Registering! " });
            }
            return Ok(user);
        }
    }
}
