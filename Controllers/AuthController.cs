using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper; 
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
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

                    byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, passwordSalt);

                    string sqlAuth = @"INSERT INTO TutorialAppSchema.Auth(Email,PasswordHash,PasswordSalt) VALUES('"
                    + user.Email + "', @passwordHash, @passwordSalt);";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    SqlParameter passwordHashSqlParameter = new SqlParameter("@passwordHash", SqlDbType.Binary);
                    passwordHashSqlParameter.Value = passwordHash;

                    SqlParameter passwordSaltSqlParameter = new SqlParameter("@passwordSalt", SqlDbType.Binary);
                    passwordSaltSqlParameter.Value = passwordSalt;

                    sqlParameters.Add(passwordHashSqlParameter);
                    sqlParameters.Add(passwordSaltSqlParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAuth, sqlParameters))
                    {
                        string sql = @"INSERT INTO TutorialAppSchema.Users(
                                        FirstName,
                                        LastName,
                                        Email,
                                        Gender,
                                        Active
                                    ) VALUES('"
                                    + user.FirstName
                                    + "','" + user.LastName
                                    + "','" + user.Email
                                    + "','" + user.Gender
                                    +  "' , 1)";

                        if (_dapper.ExecuteSql(sql))
                        {
                            return Ok();
                        }
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
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto user)
        {
            string sqlForHashSalt = @"SELECT [PasswordHash],[PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email='" + user.Email + "'";

            UserForLoginConfirmation userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmation>(sqlForHashSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, userForLoginConfirmation.PasswordSalt);

            if (passwordHash.Length != userForLoginConfirmation.PasswordHash.Length)
                return StatusCode(401, "Invalid Password");
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                    return StatusCode(401, "Invalid Password");
            }

            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email='" + user.Email + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string,string>{
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId='" + User.FindFirst("userId")?.Value + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            return _authHelper.CreateToken(userId);

        }


    }
}