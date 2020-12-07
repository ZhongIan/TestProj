using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.ViewModels.Acc
{
    public class v基本資料VM
    {
        public v基本資料VM()
        {
            this.f會員照片 = WebApplication1.Models.Common.CDictionary.預設NOIMAGE;
            this.f暱稱 = "";
        }


        public int fUID { get; set; }

        [Display(Name = "姓名")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "您必須輸入姓名！")]
        public string f姓名 { get; set; }


        [Display(Name = "暱稱")]
        [DataType(DataType.Text)]
        public string f暱稱 { get; set; }

        [Display(Name = "電話")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10,MinimumLength = 10, ErrorMessage = "電話格式錯誤: 手機請用 0912345678 市話請用: 07-6661111")]
        [Required(ErrorMessage = "您必須輸入電話！")]
        public string f電話 { get; set; }

        /// <summary>
        /// 下拉式選單
        /// </summary>
        public List<SelectListItem> Select地區 { get; set; }


        [Display(Name = "居住縣市")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "您必須居住縣市！")]
        public string f居住縣市 { get; set; }


        [Display(Name = "詳細地址")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "您必須輸入地址！")]
        public string f詳細地址 { get; set; }


        [Required(ErrorMessage = "您必須輸入生日")]
        [Display(Name = "生日")]
        [DataType(DataType.Date, ErrorMessage = "請輸入正確日期")]
        public string f生日 { get; set; }


        public string f會員照片 { get; set; }

        public HttpPostedFileBase image { get; set; }
    }
}