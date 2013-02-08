using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using SOAP.Models;

namespace SOAP.Controllers
{
    public class SampleService
    {
        private string connectionString;

        public SampleService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SOAP_DB_CONNECTION"].ConnectionString;
        }

        public List<DropdownCategory> SelectQuery()
        {
            List<DropdownCategory> dropdowns = new List<DropdownCategory>();
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                string sql = @"SELECT ID, ShortName, LongName FROM dbo.Dropdown_Categories WHERE ShortName LIKE @Param";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Param", SqlDbType.VarChar).Value = "%Drug%";
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        DropdownCategory dcat = new DropdownCategory();
                        dcat.Id = Convert.ToInt32(read["Id"]);
                        dcat.ShortName = read["ShortName"].ToString();
                        dcat.LongName = read["LongName"].ToString();
                        dropdowns.Add(dcat);
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
                return dropdowns;
            }
        }
    }
}