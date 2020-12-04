using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Common;
using WebApplication1.Models.Home;
using WebApplication1.Models.SearchResult;
using WebApplication1.ViewModels.Home;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        
        private Database1Entities db = new Database1Entities();

        public ActionResult Index()
        {
            //取得熱門品項及熱門課程       
            var list_p = (new CHomeFeaturesFactory()).GetPopularProduct();
            var list_l = (new CHomeFeaturesFactory()).GetPopularLesson();
            var list = new PopularProductViewModel
            {
                熱門品項 = list_p,
                熱門課程 = list_l
            };

            //設定地區 SelectListItem
            list.f地區 = (new CSearchResultFactory()).Add地區SelectListItem();

            //設定時段 SelectListItem
            list.f時段 = (new CSearchResultFactory()).Add時段SelectListItem();

            //設定風格 SelectListItem
            list.f風格 = (new CSearchResultFactory()).Add風格SelectListItem();

            //設定服務種類 SelectListItem
            list.f服務種類 = (new CSearchResultFactory()).Add服務種類SelectListItem();


            return View(list);
        }


        public ActionResult About()
        {
            return View();
        }

        
        public ActionResult Service()
        {
            return View();
        }
    }
}