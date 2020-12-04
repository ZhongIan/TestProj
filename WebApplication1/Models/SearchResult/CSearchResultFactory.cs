using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models.Common;
using WebApplication1.ViewModels.Home;

namespace WebApplication1.Models.SearchResult
{

    public class CSearchResultFactory
    {
        private Database1Entities db = new Database1Entities();

        public List<SearchProduct> GetCSearchResultsByCondition(string 風格, string 地區, string 服務種類, string 日期, string 時段, int? fUID)
        {
            if (fUID != null)
            {
                string sSQL = "select p.fCID, p.fPID, p.f項目照片, p.f項目名稱, c.f私廚評級, p.f價格,u.f姓名,u.f會員照片,x.fFID ,case when x.fPID is null then 0 else 1 end is我的最愛 " +
                    "from (((((t販售項目 p inner join t私廚 c on p.fCID=c.fCID) " +
                    "inner join t會員 u on u.fUID = c.fUID) " +
                    "inner join t私廚可預訂時間 as t on t.fCID = p.fCID) " +
                    "inner join t風格 as s on s.fSID = p.fSID)" +
                    "inner join t服務種類 as k on k.fKID = p.fKID)" +
                    "left join (select f.fPID,f.fFID from t我的最愛 as f where f.fUID = @登入會員) as x on x.fPID = p.fPID " +
                    "where s.fSID = @風格 and k.fKID = @服務種類 and c.f服務地區 = @地區 and p.f上架 = 'true' " +
                    "and t.f日期 = @日期 and t.f時段 = @時段 ; ";
                DateTime D日期 = Convert.ToDateTime(日期);
                List<SqlParameter> paraList = new List<SqlParameter>();
                if (風格 != null && 地區 != null && 服務種類 != null && 日期 != null && 時段 != null)
                {
                    paraList.Add(new SqlParameter("@風格", 風格));
                    paraList.Add(new SqlParameter("@地區", 地區));
                    paraList.Add(new SqlParameter("@服務種類", 服務種類));
                    paraList.Add(new SqlParameter("@日期", D日期));
                    paraList.Add(new SqlParameter("@時段", 時段));
                    paraList.Add(new SqlParameter("@登入會員", fUID));

                }

                List<SearchProduct> list = new List<SearchProduct>();
                Common.DBClass.SQLReader(sSQL, paraList.ToArray(), (reader) =>
                {
                    while (reader.Read())
                    {
                        SearchProduct R = new SearchProduct();

                        R.fPID = (int)reader["fPID"];
                        R.fCID = (int)reader["fCID"];
                        R.f私廚姓名 = (string)reader["f姓名"];
                        R.f項目名稱 = (string)reader["f項目名稱"];
                        R.f項目照片 = (string)reader["f項目照片"];
                        R.f私廚評級 = (int)reader["f私廚評級"];
                        R.f價格 = (int)reader["f價格"];
                        R.f會員照片 = (string)reader["f會員照片"];
                        R.fis我的最愛 = (int)reader["is我的最愛"];       
                        list.Add(R);
                    }
                });

                return list;
            }
            else
            {
                string sSQL = "select p.fCID, p.fPID, p.f項目照片, p.f項目名稱, c.f私廚評級, p.f價格,u.f姓名,u.f會員照片 " +
                    "from (((((t販售項目 p inner join t私廚 c on p.fCID=c.fCID) " +
                    "inner join t會員 u on u.fUID = c.fUID) " +
                    "inner join t私廚可預訂時間 as t on t.fCID = p.fCID) " +
                    "inner join t風格 as s on s.fSID = p.fSID)" +
                    "inner join t服務種類 as k on k.fKID = p.fKID)" +
                    "where s.fSID = @風格 and k.fKID = @服務種類 and c.f服務地區 = @地區 and p.f上架 = 'true' " +
                    "and t.f日期 = @日期 and t.f時段 = @時段; ";
                DateTime D日期 = Convert.ToDateTime(日期);
                List<SqlParameter> paraList = new List<SqlParameter>();
                if (風格 != null && 地區 != null && 服務種類 != null && 日期 != null && 時段 != null)
                {
                    paraList.Add(new SqlParameter("@風格", 風格));
                    paraList.Add(new SqlParameter("@地區", 地區));
                    paraList.Add(new SqlParameter("@服務種類", 服務種類));
                    paraList.Add(new SqlParameter("@日期", D日期));
                    paraList.Add(new SqlParameter("@時段", 時段));


                }

                List<SearchProduct> list = new List<SearchProduct>();
                Common.DBClass.SQLReader(sSQL, paraList.ToArray(), (reader) =>
                {
                    while (reader.Read())
                    {
                        SearchProduct R = new SearchProduct();

                        R.fPID = (int)reader["fPID"];
                        R.fCID = (int)reader["fCID"];
                        R.f私廚姓名 = (string)reader["f姓名"];
                        R.f項目名稱 = (string)reader["f項目名稱"];
                        R.f項目照片 = (string)reader["f項目照片"];
                        R.f私廚評級 = (int)reader["f私廚評級"];
                        R.f價格 = (int)reader["f價格"];
                        R.f會員照片 = (string)reader["f會員照片"];
                        list.Add(R);
                    }
                });
                return list;
            }
        }
        public List<SearchProduct> GetCSearchResultsByKeyWord(string keyWord, int? fUID)
        {
            if (fUID != null)
            {
                string sSQL = "select p.fCID, p.fPID, p.f項目照片,p.f項目名稱, c.f私廚評級, p.f價格,u.f姓名, u.f會員照片, x.fFID ,case when x.fPID is null then 0 else 1 end is我的最愛 " +
                    "from t販售項目 p inner join t私廚 c on p.fCID=c.fCID " +
                    "inner join t會員 as u on u.fUID = c.fUID " +
                    "inner join t風格 as s on s.fSID = p.fSID " +
                    "inner join t服務種類 as k on k.fKID = p.fKID " +
                    "left join (select f.fPID, fFID from t我的最愛 as f where f.fUID = @登入會員) as x on x.fPID = p.fPID " +
                    "where (u.f姓名 like @姓名 or p.f項目名稱 like @項目名稱) and p.f上架 = 'true' ";
                List<SqlParameter> paraList = new List<SqlParameter>();
                if (keyWord != null)
                {
                    paraList.Add(new SqlParameter("@姓名", "%" + keyWord + "%"));
                    paraList.Add(new SqlParameter("@項目名稱", "%" + keyWord + "%"));
                    paraList.Add(new SqlParameter("@登入會員", fUID));
                }

                List<SearchProduct> list = new List<SearchProduct>();
                Common.DBClass.SQLReader(sSQL, paraList.ToArray(), (reader) =>
                {
                    while (reader.Read())
                    {
                        SearchProduct R = new SearchProduct();
                        R.fPID = (int)reader["fPID"];
                        R.fCID = (int)reader["fCID"];
                        R.f私廚姓名 = (string)reader["f姓名"];
                        R.f項目名稱 = (string)reader["f項目名稱"];
                        R.f項目照片 = (string)reader["f項目照片"];
                        R.f私廚評級 = (int)reader["f私廚評級"];
                        R.f價格 = (int)reader["f價格"];
                        R.f會員照片 = (string)reader["f會員照片"];
                        R.fis我的最愛 = (int)reader["is我的最愛"];
                        list.Add(R);
                    }
                });
                return list;
            }
            else
            {
                string sSQL = "select p.fCID, p.fPID, p.f項目照片,p.f項目名稱, c.f私廚評級, p.f價格,u.f姓名,u.f會員照片 " +
                    "from t販售項目 p inner join t私廚 c on p.fCID=c.fCID " +
                    "inner join t會員 as u on u.fUID = c.fUID " +
                    "inner join t風格 as s on s.fSID = p.fSID " +
                    "inner join t服務種類 as k on k.fKID = p.fKID " +
                    "where (u.f姓名 like @姓名 or p.f項目名稱 like @項目名稱) and p.f上架 = 'true' ";
                List<SqlParameter> paraList = new List<SqlParameter>();
                if (keyWord != null)
                {
                    paraList.Add(new SqlParameter("@姓名", "%" + keyWord + "%"));
                    paraList.Add(new SqlParameter("@項目名稱", "%" + keyWord + "%"));

                }

                List<SearchProduct> list = new List<SearchProduct>();
                Common.DBClass.SQLReader(sSQL, paraList.ToArray(), (reader) =>
                {
                    while (reader.Read())
                    {
                        SearchProduct R = new SearchProduct();
                        R.fPID = (int)reader["fPID"];
                        R.fCID = (int)reader["fCID"];
                        R.f私廚姓名 = (string)reader["f姓名"];
                        R.f項目名稱 = (string)reader["f項目名稱"];
                        R.f項目照片 = (string)reader["f項目照片"];
                        R.f私廚評級 = (int)reader["f私廚評級"];
                        R.f價格 = (int)reader["f價格"];
                        R.f會員照片 = (string)reader["f會員照片"];

                        list.Add(R);
                    }
                });
                return list;
            }
        }

        public List<SelectListItem> Add地區SelectListItem()
        {
            var 地區預設 = new List<SelectListItem>();
            地區預設.Add(new SelectListItem { Text = "請選擇地區", Disabled = true, Selected = true });
            地區預設.AddRange(
                CDictionary.地區.Select(x => new SelectListItem
                {
                    Value = x,
                    Text = x
                }).ToArray()
            );
            return 地區預設;
        }

        public List<SelectListItem> Add時段SelectListItem()
        {
            var 時段 = new List<SelectListItem>();
            時段.Add(new SelectListItem { Text = "請選擇時段", Disabled = true, Selected = true });
            時段.AddRange(
                Enum.GetValues(typeof(e私廚可預訂_時段)).Cast<e私廚可預訂_時段>().Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = x.GetHashCode().ToString()
                }));
            return 時段;
        }

        public List<SelectListItem> Add風格SelectListItem()
        {
            var 風格 = new List<SelectListItem>();
            風格.Add(new SelectListItem { Text = "請選擇風格", Disabled = true, Selected = true });
            風格.AddRange(db.t風格.Select(x =>
            new SelectListItem
            {
                Text = x.f風格,
                Value = x.fSID.ToString()
            }).ToList());

            return 風格;
        }

        public List<SelectListItem> Add服務種類SelectListItem()
        {
            var 服務種類 = new List<SelectListItem>();
            服務種類.Add(new SelectListItem { Text = "請選擇服務種類", Disabled = true, Selected = true });
            服務種類.AddRange(db.t服務種類.Select(x =>
            new SelectListItem
            {
                Text = x.f服務種類,
                Value = x.fKID.ToString()
            }).ToList());
            return 服務種類;
        }
    }
}