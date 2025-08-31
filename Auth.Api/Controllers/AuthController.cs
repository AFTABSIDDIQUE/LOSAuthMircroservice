using Auth.Application.Repository;
using Auth.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        ILogin login;
        public AuthController(ILogin login)
        {
            this.login = login;
        }

        [HttpPost]
        [Route("FetchLogin")]
        public IActionResult FetchLogin(Login data)
        {

            var validate = login.Logins(data);
            if (validate != null)
            {
                return Ok(new { token = validate });
            }
            else
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }


        }
    }
}
