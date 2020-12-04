using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Order
{
    public class v儲值點數VM
    {
        
        public int fUID { get; set; }

        [Display(Name = "點數")]
        [Required]
        [Range(1,100000000)]
        public int f點數 { get; set; }

    }


}