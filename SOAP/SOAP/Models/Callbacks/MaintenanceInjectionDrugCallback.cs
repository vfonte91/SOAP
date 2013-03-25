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
            if (read["a.DrugId"].ToString() != "")
                maintInjectDrug.Drug.Drug.Id = Convert.ToInt32(read["a.DrugId"].ToString());
            if (read["a.RouteOfAdministrationId"].ToString() != "")
                maintInjectDrug.RouteOfAdministration.Id = Convert.ToInt32(read["a.RouteOfAdministrationId"].ToString());
            if (read["a.Dosage"].ToString() != "")
                maintInjectDrug.Dose = Convert.ToDecimal(read["a.Dosage"].ToString());
            maintInjectDrug.OtherAnestheticDrug = read["a.OtherAnestheticDrugs"].ToString();
            if (read["a.IntraoperativeAnalgesiaId"].ToString() != "")
                maintInjectDrug.IntroaperativeAnalgesia.Id = Convert.ToInt32(read["a.IntraoperativeAnalgesiaId"].ToString());

            foreach (MaintenanceInjectionDrug.LazyComponents a in lazyComponents)
            {
                if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_DRUG_INFORMATION && maintInjectDrug.Drug.Drug.Id != -1)
                {
                    if (read["d.CategoryId"].ToString() != "")
                        maintInjectDrug.Drug.Drug.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    maintInjectDrug.Drug.Drug.Label = read["d.Label"].ToString();
                    maintInjectDrug.Drug.Drug.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    maintInjectDrug.Drug.Drug.Description = read["b.Description"].ToString();
                    if (read["b.Id"].ToString() != "")
                        maintInjectDrug.Drug.Id = Convert.ToInt32(read["b.Id"].ToString());
                    if (read["b.DoseMinRange"].ToString() != "")
                        maintInjectDrug.Drug.DoseMinRange = Convert.ToDecimal(read["b.DoseMinRange"].ToString());
                    if (read["b.DoseMaxRange"].ToString() != "")
                        maintInjectDrug.Drug.DoseMaxRange = Convert.ToDecimal(read["b.DoseMaxRange"].ToString());
                    if (read["b.DoseMax"].ToString() != "")
                        maintInjectDrug.Drug.DoseMax = Convert.ToDecimal(read["b.DoseMax"].ToString());
                    maintInjectDrug.Drug.DoseUnits = read["b.DoseUnits"].ToString();
                    maintInjectDrug.Drug.Route = read["b.Route"].ToString();
                    if (read["b.Concentration"].ToString() != "")
                        maintInjectDrug.Drug.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                    maintInjectDrug.Drug.ConcentrationUnits = read["b.ConcentrationUnits"].ToString();
                }
                else if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS && maintInjectDrug.RouteOfAdministration.Id != -1)
                {
                    if (read["c.CategoryId"].ToString() != "")
                        maintInjectDrug.RouteOfAdministration.Category.Id = Convert.ToInt32(read["c.CategoryId"].ToString());
                    maintInjectDrug.RouteOfAdministration.Label = read["c.Label"].ToString();
                    maintInjectDrug.RouteOfAdministration.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    maintInjectDrug.RouteOfAdministration.Description = read["c.Description"].ToString();
                }
                else if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_INTRAOP_WITH_DETAILS && maintInjectDrug.IntroaperativeAnalgesia.Id != -1)
                {
                    if (read["e.CategoryId"].ToString() != "")
                        maintInjectDrug.IntroaperativeAnalgesia.Category.Id = Convert.ToInt32(read["e.CategoryId"].ToString());
                    maintInjectDrug.IntroaperativeAnalgesia.Label = read["e.Label"].ToString();
                    maintInjectDrug.IntroaperativeAnalgesia.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                    maintInjectDrug.IntroaperativeAnalgesia.Description = read["e.Description"].ToString();
                }
            }

            return maintInjectDrug;
        }
    }
}