using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTfulAPI_Technical_Task_.Composable;
using RESTfulAPI_Technical_Task_.Model;
using Serilog;

namespace RESTfulAPI_Technical_Task_.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request.Username != "admin" || request.Password != "password") // Заменить на проверку в БД
                    return Unauthorized("Invalid credentials");

                var token = await _jwtService.GenerateTokenAsync(request.Username);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при попытке входа");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}
