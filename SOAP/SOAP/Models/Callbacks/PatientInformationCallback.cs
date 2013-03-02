using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class PatientInformationCallback
    {
        public PatientInformation ProcessRow(SqlDataReader read, params PatientInformation.LazyComponents[] lazyComponents)
        {
            PatientInformation patientInfo = new PatientInformation();
            patientInfo.PatientId = Convert.ToInt32(read["a.PatientId"]);
            patientInfo.Student.UserId = (Guid)read["a.StudentId"];
            patientInfo.Clinician.UserId = (Guid)read["a.ClinicianId"];
            patientInfo.FormCompleted = Convert.ToChar(read["a.ClinicianId"].ToString());
            patientInfo.Temperament.Id = Convert.ToInt16(read["a.TemperamentId"].ToString());
            patientInfo.DateSeenOn = Convert.ToDateTime(read["a.DateSeenOn"].ToString());
            patientInfo.CageOrStallNumber = Convert.ToInt32(read["a.CageOrStallNumber"].ToString());
            patientInfo.BodyWeight = Convert.ToDecimal(read["a.BodyWeight"].ToString());
            patientInfo.AgeInYears = Convert.ToInt16(read["a.AgeInYears"].ToString());
            patientInfo.AgeInMonths = Convert.ToInt16(read["a.AgeInMonths"].ToString());
            patientInfo.PreOperationPainAssessment.Id = Convert.ToInt16(read["a.PreOpPainAssessmentId"].ToString());
            patientInfo.PostOperationPainAssessment.Id = Convert.ToInt16(read["a.PostOpPainAssessmentId"].ToString());

            foreach (PatientInformation.LazyComponents a in lazyComponents)
            {
                if (a == PatientInformation.LazyComponents.LOAD_CLINICIAN_DETAIL)
                {
                    patientInfo.Clinician.Username = read["b.Username"].ToString();
                    patientInfo.Clinician.FullName = read["b.FullName"].ToString();
                    patientInfo.Clinician.EmailAddress = read["b.Email"].ToString();
                }
                else if (a == PatientInformation.LazyComponents.LOAD_STUDENT_DETAIL)
                {
                    patientInfo.Student.Username = read["c.Username"].ToString();
                    patientInfo.Student.FullName = read["c.FullName"].ToString();
                    patientInfo.Student.EmailAddress = read["c.Email"].ToString();
                }
                else if (a == PatientInformation.LazyComponents.LOAD_POSTOP_PAIN_DETAIL)
                {
                    patientInfo.PostOperationPainAssessment.Category.Id = Convert.ToInt16(read["d.CategoryId"].ToString());
                    patientInfo.PostOperationPainAssessment.Label = read["d.Label"].ToString();
                    patientInfo.PostOperationPainAssessment.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                    patientInfo.PostOperationPainAssessment.Description = read["d.Description"].ToString();
                }
                else if (a == PatientInformation.LazyComponents.LOAD_PREOP_PAIN_DETAIL)
                {
                    patientInfo.PreOperationPainAssessment.Category.Id = Convert.ToInt16(read["e.CategoryId"].ToString());
                    patientInfo.PreOperationPainAssessment.Label = read["e.Label"].ToString();
                    patientInfo.PreOperationPainAssessment.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                    patientInfo.PreOperationPainAssessment.Description = read["e.Description"].ToString();
                }
                else if (a == PatientInformation.LazyComponents.LOAD_TEMPERAMENT_DETAIL)
                {
                    patientInfo.Temperament.Category.Id = Convert.ToInt16(read["f.CategoryId"].ToString());
                    patientInfo.Temperament.Label = read["f.Label"].ToString();
                    patientInfo.Temperament.OtherFlag = Convert.ToChar(read["f.OtherFlag"].ToString());
                    patientInfo.Temperament.Description = read["f.Description"].ToString();
                }
            }

            return patientInfo;
        }
    }
}