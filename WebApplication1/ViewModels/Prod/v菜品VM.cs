using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Prod
{
    public class v菜品VM
    {
        public v菜品VM()
        {
            this.f菜品照片 = WebApplication1.Models.Common.CDictionary.預設NOIMAGE;
        }

        [Key]
        public int fVID { get; set; }

        public string 項目名稱 { get; set; }

        [Display(Name = "產品編號")]
        public int fPID { get; set; }

        [Display(Name = "菜品簡介")]
        [Required(ErrorMessage = "您必須輸入菜品簡介！")]
        public string f菜品簡介 { get; set; }

        [Display(Name = "菜品照片")]
        public string f菜品照片 { get; set; }

        [Display(Name = "菜品名稱")]
        [Required(ErrorMessage = "您必須輸入菜品名稱！")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "請輸入1~20個字")]
        public string f菜品名稱 { get; set; }

        public HttpPostedFileBase image { get; set; }

    }
}