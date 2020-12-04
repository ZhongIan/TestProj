using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Acc
{
    public class v登入VM
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "您必須輸入帳號！")]
        public string 帳號 {get; set;}

        [DataType(DataType.Password)]   //表示此欄位為密碼欄位，所以輸入時會產生隱碼
        [Required(ErrorMessage = "您必須輸入密碼！")]
        public string 密碼 { get; set; }
    }
}