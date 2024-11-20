using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Auth;
using BlogPost.Application.Interfaces.Posts;
using Microsoft.AspNetCore.Authorization;
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
            var res = await _postService.AddPosts(request, Convert.ToInt32(userId));
            return Ok(res);

        }
        #endregion SAVE

        #region GET
        [HttpGet]
        [Route("get-all-posts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PostResponse>))]
        public async Task<IActionResult> GetAllPosts()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var res = await _postService.GetAllPosts();
            return Ok(res);

        }

        [HttpPost]
        [Route("get-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostResponse))]
        public async Task<IActionResult> GetPosts(int id)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var res = await _postService.GetPost(id);
            return Ok(res);

        }
        #endregion GET

        #region Delete
        [HttpDelete]
        [Route("post")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var res = await _postService.DeletePosts(id);
            return Ok(res);
        }
        #endregion Delete
    }
}
