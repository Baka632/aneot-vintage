using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.ViewModels.Tag;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers;

/// <summary>
/// 标签页面控制器。
/// </summary>
/// <param name="categoryAndTagHelper">
/// 提供文章分类与标签信息的 <see cref="CategoryAndTagHelper"/>。
/// </param>
public class TagController(CategoryAndTagHelper categoryAndTagHelper) : Controller
{
    /// <summary>
    /// 显示标签页面。
    /// </summary>
    public IActionResult Index()
    {
        IDictionary<string, List<string>> mapping = categoryAndTagHelper.GetTagToArticleMapping();

        IndexViewModel model = new(mapping);
        return View(model);
    }

    /// <summary>
    /// 显示分类详细信息页。
    /// </summary>
    [Route("Tag/{tag}")]
    public IActionResult Detail(string tag)
    {
        IDictionary<string, List<string>> mapping = categoryAndTagHelper.GetTagToArticleMapping();

        if (mapping.ContainsKey(tag) != true)
        {
            return NotFound();
        }

        DetailViewModel model = new(tag, mapping[tag]);
        return View(model);
    }
}
