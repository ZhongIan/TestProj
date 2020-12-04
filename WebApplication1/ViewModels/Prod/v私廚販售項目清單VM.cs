using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Prod
{

    public class 項目
    {
        public t販售項目 product { get; set; }
        public List<t菜品> dish { get; set; }
    }

    public class v私廚販售項目清單VM
    {
        public t私廚 chef { get; set; }
        public List<項目> prods { get; set; }
    }
}