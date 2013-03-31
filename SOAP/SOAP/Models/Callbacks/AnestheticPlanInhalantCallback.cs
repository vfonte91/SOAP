using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace SOAP.Models.Callbacks
{
    public class AnestheticPlanInhalantCallback
    {
        public AnestheticPlanInhalant ProcessRow(SqlDataReader read, params AnestheticPlanInhalant.LazyComponents[] lazyComponents)
        {
            AnestheticPlanInhalant anesPlanInhalant = new AnestheticPlanInhalant();
            anesPlanInhalant.Id = Convert.ToInt32(read["a.Id"]);
            anesPlanInhalant.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.DrugId"].ToString() != "")
                anesPlanInhalant.Drug.Id = Convert.ToInt32(read["a.DrugId"].ToString());
            if (read["a.FlowRate"].ToString() != "")
                anesPlanInhalant.FlowRate = Convert.ToDecimal(read["a.FlowRate"].ToString());
            if (read["a.Percentage"].ToString() != "")
                anesPlanInhalant.Percentage = Convert.ToDecimal(read["a.Percentage"].ToString());

            foreach (AnestheticPlanInhalant.LazyComponents a in lazyComponents)
            {
                if (a == AnestheticPlanInhalant.LazyComponents.LOAD_DRUG_WITH_DETAILS && anesPlanInhalant.Drug.Id != -1)
                {
                    if (read["b.CategoryId"].ToString() != "")
                        anesPlanInhalant.Drug.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    anesPlanInhalant.Drug.Label = read["b.Label"].ToString();
                    anesPlanInhalant.Drug.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    anesPlanInhalant.Drug.Description = read["b.Description"].ToString();
                    if (read["b.Concentration"].ToString() != "")
                        anesPlanInhalant.Drug.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                    if (read["b.MaxDosage"].ToString() != "")
                        anesPlanInhalant.Drug.MaxDosage = Convert.ToDecimal(read["b.MaxDosage"].ToString());
                }
            }

            return anesPlanInhalant;
        }
    }
}