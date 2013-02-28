﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class AnestheticPlanPremedicationCallback
    {
        public AnestheticPlanPremedication ProcessRow(SqlDataReader read, params AnestheticPlanPremedication.LazyComponents[] lazyComponents)
        {
            AnestheticPlanPremedication anesPlanPremed = new AnestheticPlanPremedication();
            anesPlanPremed.Id = Convert.ToInt32(read["a.Id"]);
            anesPlanPremed.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            anesPlanPremed.Drug.Id = Convert.ToInt16(read["a.DrugId"].ToString());
            anesPlanPremed.Route.Id = Convert.ToInt16(read["a.RouteId"].ToString());
            anesPlanPremed.Dosage = Convert.ToDecimal(read["a.Dosage"].ToString());

            foreach (AnestheticPlanPremedication.LazyComponents a in lazyComponents)
            {
                if (a == AnestheticPlanPremedication.LazyComponents.LOAD_DRUG_WITH_DETAILS)
                {
                    anesPlanPremed.Drug.Category.Id = Convert.ToInt16(read["b.CategoryId"].ToString());
                    anesPlanPremed.Drug.Label = read["b.Label"].ToString();
                    anesPlanPremed.Drug.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    anesPlanPremed.Drug.Description = read["b.Description"].ToString();
                }
                else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                {
                    anesPlanPremed.Route.Category.Id = Convert.ToInt16(read["c.CategoryId"].ToString());
                    anesPlanPremed.Route.Label = read["c.Label"].ToString();
                    anesPlanPremed.Route.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    anesPlanPremed.Route.Description = read["c.Description"].ToString();
                }
            }

            return anesPlanPremed;
        }
    }
}