using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication1.Models.Common;


namespace WebApplication1.Models
{
    // https://ithelp.ithome.com.tw/questions/10195967

    public class MyAuthorize : AuthorizeAttribute
    {
        public e會員_權限 權限 { get; set; }

        // 驗證邏輯，成功回傳True，失敗回傳False
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            // 透過httpContext取得Session
            HttpSessionStateBase session = httpContext.Session;

            if (權限 == e會員_權限.一般)
            {
                if (session[CDictionary.SK_LOGINED_USER_ID] != null)
                    return true;
            }
            else if (權限 == e會員_權限.私廚)
            {
                if (session[CDictionary.SK_LOGINED_CHEF_ID] != null)
                    return true;
            }

            return false;
        }

        // 驗證失敗要做什麼
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // 設定要執行的Result
            var session = filterContext.HttpContext.Session;

            if (session[CDictionary.SK_LOGINED_USER_ID] == null) // 沒登入 回登入頁
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "Acc", action = "login" }
                    ));
            }
            else if (session[CDictionary.SK_LOGINED_CHEF_ID] == null) // 不是私廚 回會員中心[成為私廚]
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "Acc", action = "center" }
                    ));
            }
        }
    }
}