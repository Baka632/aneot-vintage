using Markdig;
using Markdig.Syntax;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers
{
    /// <summary>
    /// 文章相关资源获取API的控制器
    /// </summary>
    [Route("api/posts")]
    [ApiController]
    public class PostsApiController : ControllerBase
    {
        /// <summary>
        /// 程序执行环境的信息提供者
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// 构造<see cref="PostsApiController"/>控制器的新实例，通常此构造器仅由依赖注入容器调用
        /// </summary>
        public PostsApiController(IWebHostEnvironment env)
        {
            environment = env;
        }

        /// <summary>
        /// 获取指定的文章
        /// </summary>
        /// <param name="post">刊物期数</param>
        /// <param name="article">文章名称</param>
        /// <returns>如果article参数为空，则返回当期刊物的全部文章列表；反之返回指定的文章</returns>
        /// <response code="404">找不到指定的期刊（或文章）</response>
        #region HttpGet
        //不在这里添加可选参数"article"的原因是：
        //若在这里添加"article"，Swagger会把article显示为必选参数，这不符合语义
        [HttpGet("{post}/")]
        #endregion
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPost(string post, string? article)
        {
            string contentPath;
            if (string.IsNullOrWhiteSpace(article))
            {
                //article参数为空，那我们就返回当前期刊的全部文章
                //MIME为application/json
                contentPath = Path.Combine(environment.WebRootPath, "aneot", "posts", post, "README.md");
                
                if (!System.IO.File.Exists(contentPath))
                {
                    return NotFound();
                }
            }


            return Ok($"Test:{post}/{article}");
        }
    }
}
