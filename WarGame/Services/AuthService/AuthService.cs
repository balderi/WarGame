using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WarGame.Data;

namespace WarGame.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<LoginResultDto>> LoginAsync(UserLoginDto userLogin)
        {
            var response = new ServiceResponse<LoginResultDto>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == userLogin.UserName.ToLower());
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            var data = await _context.UserData.Where(d => d.UserId == user.Id).FirstOrDefaultAsync();
            if (data == null)
            {
                response.Success = false;
                response.Message = "User data not found.";
                return response;
            }

            if (!VerifyPasswordHash(userLogin.Password, data.PasswordHash, data.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
                return response;
            }
            var token = CreateToken(user);
            response.Data = new LoginResultDto
            {
                Id = user.Id,
                Token = token
            };
            data.Token = token;
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<ServiceResponse<Guid>> RegisterAsync(UserRegisterDto userRegister)
        {
            if (await UserExists(userRegister.UserName))
            {
                return new ServiceResponse<Guid>
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            var user = new User { UserName = userRegister.UserName, DateCreated = DateTime.UtcNow };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            UserData data = new() { UserId = user.Id, Email = userRegister.Email };

            CreatePasswordHash(userRegister.Password, out byte[] passwordHash, out byte[] passwordSalt);

            data.PasswordHash = passwordHash;
            data.PasswordSalt = passwordSalt;

            _context.UserData.Add(data);

            await _context.SaveChangesAsync();

            return new ServiceResponse<Guid> { Data = user.Id, Message = "Registration successful!" };
        }

        public async Task<bool> UserExists(string name)
        {
            if (await _context.Users.AnyAsync(u => u.UserName.ToLower() == name.ToLower()))
            {
                return true;
            }

            return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(Guid userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "User not found."
                };
            }
            var data = await _context.UserData.Where(d => d.UserId == userId).FirstOrDefaultAsync();

            if (data == null)
            {
                throw new Exception("User data not found.");
            }

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            data.PasswordHash = passwordHash;
            data.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true, Message = "Password has been changed" };
        }
    }
}
