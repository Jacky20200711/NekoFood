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
            #region 開發階段可以取消這一段的註解以方便測試
            //context.HttpContext.Session.SetString("loginName", "Jacky");
            //context.HttpContext.Session.SetString("admin", "Y");
            #endregion

            // 若用戶尚未登入，則跳轉到登入頁面
            string loginName = Utility.GetLoginName(context.HttpContext);
            if (string.IsNullOrEmpty(loginName))
            {
                context.Result = new RedirectToRouteResult(new { controller = "Login", action = "Index" });
            }

            // 控制只有管理員才能存取的 Controller 權限
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                // 取得 request的 controller 名稱
                string controllerName = controllerActionDescriptor.ControllerName;
                bool isAdmin = context.HttpContext.Session.GetString("admin") != null;
                if (!isAdmin && (controllerName == "UserAccount" || controllerName == "BentoShop"))
                {
                    context.Result = new NotFoundResult();
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
                    { "isClearCache", false }, // 小心不要清除到 Cache (會造成新裝置的登入也失效)
                });
            }
        }
    }
}
