using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.ViewModels.Prod

{
    public class v販售項目VM
    {
        public v販售項目VM()
        {
            this.f項目照片 = WebApplication1.Models.Common.CDictionary.預設NOIMAGE;
        }

        public int fPID { get; set; }

        public int fCID { get; set; }

        [Display(Name = "風格")]
        public int fSID { get; set; }

        public List<SelectListItem> style { get; set; }

        [Display(Name = "服務種類")]
        public int fKID { get; set; }

        public List<SelectListItem> kind { get; set; }

        [Required(ErrorMessage = "您必須輸入項目名稱")]
        [Display(Name = "項目名稱")]
        public string f項目名稱 { get; set; }

        [Required(ErrorMessage = "您必須輸入項目內容！")]
        [Display(Name = "項目內容")]
        public string f項目內容 { get; set; }

        [Required(ErrorMessage = "您必須輸入價格")]
        [Display(Name = "價格(NT$)")]
        [DataType(DataType.Currency)]
        [Range(100, 10000, ErrorMessage = "價格介於 100 ~ 10000")]
        public int f價格 { get; set; }

        [Display(Name = "上架")]
        [Required]
        public bool f上架 { get; set; }

        public string f項目照片 { get; set; }

        public HttpPostedFileBase image { get; set; }
    }
}