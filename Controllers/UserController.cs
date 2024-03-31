using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        [HttpGet("GetUser")]
        public User GetUser(int userId)
        {
            string sql = @"SELECT  [UserId]
                        , [FirstName]
                        , [LastName]    
                        , [Email]
                        , [Gender]
                        , [Active]
                        FROM  TutorialAppSchema.Users WHERE [UserId]=" + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
            return user;
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            string sql = @"SELECT  [UserId]
                        , [FirstName]
                        , [LastName]
                        , [Email]
                        , [Gender]
                        , [Active]
                        FROM  TutorialAppSchema.Users;";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            string sql = @"
            UPDATE [TutorialAppSchema].[Users]
            SET
                [FirstName] = '" + user.FirstName
                + "', [LastName] = '" + user.LastName
                + "', [Email] = '" + user.Email
                + "', [Gender] = '" + user.Gender
                + "', [Active] = '" + user.Active
                + "' WHERE UserId = " + user.UserId;
            // Console.WriteLine(sql);
            bool result = _dapper.ExecuteSql(sql);
            if (result)
            {
                return Ok();
            }
            throw new Exception("Failed to update User");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserInsertDto user)
        {
            string sql = @"INSERT INTO TutorialAppSchema.Users(
                        FirstName,
                        LastName,
                        Email,
                        Gender,
                        Active
                    ) VALUES('"
            + EncloseSingleQuote(user.FirstName)
            + "','" + EncloseSingleQuote(user.LastName)
            + "','" + user.Email
            + "','" + user.Gender
            + "','" + user.Active
            + "')";
            Console.WriteLine(sql);
            bool result = _dapper.ExecuteSql(sql);
            if (result)
            {
                return Ok();
            }
            throw new Exception("Failed to add user");
        }

        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(int UserId)
        {
            string sql = "DELETE  From [TutorialAppSchema].[Users] WHERE UserId = " + UserId.ToString();
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Unable to delete user");
        }

        [HttpGet("UserSalary/{userId}")]
        public IEnumerable<UserSalary> GetUserSalary(int userId)
        {
            return _dapper.LoadData<UserSalary>(@"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM  TutorialAppSchema.UserSalary
                WHERE UserId = " + userId.ToString());
        }

        [HttpPost("UserSalary")]
        public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES (" + userSalaryForInsert.UserId.ToString()
                    + ", " + userSalaryForInsert.Salary
                    + ")";

            if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
            {
                return Ok(userSalaryForInsert);
            }
            throw new Exception("Adding User Salary failed on save");
        }

        [HttpPut("UserSalary")]
        public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary="
                + userSalaryForUpdate.Salary
                + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userSalaryForUpdate);
            }
            throw new Exception("Updating User Salary failed on save");
        }

        [HttpDelete("UserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId=" + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Deleting User Salary failed on save");
        }

        [HttpGet("UserJobInfo/{userId}")]
        public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
        {
            return _dapper.LoadData<UserJobInfo>(@"
            SELECT  UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
            FROM  TutorialAppSchema.UserJobInfo
                WHERE UserId = " + userId.ToString());
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES (" + userJobInfoForInsert.UserId
                    + ", '" + userJobInfoForInsert.Department
                    + "', '" + userJobInfoForInsert.JobTitle
                    + "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForInsert);
            }
            throw new Exception("Adding User Job Info failed on save");
        }

        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='"
                + userJobInfoForUpdate.Department
                + "', JobTitle='"
                + userJobInfoForUpdate.JobTitle
                + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForUpdate);
            }
            throw new Exception("Updating User Job Info failed on save");
        }

        // [HttpDelete("UserJobInfo/{userId}")]
        // public IActionResult DeleteUserJobInfo(int userId)
        // {
        //     string sql = "DELETE FROM TutorialAppSchema.UserJobInfo  WHERE UserId=" + userId;

        //     if (_dapper.ExecuteSql(sql))
        //     {
        //         return Ok();
        //     }
        //     throw new Exception("Deleting User Job Info failed on save");
        // }

        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = " + userId.ToString();

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