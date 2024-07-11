using System.Runtime.InteropServices;
using System.Web.Http;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using System.Diagnostics;

namespace Sadna_17_B_Backend.Controllers
{
    [RoutePrefix("api/RestAPI")]
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
            Trace.WriteLine("SignUp method called.");
            Trace.WriteLine($"Username: {user.Username}");
            var response = userService.CreateSubscriber(user.Username, user.Password);
            if (!response.Success)
            {
                Trace.WriteLine("Sign up failed: " + response.Message);
                return BadRequest(response.Message);
            }
            Trace.WriteLine("Sign up successful!");
            return Ok("Sign up successful!");
        }
//        [HttpPost]
//        [Route("login")]
//        public IHttpActionResult Login([FromBody] UserDto user)
//        {
    // header || from || to || body - > content
//        }

        
    }

    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
    }
}
