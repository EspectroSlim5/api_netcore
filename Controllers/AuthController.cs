using Microsoft.AspNetCore.Mvc;
using static api_netcore.DTOs.AuthDTO;
using api_netcore.Security;

namespace api_netcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Cambiado de [auth] a api/[controller]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private string usuario = "admin";
        private string password = "admin";

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (req.Username != usuario || req.Password != password)
            {
                return Unauthorized("Credenciales incorrectas");
            }

            var (token, expires) = JwtTokenGenerador.CreateToken(req.Username, "admin", _config);
            return Ok(new AuthResponse { Token = token, ExpiresAt = expires });
        }
    }
}