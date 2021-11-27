using NekoFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NekoFood.Controllers
{
    [AuthorizeManager]
    public class BentoShopController : Controller
    {
        private readonly ILogger<BentoShopController> _logger;
        private readonly NekoFoodContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BentoShopController(NekoFoodContext context,
            ILogger<BentoShopController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var data = await _context.BentoShops.ToListAsync();
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"取得 BentoShop 失敗 -> {ex}");
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
                string name = PostData["name"].ToString().Trim();
                string address = PostData["address"].ToString().Trim();
                string phone = PostData["phone"].ToString().Trim();
                if(string.IsNullOrEmpty(name) ||
                   string.IsNullOrEmpty(address) ||
                   string.IsNullOrEmpty(phone))
                {
                    TempData["message"] = "新增失敗，店名、地址、電話不能為空";
                    return RedirectToAction("Create");
                }

                // 新增資料
                BentoShop newData = new()
                {
                    ShopGuid = Guid.NewGuid().ToString("N"),
                    Name = name,
                    Address = address,
                    Phone = phone,
                };
                
                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                TempData["message"] = "新增成功";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError("新增 BentoShop 失敗 -> {ex}");
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

                var data = _context.BentoShops.FirstOrDefault(u => u.Id == id);

                if (data == null)
                {
                    return "刪除失敗，查無這筆資料!";
                }

                #endregion

                // 更新DB (連動刪除此店家的商品)
                using var transaction = _context.Database.BeginTransaction();
                _context.RemoveRange(_context.Bentos.Where(b => b.ShopGuid == data.ShopGuid));
                _context.Remove(data);
                await _context.SaveChangesAsync();
                transaction.Commit();

                return "刪除成功";
            }
            catch (Exception ex)
            {
                _logger.LogError($"刪除 BentoShop 失敗 -> {ex}");
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

            var data = await _context.BentoShops.FindAsync(id);

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
                string name = PostData["name"].ToString().Trim();
                string address = PostData["address"].ToString().Trim();
                string phone = PostData["phone"].ToString().Trim();
                int id = Convert.ToInt32(PostData["id"].ToString());
                if (string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(address) ||
                    string.IsNullOrEmpty(phone))
                {
                    TempData["message"] = "修改失敗，店名、地址、電話不能為空";
                    return RedirectToAction("Edit", new { id });
                }

                // 取出目標資料
                var data = await _context.BentoShops.FindAsync(id);
                if (data == null)
                {
                    TempData["message"] = "修改失敗，此筆資料不存在";
                    return RedirectToAction("Index");
                }

                // 修改目標資料 & 更新DB
                data.Name = name;
                data.Address = address;
                data.Phone = phone;
                await _context.SaveChangesAsync();

                TempData["message"] = "修改成功";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"修改 BentoShop 失敗 -> {ex}");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }
    }
}
