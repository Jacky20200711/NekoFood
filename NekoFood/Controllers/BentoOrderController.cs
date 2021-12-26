using NekoFood.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NekoFood.Services;

namespace NekoFood.Controllers
{
    public class BentoOrderController : Controller
    {
        private readonly ILogger<BentoOrderController> _logger;
        private readonly NekoFoodContext _context;
        private readonly HttpContext? _httpContext;

        public BentoOrderController(NekoFoodContext context,
            ILogger<BentoOrderController> logger,
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
                var data = await _context.BentoOrders.OrderByDescending(d => d.CreateTime).ToListAsync();
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"取得 BentoOrder 失敗 -> {ex}");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }

        public async Task<IActionResult> Create(string shopGuid, string groupGuid)
        {
            // 取出該訂單所對應的店家商品
            ViewBag.bentos = await _context.Bentos.Where(d => d.ShopGuid == shopGuid).ToListAsync();

            // 暫存此訂單對應的店家與群組
            ViewBag.shopGuid = shopGuid;
            ViewBag.groupGuid = groupGuid;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection PostData)
        {
            try
            {
                // 提取前端傳來的參數
                string groupGuid = PostData["groupGuid"].ToString().Trim();
                string shopGuid = PostData["shopGuid"].ToString().Trim();
                int number =  Utility.ConvertStrToInt(PostData["number"].ToString());
                string remark = PostData["remark"].ToString().Trim();

                if(groupGuid.Length != 32)
                {
                    TempData["message"] = "訂餐失敗，無法解析對應的群組";
                    return RedirectToAction("Index");
                }

                if (shopGuid.Length != 32)
                {
                    TempData["message"] = "訂餐失敗，無法解析對應的店家";
                    return RedirectToAction("Index");
                }

                // 解析便當資訊(包含名稱與價格，兩者以逗號隔開)
                string[] bentoData = PostData["bentoName"].ToString().Trim().Split(',');
                if(bentoData.Length != 2)
                {
                    TempData["message"] = "訂餐失敗，無法解析餐點名稱與價格";
                    return RedirectToAction("Index");
                }

                string bentoName = bentoData[0];
                int price = Utility.ConvertStrToInt(bentoData[1]);

                // 創建一筆資料
                BentoOrder newData = new()
                {
                    Payer = Utility.GetLoginName(HttpContext),
                    GroupGuid = groupGuid,
                    ShopGuid = shopGuid,
                    BentoName = bentoName,
                    Number = number,
                    TotalCost = number * price,
                    IsChecked = 0,
                    CreateTime = DateTime.Now,
                    Remark = remark,
                };
                
                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                TempData["message"] = "訂餐成功";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"新增 BentoOrder 失敗 -> {ex}");
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

                var data = _context.BentoOrders.FirstOrDefault(u => u.Id == id);

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
            catch (Exception ex)
            {
                _logger.LogError($"刪除 BentoOrder 失敗 -> {ex}");
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

            var data = await _context.BentoOrders.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            #endregion

            // 取出該訂單所對應的店家商品
            ViewBag.bentos = await _context.Bentos.Where(d => d.ShopGuid == data.ShopGuid).ToListAsync();

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
                int number = Utility.ConvertStrToInt(PostData["number"].ToString());
                string remark = PostData["remark"].ToString().Trim();

                // 解析便當資訊(包含名稱與價格，兩者以逗號隔開)
                string[] bentoData = PostData["bentoName"].ToString().Trim().Split(',');
                if (bentoData.Length != 2)
                {
                    TempData["message"] = "修改失敗，無法解析餐點名稱與價格";
                    return RedirectToAction("Index");
                }

                string bentoName = bentoData[0];
                int price = Utility.ConvertStrToInt(bentoData[1]);

                // 撈取目標資料
                var data = await _context.BentoOrders.FindAsync(id);
                if (data == null)
                {
                    TempData["message"] = "修改失敗，此筆資料不存在";
                    return RedirectToAction("Index");
                }

                // 修改目標資料並更新DB(只允許修改便當名稱、數量、備註)
                data.BentoName = bentoName;
                data.Number = number;
                data.TotalCost = number * price;
                data.Remark = remark;
                await _context.SaveChangesAsync();

                TempData["message"] = "修改成功";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"修改 BentoOrder 失敗 -> {ex}");
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
        }
    }
}
