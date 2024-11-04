using BankApplication.Core.Services.Interface;
using BankApplication.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BankApplication.API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterUser user)
        {
            var registrationResponse = await _authService.RegisterUser(user);

            if (registrationResponse.Result)
            {
                var response = new ResponseDto<string>
                {
                    DisplayMessage = "User successfully registered",
                    StatusCode = 200,
                    Result = "Success"
                };

                return Ok(response);
            }

            var errorResponse = new ResponseDto<string>
            {
                DisplayMessage = registrationResponse.DisplayMessage, 
                StatusCode = registrationResponse.StatusCode,
                Result = "Failure"
            };

            return BadRequest(errorResponse);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto<bool>
                {
                    DisplayMessage = "Invalid input",
                    StatusCode = 400,
                    Result = false
                });
            }

            var loginResponse = await _authService.Login(user);

            if (loginResponse.Result)
            {
                var tokenString = _authService.GenerateTokenString(user);
                return Ok(tokenString); 
            }
            
                return BadRequest(loginResponse); 


        }   
    }
}
