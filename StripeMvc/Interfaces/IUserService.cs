using StripeMvc.Models.Dtos.UserDtos;

namespace StripeMvc.Interfaces
{
    public interface IUserService
    {
        Task<string> GetStripeCustomerIdByEmail(string email);
        Task UpdateUserStripeCustomerId(UserInfroDto userInfro);
    }
}
