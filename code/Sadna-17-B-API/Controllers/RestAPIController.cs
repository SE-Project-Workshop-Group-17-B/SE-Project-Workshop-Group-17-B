using Microsoft.AspNetCore.Mvc;
using Sadna_17_B_API.Models;
using Sadna_17_B;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;

namespace Sadna_17_B_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestAPIController : ControllerBase
    {
        private readonly IUserService _userService;

        public RestAPIController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] UserDto userDto)
        {
            // Validate userDto (consider adding data annotations to your UserDto)

            var response = _userService.CreateSubscriber(userDto.Username, userDto.Password);

            if (response.Success)
            {
                return Ok("Sign up successful!");
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}