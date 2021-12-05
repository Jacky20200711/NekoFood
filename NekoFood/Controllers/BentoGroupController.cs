using NekoFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NekoFood.Services;

namespace NekoFood.Controllers
{
    [AuthorizeManager]
    public class BentoGroupController : Controller
    {
        private readonly ILogger<BentoGroupController> _logger;
        private readonly NekoFoodContext _context;
        private readonly HttpContext? _httpContext;

        public BentoGroupController(NekoFoodContext context,
            ILogger<BentoGroupController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IActionResult> Index(bool isGetAll = false)
        {
            try
            {
                if (isGetAll)
                {
                    var data = await _context.BentoGroups.ToListAsync();
                    return View(data);
                }
                else
                {
                    var data = await _context.BentoGroups
                    .Where(d => d.Creator == Utility.GetLoginName(HttpContext))
                    .ToListAsync();

                    return View(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"取得 BentoGroup 失敗 -> {ex}");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }

        public async Task<IActionResult>  Create()
        {
            #region 撈取各商家的名稱+地址+Guid

            var shops = await _context.BentoShops.Select(x => new
            {
                x.Name,
                x.ShopGuid,
                x.Address
            }).ToListAsync();

            List<string> names = new();
            List<string> guids = new();

            foreach (var shop in shops)
            {
                names.Add($"{shop.Name} 地址: {shop.Address}");
                guids.Add(shop.ShopGuid);
            }

            #endregion
            ViewBag.names = names;
            ViewBag.guids = guids;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection PostData)
        {
            try
            {
                // 提取前端傳來的參數
                string shopGuid = PostData["shopGuid"].ToString().Trim();
                string name = PostData["name"].ToString().Trim();
                string exTimeStr = PostData["expireTime"].ToString();
                exTimeStr = exTimeStr.Replace("T", "-");
                exTimeStr = exTimeStr.Replace(":", "-");
                DateTime createTime = DateTime.Now;
                DateTime expireTime = DateTime.ParseExact(exTimeStr, "yyyy-MM-dd-HH-mm", null);

                // 檢查日期
                if (DateTime.Compare(expireTime, createTime) < 0)
                {
                    TempData["message"] = "新增失敗，收單時間設定有誤";
                    return RedirectToAction("Create");
                }

                // 創建一筆資料
                BentoGroup newData = new()
                {
                    Creator = Utility.GetLoginName(HttpContext),
                    GroupGuid = Guid.NewGuid().ToString("N"),
                    ShopGuid = shopGuid,
                    Name = name,
                    CreateTime = createTime,
                    ExpireTime = expireTime
                };

                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                TempData["message"] = "新增成功";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"新增 BentoGroup 失敗 -> {ex}");
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

                var data = _context.BentoGroups.FirstOrDefault(u => u.Id == id);

                if (data == null)
                {
                    return "刪除失敗，查無這筆資料!";
                }

                #endregion

                // 檢查是否擁有異動權限
                if (!PermissionService.HasPermissionToModifyBentoGroup(HttpContext, data.Creator))
                {
                    return "刪除失敗，權限不足";
                }

                // 更新DB
                _context.Remove(data);
                await _context.SaveChangesAsync();

                return "刪除成功";
            }
            catch (Exception ex)
            {
                _logger.LogError($"刪除 BentoGroup 失敗 -> {ex}");
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

            var data = await _context.BentoGroups.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            #endregion
            #region 撈取各商家的名稱+地址+Guid

            var shops = await _context.BentoShops.Select(x => new
            {
                x.Name,
                x.ShopGuid,
                x.Address
            }).ToListAsync();

            List<string> names = new();
            List<string> guids = new();

            foreach (var shop in shops)
            {
                names.Add($"{shop.Name} 地址: {shop.Address}");
                guids.Add(shop.ShopGuid);
            }

            #endregion
            ViewBag.names = names;
            ViewBag.guids = guids;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormCollection PostData)
        {
            try
            {
                // 提取前端傳來的參數
                int id = Convert.ToInt32(PostData["id"].ToString());
                string shopGuid = PostData["shopGuid"].ToString().Trim();
                string name = PostData["name"].ToString().Trim();
                string exTimeStr = PostData["expireTime"].ToString();
                exTimeStr = exTimeStr.Replace("T", "-");
                exTimeStr = exTimeStr.Replace(":", "-");
                DateTime createTime = DateTime.Now;
                DateTime expireTime = DateTime.ParseExact(exTimeStr, "yyyy-MM-dd-HH-mm", null);

                // 檢查日期
                if (DateTime.Compare(expireTime, createTime) < 0)
                {
                    TempData["message"] = "修改失敗，收單時間設定有誤";
                    return RedirectToAction("Edit", new { id });
                }

                // 撈取目標資料
                var data = await _context.BentoGroups.FindAsync(id);
                if (data == null)
                {
                    TempData["message"] = "修改失敗，此筆資料不存在";
                    return RedirectToAction("Index");
                }

                // 檢查是否擁有異動權限
                if (!PermissionService.HasPermissionToModifyBentoGroup(HttpContext, data.Creator))
                {
                    TempData["message"] = "修改失敗，權限不足";
                    return RedirectToAction("Index");
                }

                // 若對應的店家被修改，則更新群組的Guid(令舊訂單不要再對應到這個群組)
                if (data.ShopGuid != shopGuid)
                {
                    data.GroupGuid = Guid.NewGuid().ToString("N");
                    data.ShopGuid = shopGuid;
                }

                // 修改目標資料並更新DB
                data.Name = name;
                data.ExpireTime = expireTime;
                await _context.SaveChangesAsync();

                TempData["message"] = "修改成功";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"修改 BentoGroup 失敗 -> {ex}");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }
    }
}
