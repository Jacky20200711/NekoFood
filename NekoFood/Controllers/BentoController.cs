using NekoFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NekoFood.Controllers
{
    public class BentoController : Controller
    {
        private readonly ILogger<BentoController> _logger;
        private readonly NekoFoodContext _context;

        public BentoController(NekoFoodContext context, ILogger<BentoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string shopName, string shopGuid)
        {
            try
            {
                var data = await _context.Bentos
                    .Where(b => b.ShopGuid == shopGuid)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                ViewBag.ShopName = shopName;
                ViewBag.ShopGuid = shopGuid;
                return View(data);
            }
            catch (Exception)
            {
                _logger.LogError($"取得 Bento 失敗");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }

        [HttpPost]
        public async Task<string> Create(string shopGuid, string name, int? price)
        {
            try
            {
                // 取出參數內容
                if (string.IsNullOrEmpty(shopGuid) || string.IsNullOrEmpty(name) || price == null || price < 0)
                {
                    return "參數內容有誤";
                }

                // 新增資料
                Bento newData = new()
                {
                    ShopGuid = shopGuid,
                    Name = name,
                    Price = (int)price,
                };
                
                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                return "新增成功";
            }
            catch (Exception)
            {
                _logger.LogError($"新增 Bento 失敗");
                return "新增失敗";
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

                var data = _context.Bentos.FirstOrDefault(u => u.Id == id);

                if (data == null)
                {
                    return "刪除失敗，查無這筆資料!";
                }

                #endregion

                // 更新DB
                _context.Remove(data);
                await _context.SaveChangesAsync();

                return "刪除成功";
            }
            catch (Exception)
            {
                _logger.LogError($"刪除 Bento 失敗");
                return "刪除失敗，系統忙碌中";
            }
        }

        [HttpPost]
        public async Task<string> Edit(int? id, string name, int? price)
        {
            try
            {
                // 取出參數內容
                if (id == null || string.IsNullOrEmpty(name) || price == null || price < 0)
                {
                    return "修改失敗，參數內容有誤";
                }

                // 取出目標資料
                var data = await _context.Bentos.FindAsync(id);
                if (data == null)
                {
                    return "修改失敗，此筆資料不存在";
                }

                // 修改目標資料 & 更新DB
                data.Name = name;
                data.Price = (int)price;
                await _context.SaveChangesAsync();

                return "修改成功";
            }
            catch (Exception)
            {
                _logger.LogError($"修改 Bento 失敗");
                return "修改失敗";
            }
        }
    }
}
