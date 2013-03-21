using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class CurrentMedicationCallback
    {
        public CurrentMedication ProcessRow(SqlDataReader read, params CurrentMedication.LazyComponents[] lazyComponents)
        {
            CurrentMedication med = new CurrentMedication();
            med.Id = Convert.ToInt32(read["a.Id"]);
            med.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.MedicationId"].ToString() != "")
                med.Medication.Id = Convert.ToInt32(read["a.MedicationId"].ToString());

            foreach (CurrentMedication.LazyComponents a in lazyComponents)
            {
                if (a == CurrentMedication.LazyComponents.LOAD_CURRENT_MEDICATIONS_WITH_DETAILS && med.Medication.Id != -1)
                {
                    med.Medication.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    med.Medication.Label = read["b.Label"].ToString();
                    med.Medication.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    med.Medication.Description = read["b.Description"].ToString();
                }
            }

            return med;
        }
    }
}