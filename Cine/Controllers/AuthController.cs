using Cine.Data;
using Cine.Dto;
using Cine.Models;
using Cine.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUserServices _userServices;

        public AuthController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, IUserServices userServices)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _userServices = userServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return new BadRequestObjectResult(new ResponseDto { Success = false, Message = "Email does not exist" });
            var passwordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordCorrect) return new BadRequestObjectResult(new ResponseDto { Message = "Wrong Password" });

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roles[0])
            };

            var SignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));

            var token = new JwtSecurityToken(
                    issuer: _configuration["JwtIssuer"],
                    audience: _configuration["JwtIssuer"],
                    expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtExpireDays"])),
                    claims: claims,
                    signingCredentials: new SigningCredentials(SignKey, SecurityAlgorithms.HmacSha256)
                );

            var response = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expire = token.ValidTo,
                Role = roles[0],
                User = new
                {
                    user.Email,
                    user.Id
                }
            };

            return Ok(response);

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var response = await _userServices.AddUserAsync(userDto, AppRoles.User);
            if (response.Success) return new OkObjectResult(response);
            return new BadRequestObjectResult(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("AddAdmin")]
        public async Task<IActionResult> AddAdmin(UserDto userDto)
        {
            var response = await _userServices.AddUserAsync(userDto, AppRoles.Admin);
            if (response.Success) return new OkObjectResult(response);
            return new BadRequestObjectResult(response);
        }

    }
}
