using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class OtherAnestheticDrugCallback
    {
        public OtherAnestheticDrug ProcessRow(SqlDataReader read, params OtherAnestheticDrug.LazyComponents[] lazyComponents)
        {
            OtherAnestheticDrug otherAnesDrug = new OtherAnestheticDrug();
            otherAnesDrug.Id = Convert.ToInt32(read["a.Id"]);
            otherAnesDrug.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.EquipmentId"].ToString() != "")
                otherAnesDrug.Drug.Id = Convert.ToInt32(read["a.EquipmentId"].ToString());

            foreach (OtherAnestheticDrug.LazyComponents a in lazyComponents)
            {
                if (a == OtherAnestheticDrug.LazyComponents.LOAD_DRUG_WITH_DETAIL && otherAnesDrug.Drug.Id != -1)
                {
                    otherAnesDrug.Drug.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    otherAnesDrug.Drug.Label = read["b.Label"].ToString();
                    otherAnesDrug.Drug.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    otherAnesDrug.Drug.Description = read["b.Description"].ToString();
                }
            }

            return otherAnesDrug;
        }
    }
}