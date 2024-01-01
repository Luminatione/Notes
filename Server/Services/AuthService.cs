using Common;
using Common.DTO;
using Common.Model;
using Common.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Server.DTO;
using Server.Validators;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataBaseContext _context;
        private readonly IValidator<RegisterDto> _registerDtoValidator;

        public AuthService(DataBaseContext context, IValidator<RegisterDto> registerDtoValidator)
        {
            _context = context;
            _registerDtoValidator = registerDtoValidator;
        }

        public async Task<Response<ClaimsPrincipal>> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if(user == null)
            {
                return new Response<ClaimsPrincipal> { IsSuccess = false, Message = "Failed to login", Value = null };
            }
            if(!user.PasswordHash.SequenceEqual(HashPassword(password, user.PasswordSalt)))
            {
                return new Response<ClaimsPrincipal> { IsSuccess = false, Message = "Failed to login", Value = null };
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            return new Response<ClaimsPrincipal> { IsSuccess = true, Message = "User found", Value = principal };
        }

        public async Task<Response> Register(RegisterDto registerDto)
        {
            var validationResult = _registerDtoValidator.Validate(registerDto);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(registerDto.Password, salt);
            await _context.Users.AddAsync(new User { Username = registerDto.Username, PasswordHash = hashedPassword, PasswordSalt = salt});
            await _context.SaveChangesAsync();

            return new Response { IsSuccess = true, Message = "User registered successfully" };
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hashedBytes = pbkdf2.GetBytes(32);
                return hashedBytes;
            }
        }
    }
}
