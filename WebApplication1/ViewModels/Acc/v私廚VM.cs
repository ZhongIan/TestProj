using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.ViewModels.Acc
{
    public class v私廚VM
    {
        public int fUID { get; set; }

        [Required(ErrorMessage = "您必須輸入服務地區！")]
        public string f服務地區 { get; set; }
        public string f私廚簡介 { get; set; }

        public List<SelectListItem> Select地區 { get; set; }
    }
}