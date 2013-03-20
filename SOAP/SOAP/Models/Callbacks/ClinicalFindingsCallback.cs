using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class ClinicalFindingsCallback
    {
        public ClinicalFindings ProcessRow(SqlDataReader read, params ClinicalFindings.LazyComponents[] lazyComponents)
        {
            ClinicalFindings clinicalFindings = new ClinicalFindings();
            clinicalFindings.Id = Convert.ToInt32(read["a.Id"]);
            clinicalFindings.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            clinicalFindings.Temperature = Convert.ToDecimal(read["a.Temperature"].ToString());
            clinicalFindings.PulseRate = Convert.ToDecimal(read["a.PulseRate"].ToString());
            clinicalFindings.RespiratoryRate = Convert.ToDecimal(read["a.RespiratoryRate"].ToString());
            clinicalFindings.CardiacAuscultation.Id = Convert.ToInt32(read["a.CardiacAuscultationId"].ToString());
            clinicalFindings.PulseQuality.Id = Convert.ToInt32(read["a.PulseQualityId"].ToString());
            clinicalFindings.MucousMembraneColor.Id = Convert.ToInt32(read["a.MucousMembraneColor"].ToString());
            clinicalFindings.CapillaryRefillTime = Convert.ToDecimal(read["a.CapillaryRefillTime"].ToString());
            clinicalFindings.RespiratoryAuscultation.Id = Convert.ToInt32(read["a.RespiratoryAuscultationId"].ToString());
            clinicalFindings.PhysicalStatusClassification.Id = Convert.ToInt32(read["a.PhysicalStatusClassId"].ToString());
            clinicalFindings.ReasonForClassification = read["a.ReasonForClassification"].ToString();

            foreach (ClinicalFindings.LazyComponents a in lazyComponents)
            {
                if (a == ClinicalFindings.LazyComponents.LOAD_CARDIAC_WITH_DETAILS)
                {
                    clinicalFindings.CardiacAuscultation.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    clinicalFindings.CardiacAuscultation.Label = read["b.Label"].ToString();
                    clinicalFindings.CardiacAuscultation.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    clinicalFindings.CardiacAuscultation.Description = read["b.Description"].ToString();
                }
                else if (a == ClinicalFindings.LazyComponents.LOAD_PHYSICAL_STATUS_WITH_DETAILS)
                {
                    clinicalFindings.PhysicalStatusClassification.Category.Id = Convert.ToInt32(read["c.CategoryId"].ToString());
                    clinicalFindings.PhysicalStatusClassification.Label = read["c.Label"].ToString();
                    clinicalFindings.PhysicalStatusClassification.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    clinicalFindings.PhysicalStatusClassification.Description = read["c.Description"].ToString();
                }
                else if (a == ClinicalFindings.LazyComponents.LOAD_PULSE_QUALITY_WITH_DETAILS)
                {
                    clinicalFindings.PulseQuality.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                    clinicalFindings.PulseQuality.Label = read["d.Label"].ToString();
                    clinicalFindings.PulseQuality.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    clinicalFindings.PulseQuality.Description = read["d.Description"].ToString();
                }
                else if (a == ClinicalFindings.LazyComponents.LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS)
                {
                    clinicalFindings.RespiratoryAuscultation.Category.Id = Convert.ToInt32(read["e.CategoryId"].ToString());
                    clinicalFindings.RespiratoryAuscultation.Label = read["e.Label"].ToString();
                    clinicalFindings.RespiratoryAuscultation.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                    clinicalFindings.RespiratoryAuscultation.Description = read["e.Description"].ToString();
                }
            }

            return clinicalFindings;
        }
    }
}