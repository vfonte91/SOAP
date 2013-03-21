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
            patientInfo.Student.Username = read["a.StudentId"].ToString();
            patientInfo.Clinician.Username = read["a.ClinicianId"].ToString();
            patientInfo.FormCompleted = Convert.ToChar(read["a.FormCompleted"].ToString());
            if (read["a.TemperamentId"].ToString() != "")
                patientInfo.Temperament.Id = Convert.ToInt32(read["a.TemperamentId"].ToString());
            if (read["a.DateSeenOn"].ToString() != "")
                patientInfo.DateSeenOn = Convert.ToDateTime(read["a.DateSeenOn"].ToString());
            if (read["a.CageOrStallNumber"].ToString() != "")
                patientInfo.CageOrStallNumber = Convert.ToInt32(read["a.CageOrStallNumber"].ToString());
            if (read["a.BodyWeight"].ToString() != "")
                patientInfo.BodyWeight = Convert.ToDecimal(read["a.BodyWeight"].ToString());
            if (read["a.AgeInYears"].ToString() != "")
                patientInfo.AgeInYears = Convert.ToInt32(read["a.AgeInYears"].ToString());
            if (read["a.AgeInMonths"].ToString() != "")
                patientInfo.AgeInMonths = Convert.ToInt32(read["a.AgeInMonths"].ToString());
            if (read["a.PreOpPainAssessmentId"].ToString() != "")
                patientInfo.PreOperationPainAssessment.Id = Convert.ToInt32(read["a.PreOpPainAssessmentId"].ToString());
            if (read["a.PostOpPainAssessmentId"].ToString() != "") 
                patientInfo.PostOperationPainAssessment.Id = Convert.ToInt32(read["a.PostOpPainAssessmentId"].ToString());

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
                else if (a == PatientInformation.LazyComponents.LOAD_POSTOP_PAIN_DETAIL && patientInfo.PostOperationPainAssessment.Id != -1)
                {
                        patientInfo.PostOperationPainAssessment.Category.Id = Convert.ToInt32(read["d.CategoryId"].ToString());
                        patientInfo.PostOperationPainAssessment.Label = read["d.Label"].ToString();
                        patientInfo.PostOperationPainAssessment.OtherFlag = Convert.ToChar(read["d.OtherFlag"].ToString());
                        patientInfo.PostOperationPainAssessment.Description = read["d.Description"].ToString();
                }
                else if (a == PatientInformation.LazyComponents.LOAD_PREOP_PAIN_DETAIL && patientInfo.PreOperationPainAssessment.Id != -1)
                {
                        patientInfo.PreOperationPainAssessment.Category.Id = Convert.ToInt32(read["e.CategoryId"].ToString());
                        patientInfo.PreOperationPainAssessment.Label = read["e.Label"].ToString();
                        patientInfo.PreOperationPainAssessment.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                        patientInfo.PreOperationPainAssessment.Description = read["e.Description"].ToString();
                }
                else if (a == PatientInformation.LazyComponents.LOAD_TEMPERAMENT_DETAIL && patientInfo.Temperament.Id != -1)
                {
                        patientInfo.Temperament.Category.Id = Convert.ToInt32(read["f.CategoryId"].ToString());
                        patientInfo.Temperament.Label = read["f.Label"].ToString();
                        patientInfo.Temperament.OtherFlag = Convert.ToChar(read["f.OtherFlag"].ToString());
                        patientInfo.Temperament.Description = read["f.Description"].ToString();
                }
            }

            return patientInfo;
        }
    }
}