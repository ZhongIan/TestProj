using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Common;
using WebApplication1.ViewModels.Acc;
using System.Net.Mail;


namespace WebApplication1.Controllers
{

    public class AccController : Controller
    {
        // GET: Acc

        private Database1Entities db = new Database1Entities();

        // === 登入 ===
        public ActionResult login()
        {
            return View();
        }

        [HttpPost] // 限定 POST
        [ValidateAntiForgeryToken] // 防止 CSRF 跨域偽造請求, 只有從頁面來的才能通過
        public ActionResult login(v登入VM 登入VM)
        {
            // 模型驗證
            if (ModelState.IsValid)
            {
                // 帳密驗證
                var user = db.t會員.FirstOrDefault(u =>
                    u.f帳號 == 登入VM.帳號 && u.f密碼 == 登入VM.密碼
                );
                if (user != null)
                {
                    // 存進 Session
                    Session[CDictionary.SK_LOGINED_USER_ID] = user.fUID;

                    if (Session[CDictionary.SK_PAGE_BEFORE_LOGIN] != null)
                    {
                        // 回到上一頁
                        var page = Session[CDictionary.SK_PAGE_BEFORE_LOGIN] as page;
                        return RedirectToAction(page.action, page.controller, page.urlArgs);
                    }
                    else
                    {
                        // 重定向到 會員中心
                        return RedirectToAction("center");
                    }

                }
                else
                {
                    ModelState.AddModelError("帳號", "帳密錯誤，登入失敗");
                    ModelState.AddModelError("密碼", "帳密錯誤，登入失敗");
                }
            }
            return View(登入VM);
        }

        // === 登出 ===
        public ActionResult Logout()
        {
            //清除Session變數資料
            Session.Clear();
            // 回燈入頁
            return RedirectToAction("login");
        }

        #region 會員中心{ [註冊 註冊成功] [編輯基本資料 編輯私廚簡介] [會員中心(一般) 會員中心(私廚)] [我的最愛] }

        //============================================================================================
        // 會員中心{ [註冊 註冊成功] [編輯基本資料 編輯私廚簡介] [會員中心(一般) 會員中心(私廚)] [我的最愛] }
        //           私廚創建 私廚編輯
        //============================================================================================


        // === [會員中心(一般) 會員中心(私廚)] ===
        [MyAuthorize(權限 = e會員_權限.一般)] // 確認是否登入
        public ActionResult center()
        {
            //由 Session 限定 登入後使用
            //if (Session[CDictionary.SK_LOGINED_USER_ID] == null)
            //{
            //    // 重定向到 login
            //    return RedirectToAction("login", "Order");
            //}


            int userId = (int)Session[CDictionary.SK_LOGINED_USER_ID];

            var user = (from u in db.t會員
                        join c in db.t私廚
                             on u.fUID equals c.fUID into mixin
                        from x in mixin.DefaultIfEmpty() // LEFT JOIN
                        where u.fUID == userId
                        select new v會員中心VM
                        {
                            姓名 = u.f姓名,
                            暱稱 = u.f暱稱,
                            會員照片 = u.f會員照片,
                            地址 = u.f居住縣市 + u.f詳細地址,
                            生日 = u.f生日,
                            電話 = u.f電話,
                            電子郵件 = u.f電子郵件,
                            點數 = u.f點數,
                            fCID = x.fCID,
                            服務地區 = x.f服務地區,
                            私廚簡介 = x.f私廚簡介,
                            服務項目 = x.f服務種類,
                            餐飲風格 = x.f風格
                        }).FirstOrDefault();

            // 成為私廚檢查
            if (user.姓名 == "" || user.會員照片 == CDictionary.預設NOIMAGE)
            {
                // 回編輯基本資料
                TempData[CDictionary.TK_Msg_ChefCreate] = "請填妥基本資料";
            }

            // 如果有 私廚身分
            if (user.fCID != null && Session[CDictionary.SK_LOGINED_CHEF_ID] == null)
            {
                // 存進 Session
                Session[CDictionary.SK_LOGINED_CHEF_ID] = user.fCID;
            }

            return View(user);
        }


        // === 註冊 ===
        public ActionResult register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult register(v註冊簡易VM vm)
        {
            // 後端模型驗證
            if (ModelState.IsValid)
            {
                // 判斷帳號是否存在
                var acc = db.t會員.FirstOrDefault(m => m.f帳號 == vm.帳號);
                if (acc != null)
                {
                    //@ViewBag.Msgaccount = "帳號已存在";

                    // 在帳號底下 ValidationMessageFor 新增 "帳號已存在"
                    ModelState.AddModelError("帳號", "帳號已存在");
                    return View();
                }

                var email = db.t會員.FirstOrDefault(m => m.f電子郵件 == vm.電子信箱);
                if (email != null)
                {
                    //@ViewBag.Msgaccount = "信箱已存在";

                    // 在電子信箱底下 ValidationMessageFor 新增 "信箱已存在"
                    ModelState.AddModelError("電子信箱", "信箱已存在");
                    return View();
                }

                t會員 member = new t會員
                {
                    f帳號 = vm.帳號,
                    f密碼 = vm.密碼,
                    f電子郵件 = vm.電子信箱
                };

                db.t會員.Add(member);
                db.SaveChanges();
                //註冊成功跳至成功畫面.以下為寄信



                return RedirectToAction("registersuccess");
            }
            //模型驗證失敗 Form物件回傳
            return View();
        }


        // === 註冊成功 ===
        public ActionResult registersuccess()
        {
            return View();
        }


        // === [編輯基本資料] ===
        [MyAuthorize(權限 = e會員_權限.一般)] // 確認是否登入
        public ActionResult memberinfo()
        {

            var userId = (int)Session[CDictionary.SK_LOGINED_USER_ID];

            var vm = (from u in db.t會員
                      select new v基本資料VM
                      {
                          fUID = u.fUID,
                          f姓名 = u.f姓名,
                          f暱稱 = u.f暱稱,
                          f電話 = u.f電話,
                          f生日 = u.f生日,
                          f居住縣市 = u.f居住縣市,
                          f詳細地址 = u.f詳細地址, // 會員中心 地址 = f居住縣市 + f詳細地址
                          f會員照片 = u.f會員照片

                      }).FirstOrDefault(m => m.fUID == userId);

            // 下拉式選單 地區
            var 地區 = CDictionary.地區.Select(x => new SelectListItem
            {
                Text = x,
                Value = x
            }).ToList();
            // 預設值
            if (vm.f居住縣市 != "")
                地區.First(x => x.Value == vm.f居住縣市);
            vm.Select地區 = 地區;


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult memberinfo(v基本資料VM vm)
        {
            // 後端模型驗證
            if (ModelState.IsValid)
            {
                var acc = db.t會員.FirstOrDefault(u => u.fUID == vm.fUID);

                if (vm.image != null)
                {
                    // 照片更新
                    var 新會員照片地址 = (new 共用方法()).照片更新(vm.image, Server.MapPath("~/"), CDictionary.照片存檔位置_會員, acc.f會員照片);
                    acc.f會員照片 = 新會員照片地址;
                }

                acc.f姓名 = vm.f姓名;
                acc.f暱稱 = vm.f暱稱 ?? "匿名";
                acc.f電話 = vm.f電話;
                acc.f生日 = vm.f生日;
                acc.f居住縣市 = vm.f居住縣市;
                acc.f詳細地址 = vm.f詳細地址;

                // 照片更新
                db.SaveChanges();
                // 重定向到 login
                return RedirectToAction("center", "Acc");
            }
            // 後端模型驗證失敗 Form物件回傳 
            // 下拉式選單
            var 地區 = CDictionary.地區.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,

            }).ToList();
            // 預設值
            地區.First(x => x.Value == vm.f居住縣市).Selected = true;

            vm.Select地區 = 地區;
            return View(vm);
        }


        // === [私廚創建] ===
        [MyAuthorize(權限 = e會員_權限.一般)] // 確認是否登入
        public ActionResult chefCreate()
        {
            // 下拉式 地區
            var dropdown_地區 = new List<SelectListItem> {
                new SelectListItem { Text = "請選擇地區", Disabled = true, Selected = true }
            };
            dropdown_地區.AddRange(
                CDictionary.地區.Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x
                }).ToList()
            );

            var vm = new v私廚VM
            {
                // 取得登入後的 USER_ID
                fUID = (int)Session[CDictionary.SK_LOGINED_USER_ID],
                Select地區 = dropdown_地區
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult chefCreate(v私廚VM vm)
        {
            /* 1.變更權限 2.建立私廚 */

            // 後端模型驗證
            if (ModelState.IsValid)
            {
                // 取得此使用者
                var user = db.t會員.FirstOrDefault(u => u.fUID == vm.fUID);

                // 1.變更權限
                user.f權限 = e會員_權限.私廚.GetHashCode();

                // 2.建立私廚
                db.t私廚.Add(new t私廚
                {
                    fUID = vm.fUID,
                    f服務地區 = vm.f服務地區,
                    f私廚簡介 = vm.f私廚簡介
                });
                // 儲存DB
                db.SaveChanges();
                // 回會員中心
                return RedirectToAction("center");
            }
            // 後端模型驗證失敗 Form物件回傳
            // 預設值
            else
            {
                var dropdown_地區 = new List<SelectListItem>();
                dropdown_地區.Add(new SelectListItem { Text = "請選擇地區", Disabled = true, Selected = true });
                dropdown_地區.AddRange(
                    CDictionary.地區.Select(x => new SelectListItem
                    {
                        Value = x.ToString(),
                        Text = x
                    }).ToList()
                );

                dropdown_地區.First(x => x.Value == vm.f服務地區);
                vm.Select地區 = dropdown_地區;
            }
            return View(vm);
        }



        // === 私廚編輯 ===
        [MyAuthorize(權限 = e會員_權限.私廚)] // 確認是否登入
        public ActionResult chefEdit()
        {

            // 取得 chef
            var chefId = (int)Session[CDictionary.SK_LOGINED_CHEF_ID];
            var vm = (from c in db.t私廚
                      where c.fCID == chefId
                      select new v私廚VM
                      {
                          fUID = c.fUID,
                          f服務地區 = c.f服務地區,
                          f私廚簡介 = c.f私廚簡介
                      }).FirstOrDefault();
            // 下拉式 地區
            var dropdown_地區 = CDictionary.地區.Select(x => new SelectListItem
            {
                Text = x,
                Value = x
            }).ToList();
            // 預設值
            dropdown_地區.First(x => x.Value == vm.f服務地區);
            vm.Select地區 = dropdown_地區;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult chefEdit(v私廚VM vm)
        {
            // 後端模型驗證
            if (ModelState.IsValid)
            {
                var chef = db.t私廚.FirstOrDefault(c => c.fUID == vm.fUID);
                chef.f服務地區 = vm.f服務地區;
                chef.f私廚簡介 = vm.f私廚簡介;
                db.SaveChanges();

                return RedirectToAction("center");
            }
            // 後端模型驗證失敗 Form物件回傳
            // 下拉式 地區
            var dropdown_地區 = CDictionary.地區.Select(x => new SelectListItem
            {
                Text = x,
                Value = x
            }).ToList();
            // 預設值
            dropdown_地區.First(x => x.Value == vm.f服務地區);
            vm.Select地區 = dropdown_地區;

            return View(vm);
        }

        // === [我的最愛] ===
        [MyAuthorize(權限 = e會員_權限.一般)] // 確認是否登入
        public ActionResult favorite()
        {
            int fUID = (int)Session[CDictionary.SK_LOGINED_USER_ID];

            var chefProp = from f in db.t我的最愛
                           join p in db.t販售項目 on f.fPID equals p.fPID
                           join c in db.t私廚 on p.fCID equals c.fCID
                           join u in db.t會員 on c.fUID equals u.fUID
                           where f.fUID == fUID
                           select new v我的最愛VM
                           {
                               fUID = fUID,
                               fPID = p.fPID,
                               價格 = p.f價格,
                               項目名稱 = p.f項目名稱,
                               項目照片 = p.f項目照片,
                               私廚姓名 = u.f姓名,
                               私廚照片 = u.f會員照片,
                               私廚評級 = c.f私廚評級
                           };
            var Mylove = chefProp.ToList();
            return View(Mylove);
        }


        // ===[變更我的最愛]===
        [MyAuthorize(權限 = e會員_權限.一般)] //確認是否登入
        public ActionResult modifyFavorite(int fPID)
        {
            int fUID = (int)Session[CDictionary.SK_LOGINED_USER_ID];
            // 找出我的最愛
            var fav = db.t我的最愛.FirstOrDefault(f => f.fUID == fUID && f.fPID == fPID);
            if (fav == null)
            {
                db.t我的最愛.Add(new t我的最愛
                {
                    fUID = fUID,
                    fPID = fPID
                });
            }
            else
            {
                db.t我的最愛.Remove(fav);
            }
            // 儲存DB
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 測試用

        //==================================================================================
        // 測試用
        //==================================================================================

        public ActionResult List()
        {
            var table = db.t會員.Select(u => u);
            var list = table.ToList();
            return View(list);
        }

        public ActionResult Delete(int fUID)
        {
            var acc = db.t會員.FirstOrDefault(a => a.fUID == fUID);
            if (acc != null)
            {
                db.t會員.Remove(acc);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }
        #endregion

    }
}