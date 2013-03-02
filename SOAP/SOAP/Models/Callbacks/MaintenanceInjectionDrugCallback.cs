using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class MaintenanceInjectionDrugCallback
    {
        public MaintenanceInjectionDrug ProcessRow(SqlDataReader read, params MaintenanceInjectionDrug.LazyComponents[] lazyComponents)
        {
            MaintenanceInjectionDrug maintInjectDrug = new MaintenanceInjectionDrug();
            maintInjectDrug.Id = Convert.ToInt32(read["a.Id"]);
            maintInjectDrug.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            maintInjectDrug.Drug.Drug.Id = Convert.ToInt16(read["a.DrugId"].ToString());
            maintInjectDrug.RouteOfAdministration.Id = Convert.ToInt16(read["a.RouteOfAdministrationId"].ToString());
            maintInjectDrug.Dose = Convert.ToDecimal(read["a.Dosage"].ToString());

            foreach (MaintenanceInjectionDrug.LazyComponents a in lazyComponents)
            {
                if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_DRUG_INFORMATION)
                {
                    maintInjectDrug.Drug.Drug.Category.Id = Convert.ToInt16(read["d.CategoryId"].ToString());
                    maintInjectDrug.Drug.Drug.Label = read["d.Label"].ToString();
                    maintInjectDrug.Drug.Drug.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    maintInjectDrug.Drug.Drug.Description = read["b.Description"].ToString();
                    maintInjectDrug.Drug.Id = Convert.ToInt32(read["b.Id"].ToString());
                    maintInjectDrug.Drug.DoseMinRange = Convert.ToDecimal(read["b.DoseMinRange"].ToString());
                    maintInjectDrug.Drug.DoseMaxRange = Convert.ToDecimal(read["b.DoseMaxRange"].ToString());
                    maintInjectDrug.Drug.DoseMax = Convert.ToDecimal(read["b.DoseMax"].ToString());
                    maintInjectDrug.Drug.DoseUnits = read["b.DoseUnits"].ToString();
                    maintInjectDrug.Drug.Route = read["b.Route"].ToString();
                    maintInjectDrug.Drug.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                    maintInjectDrug.Drug.ConcentrationUnits = read["b.ConcentrationUnits"].ToString();
                }
                if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                {
                    maintInjectDrug.RouteOfAdministration.Category.Id = Convert.ToInt16(read["c.CategoryId"].ToString());
                    maintInjectDrug.RouteOfAdministration.Label = read["c.Label"].ToString();
                    maintInjectDrug.RouteOfAdministration.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    maintInjectDrug.RouteOfAdministration.Description = read["c.Description"].ToString();
                }
            }

            return maintInjectDrug;
        }
    }
}