using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class MaintenanceInhalantDrugCallback
    {
        public MaintenanceInhalantDrug ProcessRow(SqlDataReader read, params MaintenanceInhalantDrug.LazyComponents[] lazyComponents)
        {
            MaintenanceInhalantDrug maintInhalantDrug = new MaintenanceInhalantDrug();
            maintInhalantDrug.Id = Convert.ToInt32(read["a.Id"]);
            maintInhalantDrug.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.DrugId"].ToString() != "")
                maintInhalantDrug.Drug.Id = Convert.ToInt32(read["a.DrugId"].ToString());
            if (read["a.InductionDose"].ToString() != "")
                maintInhalantDrug.InductionPercentage = Convert.ToDecimal(read["a.InductionDose"].ToString());
            if (read["a.InductionOxygenFlowRate"].ToString() != "")
                maintInhalantDrug.InductionOxygenFlowRate = Convert.ToDecimal(read["a.InductionOxygenFlowRate"].ToString());
            if (read["a.MaintenanceDose"].ToString() != "")
                maintInhalantDrug.MaintenancePercentage = Convert.ToDecimal(read["a.MaintenanceDose"].ToString());
            if (read["a.MaintenanceOxygenFlowRate"].ToString() != "")
                maintInhalantDrug.MaintenanceOxygenFlowRate = Convert.ToDecimal(read["a.MaintenanceOxygenFlowRate"].ToString());
            if (read["a.BreathingSystemId"].ToString() != "")
                maintInhalantDrug.BreathingSystem.Id = Convert.ToInt32(read["a.BreathingSystemId"].ToString());
            if (read["a.BreathingBagSizeId"].ToString() != "")
                maintInhalantDrug.BreathingBagSize.Id = Convert.ToInt32(read["a.BreathingBagSizeId"].ToString());
            maintInhalantDrug.OtherAnestheticDrug = read["a.OtherAnestheticDrugs"].ToString();
            if (read["a.IntraoperativeAnalgesiaId"].ToString() != "")
                maintInhalantDrug.IntraoperativeAnalgesia.Id = Convert.ToInt32(read["a.IntraoperativeAnalgesiaId"].ToString());

            foreach (MaintenanceInhalantDrug.LazyComponents a in lazyComponents)
            {
                if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS && maintInhalantDrug.Drug.Id != -1)
                {
                    if (read["b.CategoryId"].ToString() != "")
                        maintInhalantDrug.Drug.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    maintInhalantDrug.Drug.Label = read["b.Label"].ToString();
                    maintInhalantDrug.Drug.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    maintInhalantDrug.Drug.Description = read["b.Description"].ToString();
                }
                else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_DETAILS && maintInhalantDrug.BreathingBagSize.Id != -1)
                {
                    if (read["c.CategoryId"].ToString() != "")
                    maintInhalantDrug.BreathingBagSize.Category.Id = Convert.ToInt32(read["c.CategoryId"].ToString());
                    maintInhalantDrug.BreathingBagSize.Label = read["c.Label"].ToString();
                    maintInhalantDrug.BreathingBagSize.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    maintInhalantDrug.BreathingBagSize.Description = read["c.Description"].ToString();
                }
                else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS && maintInhalantDrug.BreathingSystem.Id != -1)
                {
                    if (read["d.CategoryId"].ToString() != "")
                    maintInhalantDrug.BreathingSystem.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    maintInhalantDrug.BreathingSystem.Label = read["d.Label"].ToString();
                    maintInhalantDrug.BreathingSystem.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    maintInhalantDrug.BreathingSystem.Description = read["d.Description"].ToString();
                }
                else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_INTRAOP_WITH_DETAILS && maintInhalantDrug.IntraoperativeAnalgesia.Id != -1)
                {
                    if (read["e.CategoryId"].ToString() != "")
                    maintInhalantDrug.IntraoperativeAnalgesia.Category.Id = Convert.ToInt32(read["e.CategoryId"].ToString());
                    maintInhalantDrug.IntraoperativeAnalgesia.Label = read["e.Label"].ToString();
                    maintInhalantDrug.IntraoperativeAnalgesia.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                    maintInhalantDrug.IntraoperativeAnalgesia.Description = read["e.Description"].ToString();
                }
            }

            return maintInhalantDrug;
        }
    }
}