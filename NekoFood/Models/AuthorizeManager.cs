using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NekoFood.Services;

namespace NekoFood.Models
{
    public class AuthorizeManager : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 若用戶尚未登入，則跳轉到登入頁面
            string loginName = Utility.GetLoginName(context.HttpContext);
            if (string.IsNullOrEmpty(loginName))
            {
                context.Result = new RedirectToRouteResult(new { controller = "Login", action = "Index" });
            }

            // 處理只有管理員才能存取的情況
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                // 取得 User 請求的 controller 與 action 名稱
                string controllerName = controllerActionDescriptor.ControllerName;
                string actionName = controllerActionDescriptor.ActionName;
                bool isAdmin = context.HttpContext.Session.GetString("admin") != null;
                if (!isAdmin)
                {
                    if (controllerName == "BentoShop" || controllerName == "Bento")
                    {
                        context.Result = new NotFoundResult();
                    }

                    // 開放 UserAccount 底下的 ChangePassword 供一般用戶存取
                    if (controllerName == "UserAccount" && actionName != "ChangePassword")
                    {
                        context.Result = new NotFoundResult();
                    }
                }
            }

            // 比對 Session 和 Cache 裡面的 Guid 是否一樣，若不一樣則清除舊裝置的登入資訊
            string? loginGuid = context.HttpContext.Session.GetString("loginGuid");
            string? validGuid = AppCache.Get(loginName);
            if (loginGuid != validGuid)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "action", "Logout" },
                    { "controller", "Login" },
                    { "isClearCache", false }, // 小心不要清除到 Cache (避免造成新裝置的登入失效)
                });
            }
        }
    }
}
