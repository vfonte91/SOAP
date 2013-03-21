using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class BloodworkCallback
    {
        public Bloodwork ProcessRow(SqlDataReader read)
        {
            Bloodwork bloodwork = new Bloodwork();
            bloodwork.Id = Convert.ToInt32(read["a.Id"]);
            bloodwork.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            bloodwork.BloodworkName = read["a.BloodworkName"].ToString();
            if (read["a.Value"].ToString() != "")
                bloodwork.Value = Convert.ToDecimal(read["a.Value"].ToString());

            return bloodwork;
        }
    }
}