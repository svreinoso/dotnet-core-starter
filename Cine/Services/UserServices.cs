using Cine.Dto;
using Cine.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cine.Services
{
    public interface IUserServices
    {
        Task<ResponseDto> AddUserAsync(UserDto userDto, string role);
    }

    public class UserServices : IUserServices
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public UserServices(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResponseDto> AddUserAsync (UserDto userDto, string role)
        {
            var user = new ApplicationUser
            {
                Email = userDto.Email,
                UserName = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return new ResponseDto { Success = true };
            }

            return new ResponseDto { Success = false, Message = result.Errors.FirstOrDefault()?.Description };
        }
    }
}
