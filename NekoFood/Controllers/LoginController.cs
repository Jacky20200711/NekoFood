using Microsoft.AspNetCore.Mvc;
using NekoFood.Models;
using NekoFood.Services;

namespace NekoFood.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly NekoFoodContext _context;

        public LoginController(NekoFoodContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string LoginCheck(string username, string password)
        {
            try
            {
                // 避免重複登入
                if(!string.IsNullOrEmpty(Utility.GetLoginName(HttpContext)))
                {
                    return "請勿重複登入";
                }

                // 檢查參數
                username = username.Trim();
                password = password.Trim();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return "AD帳號或密碼不能為空";
                }

                // 檢查帳密
                var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Name == username && u.PasswordHash == Utility.GetEncryptPassword(password));
                if(userAccount == null)
                {
                    return "帳密錯誤";
                }

                // 設置登入後的狀態
                string userGuid = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("loginName", username);
                HttpContext.Session.SetString("loginGuid", userGuid);
                AppCache.Set(username, userGuid);

                // 若登入者為管理員，則設置特有的屬性
                if(userAccount.UserGroup == "admin")
                {
                    HttpContext.Session.SetString("admin", "Y");
                }

                return "登入成功";
            }
            catch (Exception ex)
            {
                _logger.LogError($"登入失敗 -> {ex}");
                return "登入失敗";
            }
        }

        public IActionResult Logout(bool isClearCache = true)
        {
            if (isClearCache)
            {
                AppCache.Remove(Utility.GetLoginName(HttpContext));
            }

            HttpContext.Session.Clear();
            TempData["isUserLogout"] = "Y";
            return RedirectToAction("Index");
        }
    }
}
