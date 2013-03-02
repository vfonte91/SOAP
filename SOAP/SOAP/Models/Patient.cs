using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Patient
    {
        private int _patientId;
        private PatientInformation _patientInfo;
        private ClinicalFindings _clinicalFindings;
        private List<Bloodwork> _bloodworkGroup;
        private AnestheticPlan _anestheticPlan;
        private Maintenance _maintenance;
        private List<Monitoring> _monitoring;

        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public PatientInformation PatientInfo
        {
            get { return _patientInfo; }
            set { _patientInfo = value; }
        }

        public ClinicalFindings ClinicalFindings
        {
            get { return _clinicalFindings; }
            set { _clinicalFindings = value; }
        }

        public List<Bloodwork> BloodworkGroup
        {
            get { return _bloodworkGroup; }
            set { _bloodworkGroup = value; }
        }

        public AnestheticPlan AnestheticPlan
        {
            get { return _anestheticPlan; }
            set { _anestheticPlan = value; }
        }

        public Maintenance Maintenance
        {
            get { return _maintenance; }
            set { _maintenance = value; }
        }

        public List<Monitoring> Monitoring
        {
            get { return _monitoring; }
            set { _monitoring = value; }
        }

        public Patient()
        {
            _patientId = -1;
        }

        public bool ValidatePatient()
        {
            if (_patientId == 0)
                return false;
            else
                return true;
        }
    }
}