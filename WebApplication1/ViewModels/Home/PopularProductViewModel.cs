using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels.Home
{
   
    public class Product
    {
        public int fPID { get; set; }
        public int fCID { get; set; }
        public string f項目名稱 { get; set; }
        public string f私廚姓名 { get; set; }
        public string f項目照片 { get; set; }
        public int f價格 { get; set; }
        public int f私廚評級 { get; set; }
        public string f風格 { get; set; }
        public string f服務種類 { get; set; }
        public string f會員照片 { get; set; }
    }
    public class Lesson
    {
        public int fPID { get; set; }
        public int fCID { get; set; }
        public string f項目名稱 { get; set; }
        public string f私廚姓名 { get; set; }
        public string f項目照片 { get; set; }
        public int f價格 { get; set; }
        public int f私廚評級 { get; set; }
        public string f風格 { get; set; }
        public string f服務種類 { get; set; }
        public string f會員照片 { get; set; }
    }
    public class PopularProductViewModel
    {
        public List<Product> 熱門品項 { get; set; }
        public List<Lesson> 熱門課程 { get; set; }

        
        public string 地區 { get; set; }
        public List<SelectListItem> f地區 { get; set; }
        
      
        public string f日期 { get; set; }

       
        public string 時段 { get; set; }
        public List<SelectListItem> f時段 { get; set; }

       
        public string 服務種類 { get; set; }
        public List<SelectListItem> f服務種類 { get; set; }

       
        public string 風格 { get; set; }
        public List<SelectListItem> f風格 { get; set; }
             

        public string txtkeyword { get; set; }
    }
}