using NekoFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NekoFood.Services;

namespace NekoFood.Controllers
{
    [AuthorizeManager]
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
            catch (Exception ex)
            {
                _logger.LogError($"取得 UserAccount 失敗 -> {ex}");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> Create(string userName, string password)
        {
            try
            {
                userName = userName.Trim();
                password = password.Trim();

                if (userName.Length < 1 || password.Length < 1)
                {
                    return "用戶名稱與密碼不可為空";
                }

                // 禁止帳號重複
                var data = _context.UserAccounts.FirstOrDefault(p => p.Name == userName);
                if(data != null)
                {
                    return "新增失敗，此帳號已被使用";
                }

                // 新增資料
                UserAccount newData = new()
                {
                    Name = userName,
                    PasswordHash = Utility.GetEncryptPassword(password),
                };
                
                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                return "新增成功";
            }
            catch (Exception ex)
            {
                _logger.LogError($"新增 UserAccount 失敗 -> {ex}");
                return "新增失敗，資料庫忙碌中";
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
            catch (Exception ex)
            {
                _logger.LogError($"刪除 UserAccount 失敗 -> {ex}");
                return "刪除失敗，系統忙碌中";
            }
        }

        [HttpPost]
        public async Task<string> Edit(string userName, string newPassword)
        {
            try
            {
                newPassword = newPassword.Trim();

                if (string.IsNullOrEmpty(newPassword))
                {
                    return "密碼不可為空";
                }


                // 取出目標資料
                var data = _context.UserAccounts.FirstOrDefault(p => p.Name == userName);
                if (data == null)
                {
                    return "無此帳號";
                }

                // 更新DB
                data.PasswordHash = Utility.GetEncryptPassword(newPassword);
                await _context.SaveChangesAsync();

                return "修改成功";
            }
            catch (Exception ex)
            {
                _logger.LogError($"修改 UserAccount 失敗 -> {ex}");
                return "修改失敗，資料庫忙碌中";
            }
        }

        [HttpPost]
        public async Task<string> ChangePassword(string loginName, string newPassword)
        {
            try
            {
                // 取出目標資料
                var data = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Name == loginName);
                if (data == null)
                {
                    return "修改失敗，此筆資料不存在";
                }

                // 修改目標資料 & 更新DB
                data.PasswordHash = Utility.GetEncryptPassword(newPassword);
                await _context.SaveChangesAsync();

                return "修改成功";
            }
            catch (Exception ex)
            {
                _logger.LogError($"修改 UserAccount 失敗 -> {ex}");
                return "修改失敗，資料庫忙碌中";
            }
        }
    }
}
