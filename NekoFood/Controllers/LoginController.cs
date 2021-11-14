using Microsoft.AspNetCore.Mvc;
using NekoFood.Models;
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
        [ValidateAntiForgeryToken]
        public IActionResult LoginCheck(string username, string password)
        {
            try
            {
                // 檢查參數
                username = username.Trim();
                password = password.Trim();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    TempData["message"] = "AD帳號或密碼不能為空";
                    return RedirectToAction("Index");
                }

                // 轉換密碼
                using var md5 = MD5.Create();
                var hashResult = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
                string passwordHash = BitConverter.ToString(hashResult).Replace("-", "");

                // 檢查帳密
                var userAccount = _context.UserAccounts.FirstOrDefault(u => u.Name == username && u.PasswordHash == passwordHash);
                if(userAccount == null)
                {
                    TempData["message"] = "帳密錯誤";
                    return RedirectToAction("Index");
                }

                // 設置登入後的狀態
                HttpContext.Session.SetString("loginName", username);

                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            catch (Exception)
            {
                _logger.LogError("登入失敗");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
