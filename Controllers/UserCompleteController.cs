using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controller
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        [HttpGet("GetUsers")]
        public IEnumerable<UserComplete> GetUsers(int userId,bool Active)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string parameter = "";
            if(userId!=0)
            {
                parameter += ", @UserId=" + userId.ToString();
            }
            if(Active)
            {
                parameter += ", @Active=" + Active.ToString();
            }
            if(parameter.IsNullOrEmpty()==false)
                sql+= parameter.Substring(1);
            Console.WriteLine(sql);
            IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserComplete user)
        {

            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                @FirstName='" + user.FirstName
                + "', @LastName='" + user.LastName
                + "', @Email='" + user.Email
                + "', @Gender='" + user.Gender
                + "', @JobTitle='" + user.JobTitle
                + "', @Department='" + user.Department
                + "', @Salary='" + user.Salary
                + "', @Active='" + user.Active
                + "', @UserId=" + user.UserId ;
            Console.WriteLine(sql);
            bool result = _dapper.ExecuteSql(sql);
            if (result)
            {
                return Ok();
            }
            throw new Exception("Failed to update User");
        }

        
        
 
        
        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Delete @UserId = "+userId.ToString();

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }



        public static string EncloseSingleQuote(string input)
        {
            return input.Replace("'", "''");
        }
    }


}