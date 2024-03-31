using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Models
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController: ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration iconfig)
        {
            _dapper = new DataContextDapper(iconfig);
        }

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sqlForPost = @"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] FROM TutorialAppSchema.Posts;";

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sqlForPost);
            return posts;
        }

        [HttpGet("PostSingle/{PostId}")]
        public Post GetPostSingle(int PostId)
        {
            string sqlForPost = @"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] FROM TutorialAppSchema.Posts
                                    WHERE PostId = " + PostId.ToString();

            Post post = _dapper.LoadDataSingle<Post>(sqlForPost);
            return post;
        }
        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsBy(int userId)
        {
            string sqlForPost = @"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] FROM TutorialAppSchema.Posts WHERE UserId =" + userId.ToString();

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sqlForPost);
            return posts;
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sqlForPost = @"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] FROM TutorialAppSchema.Posts WHERE UserId =" + User.FindFirst("userId")?.Value;

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sqlForPost);
            return posts;
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string  sql = @"INSERT INTO [TutorialAppSchema].[Posts]
                            (  [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated]
                            )
                            VALUES
                            ( "+User.FindFirst("userId")?.Value
                             +",'"+postToAdd.PostTitle
                             +"','"+postToAdd.PostContent
                             +"', GETDATE(), GETDATE() )";
                if(_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
                throw new Exception("Failed to create new Post");
        }

           [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE PostTitle LIKE '%" + searchParam + "%'" +
                        " OR PostContent LIKE '%" + searchParam + "%'";
                
            return _dapper.LoadData<Post>(sql);
        }

        
        [HttpPut("Post/{postToEdit}")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string  sql = @" UPDATE [TutorialAppSchema].[Posts]
                            SET
                                [PostTitle] =  '" +  postToEdit.PostTitle
                                + "', [PostContent] = '"  + postToEdit.PostContent
                                + "', [PostUpdated] = GETDATE() WHERE  PostId = " +  postToEdit.PostId.ToString()
                                + "AND UserId = " + User.FindFirst("userId")?.Value.ToString();

                if(_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
                throw new Exception("Failed to create edit Post");
        }


        [HttpDelete("Post/{postID}")]
        public IActionResult DeletePost(int postID)
        {
            string sql  = "DELETE FROM  [TutorialAppSchema].[Posts] WHERE PostId = " + postID.ToString()  
            + "AND UserId = " + User.FindFirst("userId")?.Value.ToString();
            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete");
        }

    }

}