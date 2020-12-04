using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebApplication1.Models.Common
{

    /// <summary>
    /// 給 登入後跳轉回前頁用
    /// </summary>
    public class page
    {
        public string controller { get; set; }
        public string action { get; set; }

        public object urlArgs { get; set; }
    }

    /// <summary>
    /// SESSION KEY OR ... 
    /// </summary>
    public class CDictionary
    {
        // SESSION KEY 
        public static readonly string SK_LOGINED_USER_ID = "SK_LOGINED_USER_ID";
        public static readonly string SK_LOGINED_CHEF_ID = "SK_LOGINED_CHEF_ID";
        public static readonly string SK_LOGINED_USER_AUTH = "SK_LOGINED_USER_AUTH";
        public static readonly string SK_PAGE_BEFORE_LOGIN = "SK_PAGE_BEFORE_LOGIN";

        // TEMP_DATA MESSAGE KEY
        public static readonly string TK_Msg_ChefCreate = "TK_Msg_ChefCreate";
        public static readonly string TK_Msg_SalesItem = "TK_Msg_SalesItem";
        public static readonly string TK_Msg_SaveCalendar = "TK_Msg_SaveCalendar";

        public static readonly string 照片存檔位置_會員 = "Memberimage";
        public static readonly string 照片存檔位置_項目 = "Productimage";
        public static readonly string 照片存檔位置_菜品 = "Dishimage";

        // NOIMAGE 圖
        public static readonly string 預設NOIMAGE = "/Content/img/nopic.jpg";
        
        public static string[] 地區 =
        {
            "台北市",
            "新北市",
            "基隆市",
            "桃園市",
            "新竹市",
            "新竹縣",
            "苗栗縣",
            "台中市",
            "彰化縣",
            "南投縣",
            "雲林縣",
            "嘉義市",
            "嘉義縣",
            "台南市",
            "高雄市",
            "屏東縣",
            "宜蘭縣",
            "花蓮縣",
            "台東縣",
            "澎湖縣",
            "金門縣",
            "連江縣"
        };

    }

    /// <summary>
    /// 會員_權限
    /// </summary>
    public enum e會員_權限
    {
        一般 = 1,
        私廚 = 2
    }



    /// <summary>
    /// [t私廚可預訂時間]
    ///  的時段
    /// </summary>
    public enum e私廚可預訂_時段
    {
        午餐 = 1,
        晚餐 = 2
    }

    /// <summary>
    /// [t私廚可預訂時間]
    ///  的時段之狀態
    /// </summary>
    public enum e私廚可預訂_時段_狀態
    {
        可預定 = 1,
        被預訂_不可修改 = 2
    }

    /// <summary>
    /// [t訂單]
    /// [狀態]
    /// </summary>
    public enum e訂單狀態
    {
        客戶預訂 = 1,
        私廚確認_開放客戶評價 = 2,
        客戶確認_完成評價 = 3
    }



    public class 共用方法
    {
        public string 照片更新(HttpPostedFileBase httpPostedFile, string 根目錄, string 存檔位置, string 舊照片位置)
        {
            // 刪除舊照片
            if (System.IO.File.Exists($@"{根目錄 + 舊照片位置}"))
            {
                System.IO.File.Delete($@"{根目錄 + 舊照片位置}");
            }

            // 取得 . 的位置
            int point = httpPostedFile.FileName.IndexOf(".");
            // 取得 附檔名 *.jpg
            string estention = httpPostedFile.FileName
                .Substring(point, httpPostedFile.FileName.Length - point);
            // 不重複16位檔名
            string photoName = Guid.NewGuid().ToString() + estention;

            // 照片存檔
            string location = $@"/Content/{存檔位置}/" + photoName;
            httpPostedFile.SaveAs($@"{ 根目錄 + location }");

            // 回傳新路徑
            return location;
        }
    }


}