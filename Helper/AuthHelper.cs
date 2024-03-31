using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helper
{
    public class AuthHelper
    {

        private readonly IConfiguration _config;
        public AuthHelper(IConfiguration iconfig)
        {
            _config = iconfig;
        }
        public string CreateToken(int userId)
        {
            Claim[] claims = [
                new Claim("userId",userId.ToString()),
                ];


            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:TokenKey").Value));
            Console.WriteLine(tokenKey);
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1),
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        public byte[] GetPasswordHash(string password, byte[] salt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(salt);
            return KeyDerivation.Pbkdf2(
                       password: password,
                       salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                       prf: KeyDerivationPrf.HMACSHA256,
                       iterationCount: 1000,
                       numBytesRequested: 256 / 8
                   );


        }


    }

}