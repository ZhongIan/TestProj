using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Acc
{
    public class v註冊簡易VM
    {

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "您必須輸入帳號！")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "會員帳號的長度需再5~15個字元內！")]
        [RegularExpression(@"[a-zA-Z_0-9]*$", ErrorMessage = "帳號僅能有英文或數字！")]
        public string 帳號 { get; set; }


        [DataType(DataType.Password)]   //表示此欄位為密碼欄位，所以輸入時會產生隱碼
        [Required(ErrorMessage = "您必須輸入密碼！")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "會員密碼的長度需再6~20個字元內！")]
        [RegularExpression(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z]).{3,15}$", ErrorMessage = "密碼僅能有英文或數字，至少一個大小寫英+數！")]
        public string 密碼 { get; set; }

        [DataType(DataType.Password)]   //表示此欄位為密碼欄位，所以輸入時會產生隱碼
        [Required(ErrorMessage = "請再次輸入密碼！")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "確認密碼的長度需再6~20個字元內！")]
        [System.ComponentModel.DataAnnotations.Compare("密碼", ErrorMessage = "兩次輸入的密碼必須相符！")]//跟System.Web.Mvc產生模稜兩可
        public string 確認密碼 { get; set; }

        [Required(ErrorMessage = "您必須輸入Email")]
        [StringLength(300, MinimumLength = 6, ErrorMessage = "此欄位僅接受300個字")]
        [DataType(DataType.EmailAddress, ErrorMessage = "請輸入正確的電子信箱")]
        public string 電子信箱 { get; set; }
    }
}