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
            maintInhalantDrug.InductionReqFlag = Convert.ToChar(read["a.InductionReqFlag"].ToString());
            if (read["a.InductionDose"].ToString() != "")
                maintInhalantDrug.InductionDose = Convert.ToDecimal(read["a.InductionDose"].ToString());
            if (read["a.InductionOxygenFlowRate"].ToString() != "")
                maintInhalantDrug.InductionOxygenFlowRate = Convert.ToDecimal(read["a.InductionOxygenFlowRate"].ToString());
            maintInhalantDrug.MaintenanceReqFlag = Convert.ToChar(read["a.MaintenanceReqFlag"].ToString());
            if (read["a.MaintenanceDose"].ToString() != "")
                maintInhalantDrug.MaintenanceDose = Convert.ToDecimal(read["a.MaintenanceDose"].ToString());
            if (read["a.MaintenanceOxygenFlowRate"].ToString() != "")
                maintInhalantDrug.MaintenanceOxygenFlowRate = Convert.ToDecimal(read["a.MaintenanceOxygenFlowRate"].ToString());
            maintInhalantDrug.EquipmentReqFlag = Convert.ToChar(read["a.EquipmentReqFlag"].ToString());
            if (read["a.BreathingSystemId"].ToString() != "")
                maintInhalantDrug.BreathingSystem.Id = Convert.ToInt32(read["a.BreathingSystemId"].ToString());
            if (read["a.BreathingBagSizeId"].ToString() != "")
                maintInhalantDrug.BreathingBagSize.Id = Convert.ToInt32(read["a.BreathingBagSizeId"].ToString());

            foreach (MaintenanceInhalantDrug.LazyComponents a in lazyComponents)
            {
                if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS && maintInhalantDrug.Drug.Id != -1)
                {
                    maintInhalantDrug.Drug.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    maintInhalantDrug.Drug.Label = read["b.Label"].ToString();
                    maintInhalantDrug.Drug.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    maintInhalantDrug.Drug.Description = read["b.Description"].ToString();
                }
                else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_DETAILS && maintInhalantDrug.BreathingBagSize.Id != -1)
                {
                    maintInhalantDrug.BreathingBagSize.Category.Id = Convert.ToInt32(read["c.CategoryId"].ToString());
                    maintInhalantDrug.BreathingBagSize.Label = read["c.Label"].ToString();
                    maintInhalantDrug.BreathingBagSize.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    maintInhalantDrug.BreathingBagSize.Description = read["c.Description"].ToString();
                }
                else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS && maintInhalantDrug.BreathingSystem.Id != -1)
                {
                    maintInhalantDrug.BreathingSystem.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    maintInhalantDrug.BreathingSystem.Label = read["d.Label"].ToString();
                    maintInhalantDrug.BreathingSystem.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    maintInhalantDrug.BreathingSystem.Description = read["d.Description"].ToString();
                }
            }

            return maintInhalantDrug;
        }
    }
}