using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Auth;
using BlogPost.Application.Interfaces.Posts;
using Microsoft.AspNetCore.Mvc;

namespace BlogPostWebApi.Controllers.Posts
{
    [Route("api/post-management")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        #region SAVE
        [HttpPost]
        [Route("post")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostResponse))]
        public async Task<IActionResult> AddPosts([FromBody] PostRequest request)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;

            PostResponse res = await _postService.AddPosts(request, Convert.ToInt32(userId));
            return Ok(res);
        }
        #endregion SAVE

        #region Delete
        [HttpDelete]
        [Route("post")]
        public async Task<IActionResult> DeletePost(int id)
        {
            PostResponse res = await _postService.DeletePosts(id);
            return Ok(res);
        }
        #endregion Delete
    }
}
