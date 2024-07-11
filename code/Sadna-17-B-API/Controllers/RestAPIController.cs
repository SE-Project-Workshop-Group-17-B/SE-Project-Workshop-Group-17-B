using Microsoft.AspNetCore.Mvc;
using Sadna_17_B_API.Models;
using Sadna_17_B;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using Sadna_17_B.Utils;

namespace Sadna_17_B_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestAPIController : ControllerBase
    {
        private readonly ServiceFactory _serviceFactory;
        private readonly IUserService _userService;
        private readonly StoreService _storeService;

        public RestAPIController(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _userService = serviceFactory.UserService;
            _storeService = serviceFactory.StoreService;
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] UserDto userDto)
        {
            // Validate userDto (consider adding data annotations to your UserDto)

            var response = _userService.CreateSubscriber(userDto.Username, userDto.Password);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto userDto)
        {
            // Validate userDto

            var response = _userService.Login(userDto.Username, userDto.Password);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        [HttpGet("get_stores")]
        public Response get_stores()
        {
            var response = _storeService.all_stores();
            return response;
        }
    }
}