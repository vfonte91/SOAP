using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class ClinicalFindings
    {
        private int _id;
        private int _patientId;
        private int _preOperationPainAssessmentId;
        private string _preOperationPainAssessmentName;
        private int _postOperationPainAssessmentId;
        private string _postOperationPainAssessmentName;
        private decimal _temperature;
        private decimal _pulseRate;
        private decimal _respiratoryRate;
        private int _cardiacAuscultationId;
        private string _cardiacAuscultationName;
        private int _pulseQualityId;
        private string _pulseQualityName;
        private string _mucousMembraneColor;
        private decimal _capillaryRefillTime;
        private int _respiratoryAuscultationId;
        private string _respiratoryAuscultationName;
        private int _physicalStatusClassId;
        private string _physicalStatusClassName;
        private string _reasonForClassification;
        private List<CurrentMedication> _currentMedications;
        private List<Bloodwork> _bloodwork;
        private List<PriorAnesthesia> _priorAnesthesia;

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

        public string PreOperationPainAssessmentName
        {
            get { return _preOperationPainAssessmentName; }
            set { _preOperationPainAssessmentName = value; }
        }

        public int PostOperationPainAssessmentId
        {
            get { return _postOperationPainAssessmentId; }
            set { _postOperationPainAssessmentId = value; }
        }

        public string PostOperationPainAssessmentName
        {
            get { return _postOperationPainAssessmentName; }
            set { _postOperationPainAssessmentName = value; }
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

        public string CardiacAuscultationName
        {
            get { return _cardiacAuscultationName; }
            set { _cardiacAuscultationName = value; }
        }

        public int PulseQualityId
        {
            get { return _pulseQualityId; }
            set { _pulseQualityId = value; }
        }

        public string PulseQualityName
        {
            get { return _pulseQualityName; }
            set { _pulseQualityName = value; }
        }

        public string MucousMembraneColor
        {
            get { return _mucousMembraneColor; }
            set { _mucousMembraneColor = value; }
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

        public string RespiratoryAuscultationName
        {
            get { return _respiratoryAuscultationName; }
            set { _respiratoryAuscultationName = value; }
        }

        public int PhysicalStatusClassId
        {
            get { return _physicalStatusClassId; }
            set { _physicalStatusClassId = value; }
        }

        public string PhysicalStatusClassName
        {
            get { return _physicalStatusClassName; }
            set { _physicalStatusClassName = value; }
        }

        public string ReasonForClassification
        {
            get { return _reasonForClassification; }
            set { _reasonForClassification = value; }
        }

        public List<CurrentMedication> CurrentMedications
        {
            get { return _currentMedications; }
            set { _currentMedications = value; }
        }

        public List<Bloodwork> Bloodwork
        {
            get { return _bloodwork; }
            set { _bloodwork = value; }
        }

        public List<PriorAnesthesia> PriorAnesthesia
        {
            get { return _priorAnesthesia; }
            set { _priorAnesthesia = value; }
        }

        public ClinicalFindings()
        {
            _id = -1;
            _currentMedications = new List<CurrentMedication>();
            _bloodwork = new List<Bloodwork>();
            _priorAnesthesia = new List<PriorAnesthesia>();
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