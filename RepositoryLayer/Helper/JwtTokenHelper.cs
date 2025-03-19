using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Helper
{
    public class JwtTokenHelper
    {
        private readonly IConfiguration _configuration;
        public JwtTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserEntity user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object is null.");
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            if (jwtSettings == null)
            {
                throw new Exception("JWT settings are missing in configuration.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId",user.Id.ToString()),
                new Claim("userName", user.UserName),
                new Claim("email", user.Email),
                new Claim ("role",user.Role)
            };

            var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateResetToken(int userId, string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:ResetSecret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim("email", email)
            };

            var token = new JwtSecurityToken(
                issuer: "AddressBook",
                audience: "AddressBookUser",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public int ResetPassword(string token, ResetPasswordRequest model)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:ResetSecret"]));
            var handler = new JwtSecurityTokenHandler();
            var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "AddressBook",
                ValidAudience = "AddressBookUser",
                IssuerSigningKey = key
            },
            out SecurityToken validatedToken);
            // Extract userId from claims
            var userIdClaim = claimsPrincipal.FindFirstValue("userId");

            if (userIdClaim == null)
            {
                throw new SecurityTokenException("Invalid token: userId claim missing.");
            }
            return int.Parse(userIdClaim);
        }
    }
}
