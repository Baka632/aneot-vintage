﻿using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;

namespace AnEoT.Vintage.Controllers.Api;

/// <summary>
/// 文章相关资源获取 API 的控制器
/// </summary>
/// <param name="env">
/// 程序执行环境的信息提供者
/// </param>
[ApiController]
[Route("api/[controller]")]
public class PostsController(IWebHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// 获取指定期刊的信息
    /// </summary>
    /// <param name="post">刊物期数</param>
    /// <returns>指定期刊的相关信息</returns>
    /// <response code="200">成功找到指定的期刊</response>
    /// <response code="404">找不到指定的期刊</response>
    [HttpGet("{post}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetVolumeInfo(string post)
    {
        string contentPath = Path.Combine(env.WebRootPath, "posts", post, "README.md");

        if (!SystemIOFile.Exists(contentPath))
        {
            return NotFound();
        }

        string markdown = SystemIOFile.ReadAllText(contentPath);
        string volName = MarkdownHelper.GetFromFrontMatter<ArticleInfo>(markdown).Title;
        MarkdownDocument document = Markdown.Parse(markdown);

        IEnumerable<LinkInline> links = document.Descendants<LinkInline>();
        Dictionary<string, string> articles = new(links.Count());
        foreach (LinkInline item in links)
        {
            if (!item.IsImage)
            {
                string articleTitle;
                if (item.FirstChild is EmphasisInline emphasis)
                {
                    articleTitle = emphasis.FirstChild?.ToString() ?? string.Empty;
                }
                else
                {
                    articleTitle = item.FirstChild?.ToString() ?? string.Empty;
                }

                string url = $"{Request.Scheme}://{Request.Host.ToUriComponent()}/api/posts/{post}/{item.Url}";
                articles[articleTitle] = url;
            }
        }

        VolumeInfo volumeInfo = new(volName, articles);
        return new JsonResult(volumeInfo);
    }

    /// <summary>
    /// 获取指定的文章
    /// </summary>
    /// <param name="post">刊物期数</param>
    /// <param name="article">文章名称（不带 .md 扩展名）</param>
    /// <returns>指定文章的 Markdown 文件</returns>
    /// <response code="200">成功找到指定的文章</response>
    /// <response code="404">找不到指定的文章</response>
    [HttpGet("{post}/{article}")]
    [Produces("text/markdown")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetArticle(string post, string article)
    {
        if (!article.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            article += ".md";
        }

        string contentPath = Path.Combine(env.WebRootPath, "posts", post, article);

        if (!SystemIOFile.Exists(contentPath))
        {
            return NotFound();
        }

        string markdown = SystemIOFile.ReadAllText(contentPath);

        ContentResult contentResult = new()
        {
            Content = markdown,
            ContentType = "text/markdown; charset=utf-8",
            StatusCode = StatusCodes.Status200OK
        };
        return contentResult;
    }
}
