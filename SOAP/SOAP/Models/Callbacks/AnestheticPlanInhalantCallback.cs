﻿using System;
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
                anesPlanInhalant.Drug.Drug.Id = Convert.ToInt32(read["a.DrugId"].ToString());
            if (read["a.FlowRate"].ToString() != "")
                anesPlanInhalant.FlowRate = Convert.ToInt32(read["a.FlowRate"].ToString());
            if (read["a.Dose"].ToString() != "")
                anesPlanInhalant.Dose = Convert.ToDecimal(read["a.Dose"].ToString());

            foreach (AnestheticPlanInhalant.LazyComponents a in lazyComponents)
            {
                if (a == AnestheticPlanInhalant.LazyComponents.LOAD_DRUG_INFORMATION && anesPlanInhalant.Drug.Id != -1)
                {
                    anesPlanInhalant.Drug.Drug.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    anesPlanInhalant.Drug.Drug.Label = read["d.Label"].ToString();
                    anesPlanInhalant.Drug.Drug.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    anesPlanInhalant.Drug.Drug.Description = read["b.Description"].ToString();
                    anesPlanInhalant.Drug.Id = Convert.ToInt32(read["b.Id"].ToString());
                    anesPlanInhalant.Drug.DoseMinRange = Convert.ToDecimal(read["b.DoseMinRange"].ToString());
                    anesPlanInhalant.Drug.DoseMaxRange = Convert.ToDecimal(read["b.DoseMaxRange"].ToString());
                    anesPlanInhalant.Drug.DoseMax = Convert.ToDecimal(read["b.DoseMax"].ToString());
                    anesPlanInhalant.Drug.DoseUnits = read["b.DoseUnits"].ToString();
                    anesPlanInhalant.Drug.Route = read["b.Route"].ToString();
                    anesPlanInhalant.Drug.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                    anesPlanInhalant.Drug.ConcentrationUnits = read["b.ConcentrationUnits"].ToString();
                }
            }

            return anesPlanInhalant;
        }
    }
}