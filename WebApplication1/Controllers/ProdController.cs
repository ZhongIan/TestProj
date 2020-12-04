using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Common;
using WebApplication1.ViewModels.Prod;
using WebApplication1.Models.SearchResult;

namespace WebApplication1.Controllers
{

    [MyAuthorize(權限 = e會員_權限.私廚)] // 登入驗證 私廚
    public class ProdController : Controller
    {
        // GET: Prod

        private Database1Entities db = new Database1Entities();

        #region 私廚{ [私廚販售項目清單] [新增販售項目] [新增菜色] [設定可預訂時間] }
        //==================================================================================
        // 私廚{ [私廚販售項目清單] [(新增)(編輯)販售項目] [(新增)(編輯)菜色] [設定可預訂時間] }
        //          
        //==================================================================================

        // === [設定可預訂時間] === 
        public ActionResult calendar()
        {
            // 取得此私廚
            var chefId = (int)Session[CDictionary.SK_LOGINED_CHEF_ID];
            // 取得此私廚可預訂時間
            var table = (from t in (new Database1Entities()).t私廚可預訂時間
                         where t.fCID == chefId
                         select t).ToList();

            // 被預訂_不可修改
            var listUnable = new List<string>();
            // 可預定
            var listEnable = new List<string>();

            listUnable.AddRange(
                table
                .Where(t => t.f狀態 == e私廚可預訂_時段_狀態.被預訂_不可修改.GetHashCode())
                .Select(x => x.f日期.ToString("d") + "-" + x.f時段.ToString()).ToList()
                );

            listEnable.AddRange(
                table
                .Where(t => t.f狀態 == e私廚可預訂_時段_狀態.可預定.GetHashCode())
                .Select(x => x.f日期.ToString("d") + "-" + x.f時段.ToString()).ToList()
                );

            var vm = new v可預訂時間VM
            {
                fCID = chefId,
                ArrEnable = listEnable,
                ArrUnable = listUnable
            };

            return View(vm);

        }

        [HttpPost] // 限定 POST
        [ValidateAntiForgeryToken] // 防止 CSRF 跨域偽造請求, 只有從頁面來的才能通過
        public ActionResult calendar(v可預訂時間VM vm)
        {
            // 新增 可預訂時間
            string sCreate = vm.ArrGetNewTime;
            if (!string.IsNullOrEmpty(sCreate))
            {
                // 拆解字串
                string[] ArrSplit = sCreate.Split(',');
                foreach (string s1 in ArrSplit)
                {
                    if (!string.IsNullOrEmpty(s1))
                    {
                        string[] Arr = s1.Split('-');
                        t私廚可預訂時間 t = new t私廚可預訂時間();
                        t.fCID = vm.fCID;
                        t.f日期 = Convert.ToDateTime(Arr[0]);
                        t.f時段 = Convert.ToInt32(Arr[1]);
                        t.f狀態 = e私廚可預訂_時段_狀態.可預定.GetHashCode();
                        db.t私廚可預訂時間.Add(t);
                    }
                }
                // 儲存DB
                db.SaveChanges();
            }

            // 刪除 可預定時間
            string sDelete = vm.ArrGetDeletedTime;

            if (!string.IsNullOrEmpty(sDelete))
            {
                // 拆解字串
                string[] ArrSplitDelete = sDelete.Split(',');
                foreach (string s in ArrSplitDelete)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        string[] Arr = s.Split('-');
                        DateTime time = Convert.ToDateTime(Arr[0]);
                        int status = Convert.ToInt32(Arr[1]);
                        var deleteTime = db.t私廚可預訂時間.FirstOrDefault(t => t.f日期 == time && t.f時段 == status);
                        if (deleteTime != null)
                        {
                            db.t私廚可預訂時間.Remove(deleteTime);

                        }
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("calendar");
        }

        // === [販售清單列表] ===
        public ActionResult salesItemList()
        {
            var chefId = (int)Session[CDictionary.SK_LOGINED_CHEF_ID];

            var chef = db.t私廚.FirstOrDefault(c => c.fCID == chefId);

            var prods = db.t販售項目.Where(p => p.fCID == chefId);

            var 項目清單 = new List<項目>();

            foreach (var item in prods)
            {
                項目清單.Add(new 項目 {
                    product = item,
                    dish = item.t菜品.ToList()
                });
            }

            var vm = new v私廚販售項目清單VM
            {
                chef = chef,
                prods = 項目清單
            };

            return View(vm);
        }


        // === 項目創建 === 
        public ActionResult productCreate()
        {
            // 取得 chef
            var chefId = (int)Session[CDictionary.SK_LOGINED_CHEF_ID];

            // 下拉式 //設定風格 SelectListItem
            var selectListStyle = (new CSearchResultFactory()).Add風格SelectListItem();

            // 下拉式 t服務種類
            var selectListKind = (new CSearchResultFactory()).Add服務種類SelectListItem();

            var vm = new v販售項目VM
            {
                fCID = chefId,
                style = selectListStyle,
                kind = selectListKind
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult productCreate(v販售項目VM vm)
        {
            /*  1.建立項目 2.變更私廚風格 3.變更私廚服務種類 */

            // 模型驗證
            if (ModelState.IsValid)
            {

                if (vm.image != null)
                {
                    // 新項目照片地址
                    vm.f項目照片 = (new 共用方法()).照片更新(vm.image, Server.MapPath("~/"), CDictionary.照片存檔位置_項目, "");
                }
                
                // 1.建立項目
                db.t販售項目.Add(new t販售項目
                {
                    fCID = vm.fCID,
                    fSID = vm.fSID,
                    fKID = vm.fKID,
                    f項目名稱 = vm.f項目名稱,
                    f項目內容 = vm.f項目內容,
                    f項目照片 = vm.f項目照片,
                    f價格 = vm.f價格,
                    f上架 = vm.f上架
                });
                // 儲存DB
                db.SaveChanges();

                // 從販售項目選出私廚風格
                var 私廚風格 = (new Models.Prod.Method()).從販售項目選出私廚風格(vm.fCID);
                // 從販售項目選出私廚服務種類
                var 私廚服務種類 = (new Models.Prod.Method()).從販售項目選出私廚服務種類(vm.fCID);

                // 2.變更私廚風格
                // 3.變更私廚服務種類 // 其他方法: 拆解字串 NOT IN 則加入 >> 刪除處理?
                // 找到此私廚
                var chef = db.t私廚.FirstOrDefault(c => c.fCID == vm.fCID);
                chef.f風格 = 私廚風格;
                chef.f服務種類 = 私廚服務種類;
                // 儲存DB
                db.SaveChanges();
                // 回私廚所有項目 // 至新增-販售項目-菜品 無法取得 fPID
                return RedirectToAction("salesItemList");
            }
            // 後端模型驗證失敗 Form物件回傳
            // 下拉式 //設定風格 SelectListItem
            var selectListStyle = (new CSearchResultFactory()).Add風格SelectListItem();
            // 下拉式 t服務種類
            var selectListKind = (new CSearchResultFactory()).Add服務種類SelectListItem();
            // 上一次所選的值
            selectListStyle.First(s => s.Value == vm.fSID.ToString()).Selected = true;
            selectListKind.First(k => k.Value == vm.fKID.ToString()).Selected = true;
            vm.style = selectListStyle;
            vm.kind = selectListKind;

            return View(vm);
        }

        // === 項目編輯 ===
        public ActionResult productEdit(int fPID)
        {
            // 取得此販售項目
            var prod = db.t販售項目.FirstOrDefault(p => p.fPID == fPID);

            // 下拉式 //設定風格 SelectListItem
            var selectListStyle = (new CSearchResultFactory()).Add風格SelectListItem();
            // 下拉式 t服務種類
            var selectListKind = (new CSearchResultFactory()).Add服務種類SelectListItem();
            // 預設值
            selectListStyle.First(x => x.Value == prod.fSID.ToString()).Selected = true;
            // 預設值
            selectListKind.First(x => x.Value == prod.fKID.ToString()).Selected = true;

            var vm = new v販售項目VM
            {
                fCID = prod.fCID,
                fPID = prod.fPID,
                fSID = prod.fSID,
                fKID = prod.fKID,
                f項目名稱 = prod.f項目名稱,
                f項目內容 = prod.f項目內容,
                f項目照片 = prod.f項目照片,
                f上架 = prod.f上架,
                f價格 = prod.f價格,

                style = selectListStyle,
                kind = selectListKind
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult productEdit(v販售項目VM vm)
        {
            // 後端模型驗證
            if (ModelState.IsValid)
            {
                var prod = db.t販售項目.FirstOrDefault(p => p.fPID == vm.fPID);

                if (vm.image != null)
                {
                    // 照片更新
                    var 新項目照片地址 = (new 共用方法()).照片更新(vm.image, Server.MapPath("~/"), CDictionary.照片存檔位置_項目, prod.f項目照片);
                    prod.f項目照片 = 新項目照片地址;
                }

                prod.fSID = vm.fSID;
                prod.fKID = vm.fKID;
                prod.f項目名稱 = vm.f項目名稱;
                prod.f項目內容 = vm.f項目內容;
                prod.f上架 = vm.f上架;
                prod.f價格 = vm.f價格;
                // 儲存DB
                db.SaveChanges();

                // 從販售項目選出私廚風格
                var 私廚風格 = (new Models.Prod.Method()).從販售項目選出私廚風格(vm.fCID);
                // 從販售項目選出私廚服務種類
                var 私廚服務種類 = (new Models.Prod.Method()).從販售項目選出私廚服務種類(vm.fCID);

                // 2.變更私廚風格
                // 3.變更私廚服務種類
                // 找到此私廚
                var chef = db.t私廚.FirstOrDefault(c => c.fCID == vm.fCID);
                chef.f風格 = 私廚風格;
                chef.f服務種類 = 私廚服務種類;
                // 儲存DB
                db.SaveChanges();

                // 重定向到 販售項目
                return RedirectToAction("salesItemList");
            }
            // 後端模型驗證失敗 Form物件回傳

            // 下拉式 //設定風格 SelectListItem
            var selectListStyle = (new CSearchResultFactory()).Add風格SelectListItem();
            // 下拉式 t服務種類
            var selectListKind = (new CSearchResultFactory()).Add服務種類SelectListItem();
            // 預設值
            selectListStyle.First(x => x.Value == vm.fSID.ToString()).Selected = true;
            // 預設值
            selectListKind.First(x => x.Value == vm.fKID.ToString()).Selected = true;

            vm.style = selectListStyle;
            vm.kind = selectListKind;

            return View(vm);
        }

        // === 項目刪除 === 
        public ActionResult productDelete(int fPID)
        {
            var prod = db.t販售項目.FirstOrDefault(p => p.fPID == fPID);
            if (prod != null)
            {
                db.t販售項目.Remove(prod);
                db.SaveChanges();
            }
            return RedirectToAction("salesItemList");
        }


        // === 菜品創建 === 
        public ActionResult dishCreate(int fPID)
        {
            var prod = db.t販售項目.FirstOrDefault(p => p.fPID == fPID);
            var d = new v菜品VM
            {
                fPID = fPID,
                項目名稱 = prod.f項目名稱
            };
            return View(d);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult dishCreate(v菜品VM vm)
        {
            // 後端模型驗證
            if (ModelState.IsValid)
            {
                if (vm.image != null)
                {
                    // 照片 新增 新菜品照片地址
                    vm.f菜品照片 = (new 共用方法()).照片更新(vm.image, Server.MapPath("~/"), CDictionary.照片存檔位置_菜品, "");
                }
                // 菜品創建
                db.t菜品.Add(new t菜品
                {
                    fPID = vm.fPID,
                    f菜品名稱 = vm.f菜品名稱,
                    f菜品簡介 = vm.f菜品簡介,
                    f菜品照片 = vm.f菜品照片
                });
                // 儲存 DB
                db.SaveChanges();
                // 回私廚所有項目
                return RedirectToAction("salesItemList");
            }
            // 後端模型驗證失敗 Form物件回傳
            return View(vm);
        }


        // === 菜品編輯 === 
        public ActionResult dishEdit(int fVID)
        {
            var vm = from p in db.t販售項目
                     join v in db.t菜品 on p.fPID equals v.fPID
                     where v.fVID == fVID
                     select new v菜品VM
                     {
                         fVID = v.fVID,
                         項目名稱 = p.f項目名稱,
                         f菜品名稱 = v.f菜品名稱,
                         f菜品簡介 = v.f菜品簡介,
                         f菜品照片 = v.f菜品照片
                     };
            return View(vm.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult dishEdit(v菜品VM vm)
        {
            // 後端模型驗證
            if (ModelState.IsValid)
            {
                // 取得此菜品
                var dish = db.t菜品.FirstOrDefault(v => v.fVID == vm.fVID);

                if (vm.image != null)
                {
                    // 照片 更新 
                    var 新菜品照片地址 = (new 共用方法()).照片更新(vm.image, Server.MapPath("~/"), CDictionary.照片存檔位置_菜品, dish.f菜品照片);
                    dish.f菜品照片 = 新菜品照片地址;
                }
                // 更新菜品
                dish.f菜品名稱 = vm.f菜品名稱;
                dish.f菜品簡介 = vm.f菜品簡介;
     
                // 儲存 DB
                db.SaveChanges();
                return RedirectToAction("salesItemList");
            }
            // 後端模型驗證失敗 Form物件回傳
            return View(vm);
        }

        // === 菜品刪除 === 
        public ActionResult dishDelete(int fVID)
        {
            var dish = db.t菜品.FirstOrDefault(p => p.fVID == fVID);
            if (dish != null)
            {
                db.t菜品.Remove(dish);
                db.SaveChanges();
            }
            return RedirectToAction("salesItemList");
        }


        // ===[變更上架]===
        [HttpPost]
        public ActionResult modifyProd(int fPID)
        {
            // 找出 販售項目
            var prod = db.t販售項目.FirstOrDefault(p => p.fPID == fPID);
            if (prod.f上架 == false)
            {
                prod.f上架 = true;
            }
            else
            {
                prod.f上架 = false;
            }
            // 儲存DB
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }


        #endregion

    }
}