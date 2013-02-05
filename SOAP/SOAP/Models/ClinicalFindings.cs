using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class ClinicalFindings
    {
        private int _id;
        private int _patientId;
        private int _preOperationPainAssessmentId;
        private int _postOperationPainAssessmentId;
        private decimal _temperature;
        private decimal _pulseRate;
        private decimal _respiratoryRate;
        private int _cardiacAuscultationId;
        private int _pulseQualityId;
        private decimal _capillaryRefillTime;
        private int _respiratoryAuscultationId;
        private int _physicalStatusClassId;
        private string _reasonForClassification;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public int PreOperationPainAssessmentId
        {
            get { return _preOperationPainAssessmentId; }
            set { _preOperationPainAssessmentId = value; }
        }

        public int PostOperationPainAssessmentId
        {
            get { return _postOperationPainAssessmentId; }
            set { _postOperationPainAssessmentId = value; }
        }

        public decimal Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

        public decimal PulseRate
        {
            get { return _pulseRate; }
            set { _pulseRate = value; }
        }

        public decimal RespiratoryRate
        {
            get { return _respiratoryRate; }
            set { _respiratoryRate = value; }
        }

        public int CardiacAuscultationId
        {
            get { return _cardiacAuscultationId; }
            set { _cardiacAuscultationId = value; }
        }

        public int PulseQualityId
        {
            get { return _pulseQualityId; }
            set { _pulseQualityId = value; }
        }

        public decimal CapillaryRefillTime
        {
            get { return _capillaryRefillTime; }
            set { _capillaryRefillTime = value; }
        }

        public int RespiratoryAuscultationId
        {
            get { return _respiratoryAuscultationId; }
            set { _respiratoryAuscultationId = value; }
        }

        public int PhysicalStatusClassId
        {
            get { return _physicalStatusClassId; }
            set { _physicalStatusClassId = value; }
        }

        public string ReasonForClassification
        {
            get { return _reasonForClassification; }
            set { _reasonForClassification = value; }
        }

        public ClinicalFindings()
        {
            _id = -1;
        }

        public bool ValidateClinicalFindings()
        {
            if (_id == 0 || _patientId == 0)
                return false;
            else
                return true;
        }
    }
}