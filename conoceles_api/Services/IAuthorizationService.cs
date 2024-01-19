using conoceles_api.DTOs;

namespace conoceles_api.Services
{
    public interface IAuthorizationService
    {
        Task<AppUserAuthDTO> ValidateUser(AppUserDTO dto);
    }
}