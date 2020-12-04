using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Common
{

    public static class DBClass
    {

        public static string myDBConnectionString = connectString();

        public static string connectString()
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
            scsb.AttachDBFilename = @"|DataDirectory|Database1.mdf"; // |DataDirectory| 預設-> \bin\Debug\
                                                                     // scsb.InitialCatalog = "mydb"; // 資料庫名稱
            scsb.IntegratedSecurity = true; // 整合驗證

            return scsb.ToString();
        }

        public static int SQLExecute(string sSQL, SqlParameter[] values)
        {

            SqlConnection con = new SqlConnection(myDBConnectionString);
            con.Open();

            SqlCommand cmd = new SqlCommand(sSQL, con);
            cmd.Parameters.AddRange(values);

            int row = cmd.ExecuteNonQuery();
            con.Close();

            return row;
        }

        public static void SQLReader(string sSQL, SqlParameter[] sqlArgs, Action<SqlDataReader> action)
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