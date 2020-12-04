using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Order
{
    public class v評價VM
    {
        public string 預定日期 { get; set; }

        public string 私廚名稱 { get; set; }
        public string 私廚照片 { get; set; }

        public int 私廚評級 { get; set; }

        public string 項目名稱 { get; set; }
        public string 項目照片 { get; set; }

        // 需傳至後端使用
        [Required]
        public int 價格 { get; set; } 
        public string 服務種類 { get; set; }

        // === 表單 ===

        [Required]
        public int fOID { get; set; }

        [Required]
        public int 評級 { get; set; }

        [Required(ErrorMessage = "您必須輸入評價內容！")]
        public string 評價內容 { get; set; }

    }
}