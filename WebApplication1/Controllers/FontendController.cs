using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    
    public class FontendController : Controller
    {
        // GET: Fontend

        // 登入
        public ActionResult login()
        {
            return View();
        }

        #region 會員中心{ [註冊 註冊成功] [編輯基本資料 編輯私廚簡介] [會員中心(一般) 會員中心(私廚)] [我的最愛] }

        // 註冊
        public ActionResult register()
        {
            return View();
        }

        // 註冊成功
        public ActionResult registersuccess()
        {
            return View();
        }

        // 編輯基本資料
        public ActionResult memberinfo()
        {
            return View();
        }

        // 編輯私廚簡介
        public ActionResult chefedit()
        {
            return View();
        }

        // 會員中心(私廚)
        public ActionResult chefcenter()
        {
            return View();
        }

        // 會員中心(一般)
        public ActionResult membercenter()
        {
            return View();
        }

        // 我的最愛
        public ActionResult favorite()
        {
            return View();
        }

        #endregion

        #region 交易紀錄{ [儲值點數 儲值成功] [交易紀錄] [私廚資訊] [客戶資訊] [填寫評價] [品項訂購]}

        // 儲值點數
        public ActionResult buypoint() // OK 需測試
        {
            return View();
        }

        // 儲值成功
        public ActionResult buypointsuccess() // OK 需測試
        {
            return View();
        }

        // 品項訂購
        public ActionResult salesitem()
        {
            return View();
        }

        // 交易紀錄
        public ActionResult transaction() // OK 需測試
        {
            return View();
        }

        // 私廚資訊
        public ActionResult chefinfo() // OK 需測試
        {
            return View();
        }

        // 客戶資訊
        public ActionResult clientinfo() // OK 需測試
        {
            return View();
        }

        // 填寫評價
        public ActionResult evaluate() // OK 需測試
        {
            return View();
        }

        #endregion

        #region 私廚{ [私廚販售項目清單] [新增販售項目] [新增菜色] [設定可預訂時間] }

        // 私廚販售項目清單
        public ActionResult salesitemlist()
        {
            return View();
        }

        // 新增販售項目
        public ActionResult productinfo()
        {
            return View();
        }

        // 新增 - 販售項目 - 菜色
        public ActionResult dishesinfo()
        {
            return View();
        }

        // 設定可預訂時間
        public ActionResult calendar()
        {
            return View();
        }

        #endregion

        // 首頁 
        public ActionResult index()
        {
            return View();
        }

        // 搜尋結果 
        public ActionResult searchlist()
        {
            return View();
        }




    }
}