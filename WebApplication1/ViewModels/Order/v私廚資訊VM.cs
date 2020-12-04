using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Order
{

    public class ChefProduct
    {
        public int fPID { get; set; }
        public string 項目名稱 { get; set; }
        public int 價格 { get; set; }
        public string 風格 { get; set; }
        public string 服務種類 { get; set; }
        public string 項目照片 { get; set; }
    }



    public class v私廚資訊VM
    {
        public string 私廚姓名 { get; set; }
        public string 私廚照片 { get; set; }
        public int 私廚評級 { get; set; }
        public string 電子郵件 { get; set; }

        public string 服務地區 { get; set; }
        public string 餐飲風格 { get; set; }
        public string 服務種類 { get; set; }
        public string 私廚簡介 { get; set; }

        public List<ChefProduct> 項目清單 { get; set; }

    }
}