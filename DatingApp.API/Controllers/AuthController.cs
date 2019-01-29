using System.Threading.Tasks;
using DatingApp.API.BusinessLogics.Interfaces;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthBusinessLogic _authBL;
        public AuthController(IAuthBusinessLogic authBL)
        {
            _authBL = authBL;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (await _authBL.IsUserExists(userDto.Username))
            {
                return BadRequest("Username already exists");
            }

            User user = new User {UserName = userDto.Username};
            user = await _authBL.Register(user, userDto.Password);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            string token = await _authBL.Login(userDto.Username, userDto.Password);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}