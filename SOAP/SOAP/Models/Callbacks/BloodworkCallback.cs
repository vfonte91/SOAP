using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class BloodworkCallback
    {
        public Bloodwork ProcessRow(SqlDataReader read, params Bloodwork.LazyComponents[] lazyComponents)
        {
            Bloodwork bloodwork = new Bloodwork();
            bloodwork.Id = Convert.ToInt32(read["a.Id"]);
            bloodwork.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            bloodwork.BloodworkInfo.Id = Convert.ToInt16(read["a.BloodworkId"].ToString());
            bloodwork.Value = Convert.ToDecimal(read["a.Value"].ToString());

            foreach (Bloodwork.LazyComponents a in lazyComponents)
            {
                if (a == Bloodwork.LazyComponents.LOAD_BLOODWORK_INFO)
                {
                    bloodwork.BloodworkInfo.Category.Id = Convert.ToInt16(read["b.CategoryId"].ToString());
                    bloodwork.BloodworkInfo.Label = read["b.Label"].ToString();
                    bloodwork.BloodworkInfo.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    bloodwork.BloodworkInfo.Description = read["b.Description"].ToString();
                }
            }

            return bloodwork;
        }
    }
}