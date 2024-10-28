using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
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
            PostResponse res = await _postService.AddPosts(request, 1);
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
