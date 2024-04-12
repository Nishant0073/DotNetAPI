using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Models
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController: ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration iconfig)
        {
            _dapper = new DataContextDapper(iconfig);
        }

        [HttpGet("Posts/{userId}/{postId}/{searchValue}")]
        public IEnumerable<Post> GetPosts(int userId=0,int postId=0,string searchValue="None")
        {

            string sql = "EXEC TutorialAppSchema.spPosts_Get ";
            string parameter = "";
            if(userId!=0)
            {
                parameter += ", @UserId=" + userId.ToString();
            }
            if(postId!=0)
            {
                parameter += ", @PostId=" + postId.ToString();
            }
            if(searchValue.ToLower()!="none")
            {
                parameter += ", @SearchValue=" + searchValue;
            }

            if(parameter.IsNullOrEmpty()==false)
                sql+= parameter.Substring(1);
            Console.WriteLine(sql);
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

         

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {            
            string sql = "EXEC TutorialAppSchema.spPosts_Get  @UserId = " + User.FindFirst("userId")?.Value;
            Console.WriteLine(sql);
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

        [HttpPut("PostUpsert")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
        string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                        @UserId =" + User.FindFirst("userId")?.Value +
                        ", @PostTitle ='" + postToUpsert.PostTitle +
                        "', @PostContent ='" + postToUpsert.PostContent + "'";

                    if (postToUpsert.PostId > 0) {
                        sql +=  ", @PostId = " + postToUpsert.PostId;
                    }

                    if (_dapper.ExecuteSql(sql))
                    {
                        return Ok();
                    }

                    throw new Exception("Failed to upsert post!");
        }

        [HttpDelete("Post/{postID}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = " + 
                    postId.ToString() +
                    ", @UserId = " + this.User.FindFirst("userId")?.Value;

            
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }

    }

}