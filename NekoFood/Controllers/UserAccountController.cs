using NekoFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NekoFood.Services;

namespace NekoFood.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly NekoFoodContext _context;
        private readonly HttpContext? _httpContext;

        public UserAccountController(NekoFoodContext context,
            ILogger<UserAccountController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var data = await _context.UserAccounts.ToListAsync();
                return View(data);
            }
            catch (Exception)
            {
                _logger.LogError($"取得 UserAccount 失敗");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection PostData)
        {
            try
            {
                // 取出參數內容
                string username = PostData["username"].ToString().Trim();
                string password = PostData["password"].ToString().Trim();
                if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    TempData["message"] = "新增失敗，帳號或密碼不能為空";
                    return RedirectToAction("Create");
                }

                // 禁止帳號重複
                var data = _context.UserAccounts.FirstOrDefault(p => p.Name == username);
                if(data != null)
                {
                    TempData["message"] = "新增失敗，帳號不能重複";
                    return RedirectToAction("Create");
                }

                // 新增資料
                UserAccount newData = new()
                {
                    Name = username,
                    PasswordHash = Utility.GetEncryptPassword(password),
                };
                
                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                TempData["message"] = "新增成功";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                _logger.LogError($"新增 UserAccount 失敗");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }

        [HttpPost]
        public async Task<string> Delete(int? id)
        {
            try
            {

                #region 檢查此筆資料是否存在

                if (id == null)
                {
                    return "刪除失敗，查無這筆資料!";
                }

                var data = _context.UserAccounts.FirstOrDefault(u => u.Id == id);

                if (data == null)
                {
                    return "刪除失敗，查無這筆資料!";
                }

                #endregion

                // 禁止刪除管理員
                if(data.UserGroup == "admin")
                {
                    return "刪除失敗";
                }

                // 更新DB
                _context.Remove(data);
                await _context.SaveChangesAsync();

                return "刪除成功";
            }
            catch (Exception)
            {
                _logger.LogError($"刪除 UserAccount 失敗");
                return "刪除失敗，系統忙碌中";
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            #region 檢查資料庫是否有這筆資料

            if (id == null)
            {
                return NotFound();
            }

            var data = await _context.UserAccounts.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            #endregion

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormCollection PostData)
        {
            try
            {
                // 取出參數內容
                string password = PostData["password"].ToString().Trim();
                int id = Convert.ToInt32(PostData["id"].ToString());
                if (string.IsNullOrEmpty(password))
                {
                    TempData["message"] = "修改失敗，帳號或密碼不能為空";
                    return RedirectToAction("Edit", new { id });
                }

                // 取出目標資料
                var data = await _context.UserAccounts.FindAsync(id);
                if (data == null)
                {
                    TempData["message"] = "修改失敗，此筆資料不存在";
                    return RedirectToAction("Index");
                }

                // 修改目標資料 & 更新DB
                data.PasswordHash = Utility.GetEncryptPassword(password);
                await _context.SaveChangesAsync();

                TempData["message"] = "修改成功";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                _logger.LogError($"修改 UserAccount 失敗");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }
    }
}
