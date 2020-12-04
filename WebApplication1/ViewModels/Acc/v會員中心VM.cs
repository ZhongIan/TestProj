using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Acc
{
    public class v會員中心VM
    {
        public string 姓名 { get; set; }
        public string 暱稱 { get; set; }
        public string 會員照片 { get; set; }
        public string 電話 { get; set; }
        public string 生日 { get; set; }
        public string 地址 { get; set; }
        public string 電子郵件 { get; set; }
        public int 點數 { get; set; }

        // 私廚
        public int? fCID { get; set; }
        public string 私廚簡介 { get; set; }
        public string 服務地區 { get; set; }
        public string 餐飲風格 { get; set; }
        public string 服務項目 { get; set; }

    }
}