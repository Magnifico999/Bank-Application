using BankApplication.Core.Services.Interface;
using BankApplication.Data.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankApplication.Core.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

         
        public async Task<ResponseDto<bool>> RegisterUser(RegisterUser user)
        {
            var response = new ResponseDto<bool>();

            try
            {
                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                var existingEmailUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null || existingEmailUser != null)
                {
                    response.DisplayMessage = "User with the same username or email already exists.";
                    response.StatusCode = 400;
                    response.Result = false;
                }
                else
                {
                    var identityUser = new IdentityUser
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                    };

                    var result = await _userManager.CreateAsync(identityUser, user.Password);

                    if (result.Succeeded)
                    {
                        response.DisplayMessage = "User registration successful.";
                        response.StatusCode = 200;
                        response.Result = true;
                    }
                    else
                    {
                        response.DisplayMessage = "Password format not correct.";
                        response.StatusCode = 500;
                        response.Result = false;
                    }
                }
            }
            catch (Exception)
            {
                response.DisplayMessage = "An error occurred during user registration.";
                response.StatusCode = 500;
                response.Result = false;

            }

            return response;
        }


        //public async Task<bool> Login(LoginUser user)
        //{
        //   var identityUser = await _userManager.FindByEmailAsync(user.Email);
        //    if (identityUser == null)
        //    {
        //        return false;
        //    }
        //    return await _userManager.CheckPasswordAsync(identityUser, user.Password);
        //}
        public async Task<ResponseDto<bool>> Login(LoginUser user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return new ResponseDto<bool>
                {
                    DisplayMessage = "User not found",
                    StatusCode = 404,
                    Result = false
                };
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(identityUser, user.Password);
            if (!isPasswordCorrect)
            {
                return new ResponseDto<bool>
                {
                    DisplayMessage = "Incorrect password",
                    StatusCode = 401,
                    Result = false
                };
            }

            return new ResponseDto<bool>
            {
                DisplayMessage = "Login successful",
                StatusCode = 200,
                Result = true
            };
        }

        public string GenerateTokenString(LoginUser user)
        {
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin"),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));

            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            var securityToken = new JwtSecurityToken(
                claims:claims,
                expires: DateTime.Now.AddMinutes(60),
                issuer:_config.GetSection("Jwt:Issuer").Value,
                audience:_config.GetSection("Jwt:Audience").Value,
                signingCredentials:signingCred 
                );
            string tokenstring = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenstring;
    }   }
}
