using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            1. 更變訂單狀態 私廚確認_開放客戶評價 -> 客戶確認_完成評價
            2. 變更 [私廚] 的評級 : 由 [訂單] 平均算出
            3. 依[訂單][數量] * [v評價VM][價格] 加入至私廚的 [會員][點數]
            */

            Dbclass db = new Dbclass(args[0]);

            var fOID_list = new List<int>();
            var fPID_list = new List<int>();
            var fCID_list = new List<int>();

            var sql_order = @"
               SELECT o.fOID [fOID], o.fPID [fPID], c.fCID [fCID]
                FROM
                    (SELECT fOID , fPID 
                    FROM [t訂單] o
                    WHERE o.f狀態=2 AND 
                        DATEADD(day,5,
	                        (SELECT TOP 1 value FROM STRING_SPLIT(o.f預定日期,' '))
                        ) < GETDATE()
                    ) AS o
                JOIN [t販售項目] p ON o.fPID=p.fPID
                JOIN [t私廚] c ON p.fCID=c.fCID
            ";

            db.SQLReader(sql_order, new SqlParameter[] { }, (reader) =>
            {
                while (reader.Read())
                {
                    fOID_list.Add((int)reader["fOID"]);
                    fPID_list.Add((int)reader["fPID"]);
                    fCID_list.Add((int)reader["fCID"]);
                }
            });

            for(int i =0; i< fOID_list.Count();i++)
            {
                // 1. 更變訂單狀態 私廚確認_開放客戶評價 -> 客戶確認_完成評價
                string sql_up_order = @"
                UPDATE o
                SET o.f評價日期 = @f評價日期,
	                o.f狀態=3,
	                o.f評級=5,
	                o.f評價內容=N'5天未評價，系統自動評價'
                FROM [t訂單] o
                WHERE o.fOID=@fOID
                ";

                db.SQLExecute(sql_up_order, new SqlParameter[] { 
                    new SqlParameter("@fOID", fOID_list[i]),
                    new SqlParameter("@f評價日期", DateTime.Now.ToString("g"))
                });

                // 2. 變更 [私廚] 的評級 : 由 [訂單] 平均算出
                string sql_up_avg = @"
                UPDATE c
                SET c.f私廚評級 = chef_Avg.平均評級,
	                c.f私廚評級筆數 = chef_Avg.總筆數
                FROM 
	                (SELECT	
		                CAST(ROUND(AVG(CAST(o.f評級 AS decimal)),0) AS int) [平均評級], 
		                COUNT(*) [總筆數], c.fCID [fCID] 
	                FROM [t私廚] c 
	                JOIN [t販售項目] p ON c.fCID = p.fCID 
	                JOIN [t訂單] o ON o.fPID = p.fPID 
	                WHERE c.fCID = @fCID  and o.f狀態 = 3 
	                GROUP BY c.fCID) AS chef_Avg
                JOIN [t私廚] c ON chef_Avg.fCID=c.fCID
                WHERE c.fCID = @fCID
                ";

                db.SQLExecute(sql_up_avg, new SqlParameter[] { new SqlParameter("@fCID", fCID_list[i]) });


                // 3. 依[訂單][數量] * [v評價VM][價格] 加入至私廚的 [會員][點數]
                string sql_up_point = @"
                UPDATE u
                SET u.f點數= u.f點數 - CAST(ROUND(0.9*o.f數量*p.f價格, 0) AS int)
                FROM
	                (SELECT *
	                FROM [t訂單] o
	                WHERE o.fPID=@fPID
		                ) AS o
                JOIN [t販售項目] p ON o.fPID=p.fPID
                JOIN [t私廚] c ON p.fCID=c.fCID
                JOIN [t會員] u ON c.fUID=u.fUID
                ";

                db.SQLExecute(sql_up_point, new SqlParameter[] {new SqlParameter("@fPID", fPID_list[i]) });

            }

        }

        public class Dbclass
        {
            public string myDBConnectionString;

            public Dbclass(string directory)
            {
                this.myDBConnectionString = connectString(directory);
            }

            public string connectString(string directory)
            {
                /*
                // 相對路徑
                // https://t.codebug.vip/questions-2087572.htm

                string Path = Environment.CurrentDirectory;
                string[] appPath = Path.Split(new string[] { "bin" }, StringSplitOptions.None);
                AppDomain.CurrentDomain.SetData("DataDirectory", appPath[0]);
                */

                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
                scsb.DataSource = @"(LocalDB)\MSSQLLocalDB";

                // 不可用 @"..//..//Database2.mdf";  ??
                scsb.AttachDBFilename = directory; //@"|DataDirectory|Database1.mdf"; // |DataDirectory| 預設-> \bin\Debug\
                                                   // scsb.InitialCatalog = "mydb"; // 資料庫名稱
                scsb.IntegratedSecurity = true; // 整合驗證

                return scsb.ToString();
            }

            public int SQLExecute(string sSQL, SqlParameter[] values)
            {

                SqlConnection con = new SqlConnection(myDBConnectionString);
                con.Open();

                SqlCommand cmd = new SqlCommand(sSQL, con);
                cmd.Parameters.AddRange(values);

                int row = cmd.ExecuteNonQuery();
                con.Close();

                return row;
            }

            public void SQLReader(string sSQL, SqlParameter[] sqlArgs, Action<SqlDataReader> action)
            {
                SqlConnection con = new SqlConnection(myDBConnectionString);
                con.Open();

                // 加入SQL 字串
                SqlCommand cmd = new SqlCommand(sSQL, con);
                // 加入SQL 參數
                cmd.Parameters.AddRange(sqlArgs);
                SqlDataReader reader = cmd.ExecuteReader();

                // callback reader
                action(reader);

                reader.Close();
                con.Close();
            }
        }

       
    }
}
