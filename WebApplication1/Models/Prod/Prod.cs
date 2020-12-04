using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models.Common;

namespace WebApplication1.Models.Prod
{
        public class Method
        {
            public string 從販售項目選出私廚風格(int fCID)
            {
                string sSQL = $"SELECT DISTINCT s.f風格 [風格] " +
                    $"FROM [t販售項目] p " +
                    $"JOIN [t風格] s ON p.fSID = s.fSID " +
                    $"WHERE p.fCID = @fCID ";
                   
                var value = new List<SqlParameter>()
                {
                    new SqlParameter("@fCID",fCID)
                };

                var res = new List<string>();

                DBClass.SQLReader(sSQL, value.ToArray(), (reader) =>
                {
                    while (reader.Read())
                    {
                        res.Add(
                            (string)reader["風格"]
                            );
                    }
                });

                return String.Join(",", res);
            }

            public string 從販售項目選出私廚服務種類(int fCID)
            {
                string sSQL = $"SELECT DISTINCT k.f服務種類 [服務種類] " +
                    $"FROM [t販售項目] p  " +
                    $"JOIN [t服務種類] k ON p.fKID = k.fKID " +
                    $"WHERE p.fCID = @fCID ";

                var value = new List<SqlParameter>()
                {
                    new SqlParameter("@fCID",fCID)
                };

                var res = new List<string>();

                DBClass.SQLReader(sSQL, value.ToArray(), (reader) =>
                {
                    while (reader.Read())
                    {
                        res.Add(
                            (string)reader["服務種類"]
                            );
                    }
                });

                return string.Join(",", res);
            }

    }
}