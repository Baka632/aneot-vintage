using AnEoT.Vintage.Helpers;
using AnEoT.Vintage.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace AnEoT.Vintage.Controllers;

/// <summary>
/// 分类页面控制器。
/// </summary>
/// <param name="categoryAndTagHelper">
/// 提供文章分类与标签信息的 <see cref="CategoryAndTagHelper"/>。
/// </param>
public class CategoryController(CategoryAndTagHelper categoryAndTagHelper) : Controller
{
    /// <summary>
    /// 显示分类页面
    /// </summary>
    public IActionResult Index()
    {
        IDictionary<string, List<string>> mapping = categoryAndTagHelper.GetCategoryToArticleMapping();

        IndexViewModel model = new(mapping);
        return View(model);
    }

    /// <summary>
    /// 显示分类详细信息页
    /// </summary>
    [Route("Category/{category}")]
    public IActionResult Detail(string category)
    {
        IDictionary<string, List<string>> mapping = categoryAndTagHelper.GetCategoryToArticleMapping();

        if (mapping.ContainsKey(category) != true)
        {
            return NotFound();
        }

        DetailViewModel model = new(category, mapping[category]);
        return View(model);
    }
}
