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
            if (read["a.Temperature"].ToString() != "")
                clinicalFindings.Temperature = Convert.ToDecimal(read["a.Temperature"]);
            if (read["a.PulseRate"].ToString() != "")
                clinicalFindings.PulseRate = Convert.ToDecimal(read["a.PulseRate"]);
            if (read["a.RespiratoryRate"].ToString() != "")
                clinicalFindings.RespiratoryRate = Convert.ToDecimal(read["a.RespiratoryRate"]);
            if (read["a.CardiacAuscultationId"].ToString() != "")
                clinicalFindings.CardiacAuscultation.Id = Convert.ToInt32(read["a.CardiacAuscultationId"]);
            if (read["a.PulseQualityId"].ToString() != "")
                clinicalFindings.PulseQuality.Id = Convert.ToInt32(read["a.PulseQualityId"]);
            if (read["a.MucousMembraneColorId"].ToString() != "")
                clinicalFindings.MucousMembraneColor.Id = Convert.ToInt32(read["a.MucousMembraneColorId"]);
            if (read["a.CapillaryRefillTime"].ToString() != "")
                clinicalFindings.CapillaryRefillTime = Convert.ToDecimal(read["a.CapillaryRefillTime"]);
            if (read["a.RespiratoryAuscultationId"].ToString() != "")
                clinicalFindings.RespiratoryAuscultation.Id = Convert.ToInt32(read["a.RespiratoryAuscultationId"]);
            if (read["a.PhysicalStatusClassId"].ToString() != "")
                clinicalFindings.PhysicalStatusClassification.Id = Convert.ToInt32(read["a.PhysicalStatusClassId"]);
            clinicalFindings.ReasonForClassification = read["a.ReasonForClassification"].ToString();

            foreach (ClinicalFindings.LazyComponents a in lazyComponents)
            {
                if (a == ClinicalFindings.LazyComponents.LOAD_CARDIAC_WITH_DETAILS && clinicalFindings.CardiacAuscultation.Id != -1)
                {
                    clinicalFindings.CardiacAuscultation.Category.Id = Convert.ToInt32(read["b.CategoryId"]);
                    clinicalFindings.CardiacAuscultation.Label = read["b.Label"].ToString();
                    clinicalFindings.CardiacAuscultation.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    clinicalFindings.CardiacAuscultation.Description = read["b.Description"].ToString();
                }
                else if (a == ClinicalFindings.LazyComponents.LOAD_PHYSICAL_STATUS_WITH_DETAILS && clinicalFindings.PhysicalStatusClassification.Id != -1)
                {
                    clinicalFindings.PhysicalStatusClassification.Category.Id = Convert.ToInt32(read["c.CategoryId"]);
                    clinicalFindings.PhysicalStatusClassification.Label = read["c.Label"].ToString();
                    clinicalFindings.PhysicalStatusClassification.OtherFlag = Convert.ToChar(read["c.OtherFlag"].ToString());
                    clinicalFindings.PhysicalStatusClassification.Description = read["c.Description"].ToString();
                }
                else if (a == ClinicalFindings.LazyComponents.LOAD_PULSE_QUALITY_WITH_DETAILS && clinicalFindings.PulseQuality.Id != -1)
                {
                    clinicalFindings.PulseQuality.Category.Id = Convert.ToInt32(read["d.CategoryId"]);
                    clinicalFindings.PulseQuality.Label = read["d.Label"].ToString();
                    clinicalFindings.PulseQuality.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    clinicalFindings.PulseQuality.Description = read["d.Description"].ToString();
                }
                else if (a == ClinicalFindings.LazyComponents.LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS && clinicalFindings.RespiratoryAuscultation.Id != -1)
                {
                    clinicalFindings.RespiratoryAuscultation.Category.Id = Convert.ToInt32(read["e.CategoryId"]);
                    clinicalFindings.RespiratoryAuscultation.Label = read["e.Label"].ToString();
                    clinicalFindings.RespiratoryAuscultation.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                    clinicalFindings.RespiratoryAuscultation.Description = read["e.Description"].ToString();
                }
                else if (a == ClinicalFindings.LazyComponents.LOAD_MUCOUS_MEMBRANE_WITH_DETAILS && clinicalFindings.MucousMembraneColor.Id != -1)
                {
                    clinicalFindings.MucousMembraneColor.Category.Id = Convert.ToInt32(read["f.CategoryId"]);
                    clinicalFindings.MucousMembraneColor.Label = read["f.Label"].ToString();
                    clinicalFindings.MucousMembraneColor.OtherFlag = Convert.ToChar(read["f.OtherFlag"].ToString());
                    clinicalFindings.MucousMembraneColor.Description = read["f.Description"].ToString();
                }
            }

            return clinicalFindings;
        }
    }
}