using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StripeMvc.Data;
using StripeMvc.Interfaces;
using StripeMvc.Models;
using StripeMvc.Models.Dtos.UserDtos;

namespace StripeMvc.Services
{
    public class UserService : IUserService
    {
        private readonly  UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task UpdateUserStripeCustomerId(UserInfroDto userInfro)
        {
            var currentUser =
                  await _userManager.FindByEmailAsync(userInfro.Email);
                

            if (currentUser != null)
            {

                currentUser.StripeCustomerId = userInfro.StripeCustomerId;
                await _userManager.UpdateAsync(currentUser);

            }

        }


        public async Task<string> GetStripeCustomerIdByEmail(string email)
        {
            var currentUser =
                  await _userManager.FindByEmailAsync(email);
                                   

               return currentUser?.StripeCustomerId;
                      

        }
    }
}
