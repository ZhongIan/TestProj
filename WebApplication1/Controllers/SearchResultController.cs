using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models.Common;
using WebApplication1.Models.SearchResult;
using WebApplication1.ViewModels.Home;

namespace WebApplication1.Controllers
{
    public class SearchResultController : Controller
    {
        // GET: SearchResult

        private Database1Entities db = new Database1Entities();

        public ActionResult SearchByCondition(CSearchResult vm)
        {
            if (Session[CDictionary.SK_LOGINED_USER_ID] == null)
            {
                Session[CDictionary.SK_PAGE_BEFORE_LOGIN] = new page
                {
                    controller = "SearchResult",
                    action = "SearchByCondition",
                    urlArgs = new { vm.地區, vm.風格, vm.f日期, vm.時段, vm.服務種類 }
                };
            }

            

            if (vm.風格 != null && vm.地區 != null && vm.服務種類 != null && vm.時段 != null && vm.f日期 != null)
            {

                //將搜尋條件指定給變數代入 Factory 方法取得資料庫資料
                string k風格 = vm.風格.ToString();//Search bar attr1
                string k地區 = vm.地區.ToString();//Search bar attr2
                string k服務種類 = vm.服務種類.ToString();//Search bar attr3
                string k時段 = vm.時段.ToString();//Search bar attr4
                string k日期 = vm.f日期.ToString();//Search bar attr5
                int? LoginID = (int?)Session[CDictionary.SK_LOGINED_USER_ID];

                var linq = from p in db.t販售項目
                           join c in db.t私廚 on p.fCID equals c.fCID
                           join u in db.t會員 on c.fUID equals u.fUID
                           join t in db.t私廚可預訂時間 on c.fCID equals t.fCID
                           join s in db.t風格 on p.fSID equals s.fSID
                           join k in db.t服務種類 on p.fKID equals k.fKID
                           join f in db.t我的最愛.Where(f => LoginID == null || f.fUID == LoginID)
                                on p.fPID equals f.fPID
                           where
                                p.f上架 == true &&
                                (vm.風格 == null || s.fSID == Convert.ToInt32(vm.風格)) &&
                                (vm.地區 == null || c.f服務地區 == vm.地區) &&
                                (vm.服務種類 == null || k.fKID == Convert.ToInt32(vm.服務種類)) &&
                                (vm.時段 == null || t.f時段 == Convert.ToInt32(vm.時段)) &&
                                (vm.f日期 == null || t.f日期 == Convert.ToDateTime(vm.f日期))
                           select new SearchProduct
                           {
                               fCID = c.fCID,
                               f私廚姓名 = u.f姓名,
                               f會員照片 = u.f會員照片,
                               f私廚評級 = c.f私廚評級,
                               fPID = p.fPID,
                               f價格 = p.f價格,
                               f項目名稱 = p.f項目名稱,
                               f項目照片 = p.f項目照片,
                               f服務種類 = k.f服務種類,
                               f風格 = s.f風格
                           };

                List < SearchProduct > Productlist = new List<SearchProduct>();
                Productlist = (new CSearchResultFactory()).GetCSearchResultsByCondition(k風格, k地區, k服務種類, k日期, k時段, LoginID);

                var list = new CSearchResult
                {
                    搜尋結果 = Productlist,
                    f日期 = k日期,
                    時段 = vm.時段
                };
                //設定地區 SelectListItem
                list.f地區 = (new CSearchResultFactory()).Add地區SelectListItem();

                //設定時段 SelectListItem
                list.f時段 = (new CSearchResultFactory()).Add時段SelectListItem();

                //設定風格 SelectListItem
                list.f風格 = (new CSearchResultFactory()).Add風格SelectListItem();

                //設定服務種類 SelectListItem
                list.f服務種類 = (new CSearchResultFactory()).Add服務種類SelectListItem();

                return View("SearchByCondition", list);
            }
            else
            {
                var list = new CSearchResult();

                //設定地區 SelectListItem
                list.f地區 = (new CSearchResultFactory()).Add地區SelectListItem();

                //設定時段 SelectListItem
                list.f時段 = (new CSearchResultFactory()).Add時段SelectListItem();

                //設定風格 SelectListItem
                list.f風格 = (new CSearchResultFactory()).Add風格SelectListItem();

                //設定服務種類 SelectListItem
                list.f服務種類 = (new CSearchResultFactory()).Add服務種類SelectListItem();

                return View("SearchByCondition", list);
            }
        }

        public ActionResult SearchByKeyWord(CSearchResult vm)
        {
            if (Session[CDictionary.SK_LOGINED_USER_ID] == null)
            {
                Session[CDictionary.SK_PAGE_BEFORE_LOGIN] = new page
                {
                    controller = "SearchResult",
                    action = "SearchByKeyWord",
                    urlArgs = new { vm.txtkeyword }
                };
            }

            //將關鍵字指定給變數代入 Factory 方法取得資料庫資料
            string keyWord = vm.txtkeyword;
            int? LoginID = (int?)Session[CDictionary.SK_LOGINED_USER_ID];
            if (keyWord != null)
            {
                List<SearchProduct> Productlist = new List<SearchProduct>();
                Productlist = (new CSearchResultFactory()).GetCSearchResultsByKeyWord(keyWord , LoginID);
                var list = new CSearchResult
                {
                    搜尋結果 = Productlist,
                    txtkeyword = keyWord
                };
                //設定地區 SelectListItem
                list.f地區 = (new CSearchResultFactory()).Add地區SelectListItem();

                //設定時段 SelectListItem
                list.f時段 = (new CSearchResultFactory()).Add時段SelectListItem();

                //設定風格 SelectListItem
                list.f風格 = (new CSearchResultFactory()).Add風格SelectListItem();

                //設定服務種類 SelectListItem
                list.f服務種類 = (new CSearchResultFactory()).Add服務種類SelectListItem();

                return View("SearchByKeyWord", list);
            }
            else
            {
                var list = new CSearchResult();

                //設定地區 SelectListItem
                list.f地區 = (new CSearchResultFactory()).Add地區SelectListItem();

                //設定時段 SelectListItem
                list.f時段 = (new CSearchResultFactory()).Add時段SelectListItem();

                //設定風格 SelectListItem
                list.f風格 = (new CSearchResultFactory()).Add風格SelectListItem();

                //設定服務種類 SelectListItem
                list.f服務種類 = (new CSearchResultFactory()).Add服務種類SelectListItem();

                // 沒有關鍵字 form 物件回傳
                return View(list);
            }
        }
    }
}
