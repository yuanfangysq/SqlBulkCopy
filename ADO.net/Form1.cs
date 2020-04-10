using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADO.net
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string GetTempName(string str)
        {
           return "##" + (str + Guid.NewGuid().ToString().Replace("-", "")).Trim();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            DataTable da = new DataTable();
            da.Columns.Add("ID", typeof(int));
            da.Columns.Add("NAME", typeof(string));
            da.TableName = "CASE";

            DataRow dr= da.NewRow();
            dr["ID"] = 1;
            dr["NAME"] = "YSQ";
            da.Rows.Add(dr);

            string cstring = "Data Source=.;Initial Catalog='test';Persist Security Info=True;User ID=sa;Password=123;";
           
            using (SqlConnection con = new SqlConnection(cstring))
            {
                con.Open();
                string strName = GetTempName("");
                using (SqlCommand cm = new SqlCommand())
                {
                    cm.Connection= con;
                    cm.CommandText = "SELECT  TOP 0 CAST(0 AS INT) AS ID,CAST('' AS VARCHAR(50)) NAME  INTO  " + strName;
                    cm.ExecuteNonQuery();
                }

                WriteDataBySqlBulkCopy(da, strName, con);


                DataTable dt = new DataTable();
                using (SqlDataAdapter SR = new SqlDataAdapter("SELECT  * FROM  " + strName, con))
                {
                    SR.Fill(dt);
                }

                //using (SqlCommand cm2 = new SqlCommand())
                //{
                //    cm2.Connection = con;
                //    cm2.CommandText = "SELECT  * FROM  " + strName;
                //    cm2.ExecuteNonQuery();


                //}



            }
         



        }

        /// <summary>
        /// 按照SqlBulkCopy写入数据库
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="tableName">数据库表名</param>
        public void WriteDataBySqlBulkCopy(DataTable dt, string tableName, DbConnection dbCon = null)
        {
            //判空
            if (dt == null || dt.Rows.Count == 0 || string.IsNullOrEmpty(tableName))
            {
                return;
            }
            SqlConnection con = dbCon as SqlConnection;
            if (con == null)
            {
                MessageBox.Show("传入的dbCon不是SqlConnection对象", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            //此部分是重要的东西
            using (System.Data.SqlClient.SqlBulkCopy sbc = new System.Data.SqlClient.SqlBulkCopy(con))
            {
                try
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {

                        sbc.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName); //设置目标表和源数据的列映射
                    }
                    sbc.DestinationTableName = tableName;  //取得目标表名
                    sbc.WriteToServer(dt);
                   

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("ex", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    throw ex;
                }
                finally
                {
                    //if (con.State == ConnectionState.Open)
                    //{
                    //    con.Close();
                    //}
                    //else
                    //{
                    //    //MessageBox.Show("数据库已经关闭", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                }
            }
        }

    }
}
