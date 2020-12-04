using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Common;
using WebApplication1.ViewModels.Order;
using System.Net.Mail;

namespace WebApplication1.Controllers
{
    [MyAuthorize(權限 = e會員_權限.一般)] // 登入驗證 會員
    public class OrderController : Controller
    {
        // GET: Order
        private Database1Entities db = new Database1Entities();

        #region 交易紀錄{ [儲值點數 儲值成功] [交易紀錄] [私廚資訊] [客戶資訊] [填寫評價] [品項訂購]}

        //==================================================================================
        // 交易紀錄{ [儲值點數 儲值成功] [交易紀錄] [私廚資訊] [客戶資訊] [填寫評價] [品項訂購]}
        //          私廚點選完成服務
        //==================================================================================


        // === [儲值點數] ===
        public ActionResult buyPoint()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult buyPoint(v儲值點數VM buyPointVM)
        {
            if (ModelState.IsValid)
            {
                // 儲存進 [t儲值點數]
                int userId = (int)Session[CDictionary.SK_LOGINED_USER_ID];
                var newPoint = new t儲值點數
                {
                    fUID = userId,
                    f點數 = buyPointVM.f點數
                };
                db.t儲值點數.Add(newPoint);
                // 找到使用者 變更 [f點數]
                var user = db.t會員.FirstOrDefault(u => u.fUID == userId);
                user.f點數 += buyPointVM.f點數;
                db.SaveChanges();

                return RedirectToAction("buyPointSuccess");
            }
            return View(buyPointVM);
        }


        // === [儲值成功] 重定向到 [交易紀錄] transaction ===
        public ActionResult buyPointSuccess()
        {
            return View();
        }

        // === [交易紀錄] ===
        public ActionResult transaction()
        {
            // 由 Session 限定 登入後使用 
            if (Session[CDictionary.SK_LOGINED_USER_ID] == null)
            {
                // 重定向到 login
                return RedirectToAction("login", "Acc");
            }

            // 找到登入的使用者
            var userId = (int)Session[CDictionary.SK_LOGINED_USER_ID];
            var user = db.t會員.FirstOrDefault(u => u.fUID == userId);

            // 取得 全部 交易紀錄
            var all = db.sp_交易紀錄(user.fUID).ToList();

            var 未完成 = from x in all
                      where x.狀態 == e訂單狀態.客戶預訂.GetHashCode().ToString()
                      select x;

            var 待評價 = from x in all
                      where x.狀態 == e訂單狀態.私廚確認_開放客戶評價.GetHashCode().ToString()
                      select x;

            var 已完成 = from x in all
                      where x.狀態 == e訂單狀態.客戶確認_完成評價.GetHashCode().ToString()
                      select x;


            var 交易紀錄 = new v交易紀錄VM
            {
                點數餘額 = user.f點數,
                交易紀錄 = all,
                交易紀錄_未完成 = 未完成.ToList(),
                交易紀錄_待評價 = 待評價.ToList(),
                交易紀錄_已完成 = 已完成.ToList()
            };

            return View(交易紀錄);
        }

        // === 私廚點選完成服務 ===
        public ActionResult finish(int fOID)
        {
            /* 1. 更改 [訂單][狀態] 私廚確認_開放客戶評價 */

            // 取得此筆訂單
            var order = db.t訂單.FirstOrDefault(o => o.fOID == fOID);
            // 1.更改[訂單][狀態] 私廚確認_開放客戶評價
            order.f狀態 = e訂單狀態.私廚確認_開放客戶評價.GetHashCode();
            // 儲存DB
            db.SaveChanges();

            return RedirectToAction("transaction", "Order");
        }

        // === [私廚資訊] ===
        [AllowAnonymous]
        public ActionResult chefInfo(int fCID)
        {
            // 1.私廚資訊 2.私廚販售項目清單

            // 私廚販售項目清單
            var chefProp = from p in db.t販售項目
                           join s in db.t風格 on p.fSID equals s.fSID
                           join k in db.t服務種類 on p.fKID equals k.fKID
                           where p.fCID == fCID && p.f上架 == true
                           select new ChefProduct
                           {
                               fPID = p.fPID,
                               項目名稱 = p.f項目名稱,
                               價格 = p.f價格,
                               風格 = s.f風格,
                               服務種類 = k.f服務種類,
                               項目照片 = p.f項目照片
                           };

            // 私廚資訊
            var linq = from c in db.t私廚
                       join u in db.t會員 on c.fUID equals u.fUID
                       where c.fCID == fCID
                       select new v私廚資訊VM
                       {
                           私廚姓名 = u.f姓名,
                           私廚照片 = u.f會員照片,
                           私廚評級 = c.f私廚評級,
                           電子郵件 = u.f電子郵件,
                           服務地區 = c.f服務地區,
                           餐飲風格 = c.f風格,
                           服務種類 = c.f服務種類,
                           私廚簡介 = c.f私廚簡介,
                           項目清單 = chefProp.ToList()
                       };

            var chefinfo = linq.FirstOrDefault();

            return View(chefinfo);
        }

        // === [客戶資訊] ===
        public ActionResult clientInfo(int fUID)
        {
            var user = db.t會員.FirstOrDefault(u => u.fUID == fUID);
            if (user != null)
            {
                var 客戶資訊 = new v客戶資訊VM
                {
                    客戶姓名 = user.f姓名,
                    客戶暱稱 = user.f暱稱,
                    客戶照片 = user.f會員照片,
                    電子郵件 = user.f電子郵件,
                    電話 = user.f電話
                };
                return View(客戶資訊);
            }
            return View();
        }

        // === [填寫評價] ===
        public ActionResult evaluate(int fOID)
        {

            var linq = from o in db.t訂單
                       join p in db.t販售項目 on o.fPID equals p.fPID
                       join k in db.t服務種類 on p.fKID equals k.fKID
                       join c in db.t私廚 on p.fCID equals c.fCID
                       join u in db.t會員 on c.fUID equals u.fUID
                       where o.fOID == fOID
                       select new v評價VM
                       {
                           預定日期 = o.f預定日期,
                           私廚名稱 = u.f姓名,
                           私廚照片 = u.f會員照片,
                           私廚評級 = c.f私廚評級,
                           項目名稱 = p.f項目名稱,
                           項目照片 = p.f項目照片,
                           價格 = p.f價格,
                           服務種類 = k.f服務種類,
                           fOID = fOID
                       };

            return View(linq.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult evaluate(v評價VM vm)
        {
            /*
             1. 更變訂單狀態 私廚確認_開放客戶評價 -> 客戶確認_完成評價
             2. 變更 [私廚] 的評級 : 由 [訂單] 平均算出
             3. 依[訂單][數量] * [v評價VM][價格] 加入至私廚的 [會員][點數]
             */

            // 後端模型驗證
            if (ModelState.IsValid)
            {
                // 取得那筆訂單
                var order = db.t訂單.FirstOrDefault(o => o.fOID == vm.fOID);

                if (order != null)
                {
                    order.f評級 = vm.評級;
                    order.f評價內容 = vm.評價內容;
                    order.f評價日期 = DateTime.Now.ToString("g");
                    // 1. 更變訂單狀態 私廚確認_開放客戶評價 -> 客戶確認_完成評價
                    order.f狀態 = e訂單狀態.客戶確認_完成評價.GetHashCode();
                    // 儲存DB
                    db.SaveChanges();
                    // 找到此訂單的 chef
                    var chef = (from c in db.t私廚
                                join p in db.t販售項目 on c.fCID equals p.fCID
                                where p.fPID == order.fPID
                                select c).FirstOrDefault(); 
                    // 取得此私廚的所有訂單 平均星星評級
                    var 訂單私廚評價 = (new Models.Order.Method()).從訂單平均私廚評價(chef.fCID);
                    // 2. 變更 [私廚] 的評級 : 由 [訂單] 平均算出
                    chef.f私廚評級 = 訂單私廚評價.平均評級;
                    chef.f私廚評級筆數 = 訂單私廚評價.總筆數;

                    // 取得此私廚的 會員資料
                    var chefUser = db.t會員.FirstOrDefault(u => u.fUID == chef.fUID);
                    // 3.依[訂單][數量] * [v評價VM][價格] 加入至私廚的[會員][點數]
                    chefUser.f點數 += (int)Math.Round(order.f數量 * vm.價格 * 0.9, 0);

                    // 儲存DB
                    db.SaveChanges();
                }
                // 重定向到 交易紀錄
                return RedirectToAction("transaction");
            }
            // 保留原資料回傳
            return View(vm);
        }

        // === [品項訂購] ===
        [AllowAnonymous]
        public ActionResult salesItem(int fPID, string f日期, string f時段)
        {
            if (Session[CDictionary.SK_LOGINED_USER_ID] == null)
            {
                Session[CDictionary.SK_PAGE_BEFORE_LOGIN] = new page
                {
                    controller = "Order",
                    action = "salesItem",
                    urlArgs = new { fPID = fPID, f日期 = f日期, f時段 = f時段 }
                };
            }

            // 取得此販售項目的菜品清單
            var 菜品_linq = from p in db.t販售項目
                          join v in db.t菜品 on p.fPID equals v.fPID
                          where p.fPID == fPID
                          select v;

            // 取得此販售項目的 顧客評價
            var 訂單狀態 = e訂單狀態.客戶確認_完成評價.GetHashCode();
            var 顧客評價_linq = from o in db.t訂單
                            join u in db.t會員 on o.fUID equals u.fUID
                            where o.f狀態 == 訂單狀態 &&
                              o.fPID == fPID
                            select new 顧客評價
                            {
                                暱稱 = u.f暱稱,
                                顧客照片 = u.f會員照片,
                                顧客評級 = o.f評級,
                                評價日期 = o.f評價日期,
                                評價內容 = o.f評價內容
                            };
            // 取得此販售項目的所有細節
            var 項目細節_linq = from p in db.t販售項目
                            join s in db.t風格 on p.fSID equals s.fSID
                            join k in db.t服務種類 on p.fKID equals k.fKID
                            join c in db.t私廚 on p.fCID equals c.fCID
                            join u in db.t會員 on c.fUID equals u.fUID
                            where p.fPID == fPID
                            select new v項目訂購VM
                            {
                                fCID = p.fCID,
                                fPID = p.fPID,
                                項目照片 = p.f項目照片,
                                項目名稱 = p.f項目名稱,
                                項目內容 = p.f項目內容,
                                價格 = p.f價格,
                                服務種類 = k.f服務種類,
                                風格 = s.f風格,
                                私廚姓名 = u.f姓名,
                                私廚照片 = u.f會員照片,
                                私廚評級 = c.f私廚評級,
                                服務地區 = c.f服務地區,
                                私廚簡介 = c.f私廚簡介,
                                私廚評級筆數 = c.f私廚評級筆數,
                                菜品清單 = 菜品_linq.ToList(),
                                顧客評價清單 = 顧客評價_linq.ToList()
                            };
            var 項目細節 = 項目細節_linq.FirstOrDefault();
            // 取得此項目的私廚的可預訂時間
            項目細節.Select日期 = (new Models.Order.Method()).get私廚可預訂時間(項目細節.fCID);
            // 來自 收尋結果
            if (!string.IsNullOrEmpty(f日期) && !string.IsNullOrEmpty(f時段))
            {
                DateTime date = Convert.ToDateTime(f日期).Date;
                int 時段 = Convert.ToInt32(f時段);
                var theTime = db.t私廚可預訂時間.FirstOrDefault(t =>
                            t.fCID == 項目細節.fCID &&
                            t.f日期 == date &&
                            t.f時段 == 時段);

                項目細節.fTID = theTime.fTID;
            }

            return View(項目細節);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult salesItem(v項目訂購VM vm)
        {
            /* 1.確認客戶點數 2.扣除客戶點數 3.更變私廚可預訂時間 4.下訂  */

            // 後端模型驗證
            if (ModelState.IsValid)
            {
                // 取得 Session userId
                int userId = (int)Session[CDictionary.SK_LOGINED_USER_ID];
                // 取得 客戶
                var user = db.t會員.FirstOrDefault(u => u.fUID == userId);
                // 1.確認客戶點數
                if (user.f點數 < vm.價格 * vm.數量)
                {
                    TempData[CDictionary.TK_Msg_SalesItem] = "點數餘額不足";
                    return RedirectToAction("salesItem", new { fPID = vm.fPID }); //重整
                }
                else
                {
                    // 2.扣除客戶點數
                    user.f點數 -= vm.價格 * vm.數量;
                }

                // 取得日期
                var date = db.t私廚可預訂時間.FirstOrDefault(t => t.fTID == vm.fTID);
                // 3.更變私廚可預訂時間
                date.f狀態 = e私廚可預訂_時段_狀態.被預訂_不可修改.GetHashCode();

                string 預定日期 = date.f日期.ToString("d");

                // 預定日期 = "2020/12/12 午餐"
                if (date.f時段 == e私廚可預訂_時段.午餐.GetHashCode())
                    預定日期 += " 午餐";
                else if (date.f時段 == e私廚可預訂_時段.晚餐.GetHashCode())
                    預定日期 += " 晚餐";

                // 4.下訂
                db.t訂單.Add(new t訂單
                {
                    fPID = vm.fPID,
                    fUID = userId,
                    f預定日期 = 預定日期,
                    f數量 = vm.數量
                });
                // 儲存DB
                db.SaveChanges();

                // 取得此產品資料 私廚
                var linq = from p in db.t販售項目
                           join s in db.t風格 on p.fSID equals s.fSID
                           join k in db.t服務種類 on p.fKID equals k.fKID
                           join c in db.t私廚 on p.fCID equals c.fCID
                           join u in db.t會員 on c.fUID equals u.fUID
                           where p.fPID == vm.fPID
                           select new
                           {
                               私廚電子郵件 = u.f電子郵件,
                               風格 = s.f風格,
                               服務種類 = k.f服務種類,
                               項目名稱 = p.f項目名稱,
                               項目內容 = p.f項目內容
                           };
                var prod = linq.FirstOrDefault();

                //以下為寄信
                MailMessage mail = new MailMessage();
                //前面是發信email後面是顯示的名稱
                mail.From = new MailAddress("xxx@gmail.com", "Chef's Foody");

                //收信者email
                mail.To.Add(prod.私廚電子郵件);

                //設定優先權
                mail.Priority = MailPriority.Normal;

                //標題
                mail.Subject = "您的商品'" + prod.項目名稱 + "'已被訂購";
                //採用html格式
                mail.IsBodyHtml = true;
                //內容
                mail.Body = "<span style = 'color:red;'>此為系統自動通知信，請勿直接回信！</span>" +
                     "<h1>客戶資訊</h1>" +
                           "<h3>" + user.f姓名 + "</h3>" +
                           "<h4><b>暱 稱:</b><span>" + user.f暱稱 + "</span></h4>" +
                           "<h4><b>電 話:</b><span>" + user.f電話 + "</span></h4>" +
                           "<h4><b>電子郵件:</b><span>" + user.f電子郵件 + "</span></h4>" +
                           "<h4><b>居住地址:</b><span>" + user.f居住縣市 + user.f詳細地址 + "</span></h4>" +
                           "<hr/>" +
                           "<h1>產品資訊</h1>" +
                           "<h4><b>風格:</b><span>" + prod.風格 + "</span></h4>" +
                           "<h4><b>服務種類:</b><span>" + prod.服務種類 + "</span></h4>" +
                           "<h4><b>產品名稱:</b><span>" + prod.項目名稱 + "</span></h4>" +
                           "<h4><b>產品內容:</b><span>" + prod.項目內容 + "</span></h4>" +
                           "<h4><b>單價:NT$</b><span>" + vm.價格 + "</span></h4>" +
                           "<h4><b>數量:</b><span>" + vm.數量 + "份</span></h4>" +
                           "<h4><b>總金額:NT$</b><span>" + vm.價格 * vm.數量 + "</span></h4>";

                //內容使用html
                mail.IsBodyHtml = true;

                //設定gmail的smtp (這是google的)
                SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

                //您在gmail的帳號密碼
                MySmtp.Credentials = new System.Net.NetworkCredential("testemail831014@gmail.com", "se0988502177");

                //開啟ssl
                MySmtp.EnableSsl = true;

                //發送郵件
                MySmtp.Send(mail);

                //放掉宣告出來的MySmtp
                MySmtp = null;

                //放掉宣告出來的mail
                mail.Dispose();

                // 至 交易紀錄
                return RedirectToAction("salesItemSuccess");
            }

            return RedirectToAction("salesItem", new { fPID = vm.fPID }); //重整
        }


        public ActionResult salesItemSuccess()
        {
            return View();
        }

        #endregion


        #region 測試用

        //==================================================================================
        // 測試用
        //==================================================================================

        public ActionResult List()
        {
            var table = db.t訂單.Select(o => o);
            var list = table.ToList();

            return View(list);
        }

        public ActionResult Create()
        {

            var selectList = db.t訂單.Select(t => t).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Create(t訂單 o)
        {

            db.t訂單.Add(o);
            db.SaveChanges();

            return RedirectToAction("List");
        }

        public ActionResult Edit(int fOID)
        {
            var order = db.t訂單.FirstOrDefault(o => o.fOID == fOID);

            return View(order);
        }

        [HttpPost]
        public ActionResult Edit(t訂單 modify)
        {
            var order = db.t訂單.FirstOrDefault(o => o.fOID == modify.fOID);

            if (order != null)
            {
                order.fPID = modify.fPID;
                order.fUID = modify.fUID;
                order.f評價日期 = modify.f評價日期;
                order.f數量 = modify.f數量;
                order.f狀態 = modify.f狀態;
                order.f訂購日期 = modify.f訂購日期;
                order.f評價內容 = modify.f評價內容;
                order.f評級 = modify.f評級;
                order.f預定日期 = modify.f預定日期;

                db.SaveChanges();
            }

            return RedirectToAction("List");
        }

        public ActionResult Delete(int fOID)
        {
            var order = db.t訂單.FirstOrDefault(o => o.fOID == fOID);
            if (order != null)
            {
                db.t訂單.Remove(order);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        #endregion

    }
}