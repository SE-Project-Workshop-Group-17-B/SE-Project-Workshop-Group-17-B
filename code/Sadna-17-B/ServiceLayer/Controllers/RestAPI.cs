using System.Runtime.InteropServices;
using System.Web.Http;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;

namespace Sadna_17_B_Backend.Controllers
{
    [RoutePrefix("api/user")]
    public class RestAPI : ApiController
    {
        private readonly IUserService userService;

        public RestAPI(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        [Route("signup")]
        public IHttpActionResult SignUp([FromBody] UserDto user)
        {
            var response = userService.CreateSubscriber(user.Username, user.Password);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok("Sign up successful!");
        }
        
    }

    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
