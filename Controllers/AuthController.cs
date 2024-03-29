using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controller
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto user)
        {
            if (user.Password == user.PassowrdConfirm)
            {
                string sqlCheckUserExist = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email =  '" + user.Email + "'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExist);
                if (!existingUsers.Any())
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = GetPasswordHash(user.Password, passwordSalt);

                    string sql = @"INSERT INTO TutorialAppSchema.Auth(Email,PasswordHash,PasswordSalt) VALUES('"
                    + user.Email + "', @passwordHash, @passwordSalt);";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    SqlParameter passwordHashSqlParameter = new SqlParameter("@passwordHash", SqlDbType.Binary);
                    passwordHashSqlParameter.Value = passwordHash;

                    SqlParameter passwordSaltSqlParameter = new SqlParameter("@passwordSalt", SqlDbType.Binary);
                    passwordSaltSqlParameter.Value = passwordSalt;

                    sqlParameters.Add(passwordHashSqlParameter);
                    sqlParameters.Add(passwordSaltSqlParameter);

                    if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
                    {
                        return Ok();
                    }
                    else
                    {
                        throw new Exception("Failed to register");
                    }
                }
                throw new Exception("User already exist");
            }
            throw new Exception("Password do not match");
        }

        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto user)
        {
            string sqlForHashSalt = @"SELECT [PasswordHash],[PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email='" + user.Email + "'";

            Console.WriteLine(sqlForHashSalt);
            
            UserForLoginConfirmation userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmation>(sqlForHashSalt);
            Console.WriteLine(userForLoginConfirmation.PasswordSalt.Length);
            Console.WriteLine(userForLoginConfirmation.PasswordHash.Length);

            byte[] passwordHash = GetPasswordHash(user.Password, userForLoginConfirmation.PasswordSalt);
            foreach (var item in passwordHash)
            {
                Console.Write(item);
            }

            Console.WriteLine();

            foreach (var item in userForLoginConfirmation.PasswordHash)
            {
                Console.Write(item);
            }

            Console.WriteLine();

            if(passwordHash.Length!=userForLoginConfirmation.PasswordHash.Length)
                return StatusCode(401,"Invalid Password");
            for(int i=0;i< passwordHash.Length;i++)
            {
                if(passwordHash[i]!=userForLoginConfirmation.PasswordHash[i])
                    return StatusCode(401,"Invalid Password");
            }
            return Ok();
        }

        private byte[] GetPasswordHash(string password, byte[] salt)
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