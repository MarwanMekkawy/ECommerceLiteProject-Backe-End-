using IdentityService.Application.DTOs.AuthDTOs;

namespace IdentityService.Application.UseCases.Auth.RegisterUser
{
    public interface IRegisterUserUseCase
    {
        Task RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken);
    }
}