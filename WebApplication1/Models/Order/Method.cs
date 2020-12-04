using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models.Common;

namespace WebApplication1.Models.Order
{
    public class Method
    {

        public class 訂單私廚評價
        {
            public int 平均評級 { get; set; }
            public int 總筆數 { get; set; }

        }

        public 訂單私廚評價 從訂單平均私廚評價(int fCID)
        {
            int 已完成 = e訂單狀態.客戶確認_完成評價.GetHashCode();

            string sSQL = @"
                SELECT	
	                CAST(ROUND(AVG(CAST(o.f評級 AS decimal)),0) AS int) [平均評級], 
	                COUNT(*) [總筆數]  
                FROM [t私廚] c 
                JOIN [t販售項目] p ON c.fCID = p.fCID 
                JOIN [t訂單] o ON o.fPID = p.fPID 
                WHERE c.fCID = @fCID and o.f狀態 = @f狀態 
                GROUP BY c.fCID
                ";

            var value = new List<SqlParameter>()
            {
                new SqlParameter("@fCID",fCID),
                new SqlParameter("@f狀態",已完成)
            };

            var res = new 訂單私廚評價();

            DBClass.SQLReader(sSQL, value.ToArray(), (reader) => {
                while (reader.Read())
                {
                    res.平均評級 = (int)reader["平均評級"];
                    res.總筆數 = (int)reader["總筆數"];
                }
            });

            return res;
        }

        public List<SelectListItem> get私廚可預訂時間 (int fCID)
        {
            int 可預訂 = e私廚可預訂_時段_狀態.可預定.GetHashCode();

            string sSQL = @"
                SELECT t.f日期 [日期], t.fTID [fTID], 
                CASE t.f時段 
                    WHEN 1 THEN N'午餐' 
                    WHEN 2 THEN N'晚餐' 
                END [時段] 
                FROM [t私廚可預訂時間] t 
                WHERE t.fCID = @fCID AND t.f日期 > @今天 AND t.f狀態 = @f狀態 
                ORDER BY [日期] 
                ";

            var value = new List<SqlParameter>()
            {
                new SqlParameter("@fCID",fCID),
                new SqlParameter("@f狀態",可預訂),
                new SqlParameter("@今天",DateTime.Now),
            };

            var res = new List<SelectListItem>();

            DBClass.SQLReader(sSQL, value.ToArray(), (reader) => {
                while (reader.Read())
                {
                    res.Add(new SelectListItem
                    {
                        Text = ((DateTime)reader["日期"]).ToString("d") + "-" + reader["時段"].ToString(),
                        Value = reader["fTID"].ToString()
                    });
                }
            });
            return res;

        } 



    }
}