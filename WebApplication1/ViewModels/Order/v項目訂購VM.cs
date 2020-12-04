using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebApplication1.ViewModels.Order
{
    public class 顧客評價
    {
        public string 暱稱 { get; set; }
        public string 顧客照片 { get; set; }
        public int 顧客評級 { get; set; }
        public string 評價日期 { get; set; }

        public string 評價內容 { get; set; }
    }

    public class v項目訂購VM
    {
        #region 項目訂購

        public string 項目照片 { get; set; }
        public string 項目名稱 { get; set; }
        public string 項目內容 { get; set; }
        public string 服務種類 { get; set; }

        public string 風格 { get; set; }

        public int 價格 { get; set; }


        public string 私廚姓名 { get; set; }
        public string 私廚照片 { get; set; }

        public int 私廚評級 { get; set; }
        public string 服務地區 { get; set; }
        public string 私廚簡介 { get; set; }

        public int 私廚評級筆數 { get; set; }

        #endregion

        #region 菜品清單 顧客評價清單

        public List<t菜品> 菜品清單 { get; set; }

        public List<顧客評價> 顧客評價清單 { get; set; }

        public List<SelectListItem> Select日期 { get; set; }

        #endregion


        #region 表單資料

        public int fCID { get; set; }

        public int fPID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "沒有可預定日期")]
        public int fTID { get; set; }

        [Required(ErrorMessage = "數量為必填")]
        public int 數量 { get; set; }
       

        #endregion

    }

}