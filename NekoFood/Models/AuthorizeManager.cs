using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NekoFood.Services;

namespace NekoFood.Models
{
    public class AuthorizeManager : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 若用戶尚未登入，則跳轉到登入頁面
            string? loginName = Utility.GetLoginName(context.HttpContext);
            if (loginName == null)
            {
                context.Result = new RedirectToRouteResult(new { controller = "Login", action = "Index" });
            }

            // 比對 Session 和 Cache 裡面的 Guid 是否一樣，若不一樣則清除舊裝置的登入資訊
            string? loginGuid = context.HttpContext.Session.GetString("loginGuid");
            string? validGuid = AppCache.Get(loginName);
            if (loginGuid != validGuid)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "action", "Logout" },
                    { "controller", "Login" },
                    { "isClearCache", false }, // 小心不要清除到 Cache (會造成新裝置的登入也失效)
                });
            }
        }
    }
}
