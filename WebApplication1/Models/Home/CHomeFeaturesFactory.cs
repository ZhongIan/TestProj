using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models.SearchResult;
using WebApplication1.ViewModels.Home;

namespace WebApplication1.Models.Home
{
    public class CHomeFeaturesFactory
    {
        public List<Product> GetPopularProduct()
        {
            string sSQL = "select p.fPID,c.f私廚評級, p.f項目名稱,p.f項目內容,k.f服務種類,u.f姓名,p.f價格,p.f項目照片,s.f風格 ,q.數量, u.f會員照片 " +
                    "from(select top 9 o.fPID, count(*) 數量 from t訂單 as o " +
                    "inner join t販售項目 p on p.fPID = o.fPID " +
                    "inner join t服務種類 k on k.fKID = p.fKID " +
                    "where k.fKID = 3 " +
                    "group by o.fPID order by count(*) desc) q " +
                    "inner join t販售項目 p on p.fPID = q.fPID " +
                    "inner join t私廚 c on c.fCID = p.fCID " +
                    "inner join t會員 u on c.fUID = u.fUID " +
                    "inner join t風格 s on s.fSID = p.fSID " +
                    "inner join t服務種類 k on k.fKID = p.fKID";

            List<Product> list = new List<Product>();
            Common.DBClass.SQLReader(sSQL, new SqlParameter[] { }, (reader) =>
            {
                while (reader.Read())
                {
                    Product R = new Product();
                    R.fPID = (int)reader["fPID"];
                    R.f私廚姓名 = (string)reader["f姓名"];
                    R.f項目名稱 = (string)reader["f項目名稱"];
                    R.f項目照片 = (string)reader["f項目照片"];
                    R.f私廚評級 = (int)reader["f私廚評級"];
                    R.f價格 = (int)reader["f價格"];
                    R.f服務種類 = (string)reader["f服務種類"];
                    R.f風格 = (string)reader["f風格"];
                    R.f會員照片 = (string)reader["f會員照片"];
                    list.Add(R);
                }
            });

            return list;
        }

        public List<Lesson> GetPopularLesson()
        {
            string sSQL = "select p.fPID,c.f私廚評級,p.f項目名稱,p.f項目內容,k.f服務種類,u.f姓名,p.f價格,p.f項目照片,s.f風格 ,q.數量 " +
                "from(select top 6 o.fPID, count(*) 數量 from t訂單 as o " +
                "inner join t販售項目 p on p.fPID = o.fPID " +
                "inner join t服務種類 k on k.fKID = p.fKID " +
                "where k.fKID in (1,2) " +
                "group by o.fPID order by count(*) desc) q " +
                "inner join t販售項目 p on p.fPID = q.fPID " +
                "inner join t私廚 c on c.fCID = p.fCID " +
                "inner join t會員 u on c.fUID = u.fUID " +
                "inner join t風格 s on s.fSID = p.fSID " +
                "inner join t服務種類 k on k.fKID = p.fKID";

            List<Lesson> list = new List<Lesson>();
            Common.DBClass.SQLReader(sSQL, new SqlParameter[] { }, (reader) =>
            {
                while (reader.Read())
                {
                    Lesson R = new Lesson();
                    R.fPID = (int)reader["fPID"];
                    R.f私廚姓名 = (string)reader["f姓名"];
                    R.f項目名稱 = (string)reader["f項目名稱"];
                    R.f項目照片 = (string)reader["f項目照片"];
                    R.f私廚評級 = (int)reader["f私廚評級"];
                    R.f價格 = (int)reader["f價格"];
                    R.f服務種類 = (string)reader["f服務種類"];
                    R.f風格 = (string)reader["f風格"];
                    
                    list.Add(R);
                }
            });

            return list;
        }
    }
}