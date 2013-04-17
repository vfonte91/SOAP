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
            {
                maintInhalantDrug.Drug.Id = Convert.ToInt32(read["a.DrugId"].ToString());
                maintInhalantDrug.Checked = true;
            }
            else
                maintInhalantDrug.Checked = false;
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

            foreach (MaintenanceInhalantDrug.LazyComponents a in lazyComponents)
            {
                if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS && maintInhalantDrug.Drug.Id != -1)
                {
                    if (read["b.CategoryId"].ToString() != "")
                        maintInhalantDrug.Drug.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    maintInhalantDrug.Drug.Label = read["b.Label"].ToString();
                    maintInhalantDrug.Drug.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    maintInhalantDrug.Drug.Description = read["b.Description"].ToString();
                    if (read["b.Concentration"].ToString() != "")
                        maintInhalantDrug.Drug.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                    if (read["b.MaxDosage"].ToString() != "")
                        maintInhalantDrug.Drug.MaxDosage = Convert.ToDecimal(read["b.MaxDosage"].ToString());
                }
                else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_DETAILS && maintInhalantDrug.BreathingBagSize.Id != -1)
                {
                    if (read["c.CategoryId"].ToString() != "")
                    maintInhalantDrug.BreathingBagSize.Category.Id = Convert.ToInt32(read["c.CategoryId"].ToString());
                    maintInhalantDrug.BreathingBagSize.Label = read["c.Label"].ToString();
                    maintInhalantDrug.BreathingBagSize.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    maintInhalantDrug.BreathingBagSize.Description = read["c.Description"].ToString();
                    if (read["c.Concentration"].ToString() != "")
                        maintInhalantDrug.BreathingBagSize.Concentration = Convert.ToDecimal(read["c.Concentration"].ToString());
                }
                else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS && maintInhalantDrug.BreathingSystem.Id != -1)
                {
                    if (read["d.CategoryId"].ToString() != "")
                    maintInhalantDrug.BreathingSystem.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    maintInhalantDrug.BreathingSystem.Label = read["d.Label"].ToString();
                    maintInhalantDrug.BreathingSystem.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    maintInhalantDrug.BreathingSystem.Description = read["d.Description"].ToString();
                    if (read["d.Concentration"].ToString() != "")
                        maintInhalantDrug.BreathingSystem.Concentration = Convert.ToDecimal(read["d.Concentration"].ToString());
                }
            }

            return maintInhalantDrug;
        }
    }
}