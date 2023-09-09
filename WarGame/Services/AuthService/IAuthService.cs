namespace WarGame.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<Guid>> RegisterAsync(UserRegisterDto userRegister);
        Task<bool> UserExists(string userName);
        Task<ServiceResponse<LoginResultDto>> LoginAsync(UserLoginDto userLogin);
        Task<ServiceResponse<bool>> ChangePasswordAsync(Guid userId, string newPassword);
    }
}
