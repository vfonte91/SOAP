using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace SOAP.Models.Callbacks
{
    public class AnestheticPlanInjectionCallback
    {
        public AnestheticPlanInjection ProcessRow(SqlDataReader read, params AnestheticPlanInjection.LazyComponents[] lazyComponents)
        {
            AnestheticPlanInjection anesPlanInject = new AnestheticPlanInjection();
            anesPlanInject.Id = Convert.ToInt32(read["a.Id"]);
            anesPlanInject.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.DrugId"].ToString() != "")
                anesPlanInject.Drug.Drug.Id = Convert.ToInt32(read["a.DrugId"].ToString());
            if (read["a.Route_Id"].ToString() != "")
                anesPlanInject.Route.Id = Convert.ToInt32(read["a.Route_Id"].ToString());
            if (read["a.Dosage"].ToString() != "")
                anesPlanInject.Dosage = Convert.ToDecimal(read["a.Dosage"].ToString());

            foreach (AnestheticPlanInjection.LazyComponents a in lazyComponents)
            {
                if (a == AnestheticPlanInjection.LazyComponents.LOAD_ROUTE_WITH_DETAILS && anesPlanInject.Route.Id != -1)
                {
                    anesPlanInject.Route.Category.Id = Convert.ToInt32(read["c.CategoryId"].ToString());
                    anesPlanInject.Route.Label = read["c.Label"].ToString();
                    anesPlanInject.Route.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    anesPlanInject.Route.Label = read["c.Description"].ToString();
                }
                else if (a == AnestheticPlanInjection.LazyComponents.LOAD_DRUG_INFORMATION && anesPlanInject.Drug.Drug.Id != -1)
                {
                    anesPlanInject.Drug.Drug.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    anesPlanInject.Drug.Drug.Label = read["d.Label"].ToString();
                    anesPlanInject.Drug.Drug.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    anesPlanInject.Drug.Drug.Description = read["b.Description"].ToString();
                    anesPlanInject.Drug.Id = Convert.ToInt32(read["b.Id"].ToString());
                    anesPlanInject.Drug.DoseMinRange = Convert.ToDecimal(read["b.DoseMinRange"].ToString());
                    anesPlanInject.Drug.DoseMaxRange = Convert.ToDecimal(read["b.DoseMaxRange"].ToString());
                    anesPlanInject.Drug.DoseMax = Convert.ToDecimal(read["b.DoseMax"].ToString());
                    anesPlanInject.Drug.DoseUnits = read["b.DoseUnits"].ToString();
                    anesPlanInject.Drug.Route = read["b.Route"].ToString();
                    anesPlanInject.Drug.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                    anesPlanInject.Drug.ConcentrationUnits = read["b.ConcentrationUnits"].ToString();
                }
            }

            return anesPlanInject;
        }
    }
}