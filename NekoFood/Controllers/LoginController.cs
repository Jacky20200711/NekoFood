using Microsoft.AspNetCore.Mvc;
using NekoFood.Models;
using NekoFood.Services;
using System.Security.Cryptography;
using System.Text;

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
                if(Utility.GetLoginName(HttpContext) != null)
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

                // 轉換密碼
                using var md5 = MD5.Create();
                var hashResult = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
                string passwordHash = BitConverter.ToString(hashResult).Replace("-", "");

                // 檢查帳密
                var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Name == username && u.PasswordHash == passwordHash);
                if(userAccount == null)
                {
                    return "帳密錯誤";
                }

                // 設置登入後的狀態
                string userGuid = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("loginName", username);
                HttpContext.Session.SetString("loginGuid", userGuid);
                AppCache.Set(username, userGuid);
                return "登入成功";
            }
            catch (Exception)
            {
                _logger.LogError("登入失敗");
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
