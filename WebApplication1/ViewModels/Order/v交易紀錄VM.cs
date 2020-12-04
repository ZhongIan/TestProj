using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels.Order
{
    public class v交易紀錄VM
    {
        public int 點數餘額 { get; set; }

        public List<sp_交易紀錄_Result> 交易紀錄 { get; set; }

        public List<sp_交易紀錄_Result> 交易紀錄_未完成 { get; set; }

        public List<sp_交易紀錄_Result> 交易紀錄_待評價 { get; set; }

        public List<sp_交易紀錄_Result> 交易紀錄_已完成 { get; set; }
    }
}