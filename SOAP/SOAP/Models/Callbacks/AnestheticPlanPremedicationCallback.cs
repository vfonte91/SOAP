using System;
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
            if (read["a.RouteId"].ToString() != "")
                anesPlanPremed.Route.Id = Convert.ToInt32(read["a.RouteId"].ToString());
            if (read["a.SedativeDrugId"].ToString() != "")
                anesPlanPremed.SedativeDrug.Id = Convert.ToInt32(read["a.SedativeDrugId"].ToString());
            if (read["a.SedativeDosage"].ToString() != "")
                anesPlanPremed.SedativeDosage = Convert.ToDecimal(read["a.SedativeDosage"].ToString());
            if (read["a.OpioidDrugId"].ToString() != "")
                anesPlanPremed.OpioidDrug.Id = Convert.ToInt32(read["a.OpioidDrugId"].ToString());
            if (read["a.OpioidDosage"].ToString() != "")
                anesPlanPremed.OpioidDosage = Convert.ToDecimal(read["a.OpioidDosage"].ToString());
            if (read["a.AnticholinergicDrugId"].ToString() != "")
                anesPlanPremed.AnticholinergicDrug.Id = Convert.ToInt32(read["a.AnticholinergicDrugId"].ToString());
            if (read["a.AnticholinergicDosage"].ToString() != "")
                anesPlanPremed.AnticholinergicDosage = Convert.ToDecimal(read["a.AnticholinergicDosage"].ToString());
            if (read["a.KetamineDosage"].ToString() != "")
                anesPlanPremed.KetamineDosage = Convert.ToDecimal(read["a.KetamineDosage"].ToString());

            foreach (AnestheticPlanPremedication.LazyComponents a in lazyComponents)
            {
                if (a == AnestheticPlanPremedication.LazyComponents.LOAD_SEDATIVE_DRUG_WITH_DETAILS && anesPlanPremed.SedativeDrug.Id != -1)
                {
                    anesPlanPremed.SedativeDrug.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    anesPlanPremed.SedativeDrug.Label = read["b.Label"].ToString();
                    anesPlanPremed.SedativeDrug.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    anesPlanPremed.SedativeDrug.Description = read["b.Description"].ToString();
                    if (read["b.Concentration"].ToString() != "")
                        anesPlanPremed.SedativeDrug.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                    if (read["b.MaxDosage"].ToString() != "")
                        anesPlanPremed.SedativeDrug.MaxDosage = Convert.ToDecimal(read["b.MaxDosage"].ToString());
                }
                else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_OPIOID_DRUG_WITH_DETAILS && anesPlanPremed.OpioidDrug.Id != -1)
                {
                    anesPlanPremed.OpioidDrug.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    anesPlanPremed.OpioidDrug.Label = read["d.Label"].ToString();
                    anesPlanPremed.OpioidDrug.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    anesPlanPremed.OpioidDrug.Description = read["d.Description"].ToString();
                    if (read["d.Concentration"].ToString() != "")
                        anesPlanPremed.OpioidDrug.Concentration = Convert.ToDecimal(read["d.Concentration"].ToString());
                    if (read["d.MaxDosage"].ToString() != "")
                        anesPlanPremed.OpioidDrug.MaxDosage = Convert.ToDecimal(read["d.MaxDosage"].ToString());
                }
                else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_ANTICHOLINERGIC_DRUG_WITH_DETAILS && anesPlanPremed.AnticholinergicDrug.Id != -1)
                {
                    anesPlanPremed.AnticholinergicDrug.Category.Id = Convert.ToInt32(read["e.CategoryId"].ToString());
                    anesPlanPremed.AnticholinergicDrug.Label = read["e.Label"].ToString();
                    anesPlanPremed.AnticholinergicDrug.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                    anesPlanPremed.AnticholinergicDrug.Description = read["e.Description"].ToString();
                    if (read["e.Concentration"].ToString() != "")
                        anesPlanPremed.AnticholinergicDrug.Concentration = Convert.ToDecimal(read["e.Concentration"].ToString());
                    if (read["e.MaxDosage"].ToString() != "")
                        anesPlanPremed.AnticholinergicDrug.MaxDosage = Convert.ToDecimal(read["e.MaxDosage"].ToString());
                }
                else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_ROUTE_WITH_DETAILS && anesPlanPremed.Route.Id != -1)
                {
                    anesPlanPremed.Route.Category.Id = Convert.ToInt32(read["c.CategoryId"].ToString());
                    anesPlanPremed.Route.Label = read["c.Label"].ToString();
                    anesPlanPremed.Route.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    anesPlanPremed.Route.Description = read["c.Description"].ToString();
                    if (read["c.Concentration"].ToString() != "")
                        anesPlanPremed.Route.Concentration = Convert.ToDecimal(read["c.Concentration"].ToString());
                }
            }

            return anesPlanPremed;
        }
    }
}