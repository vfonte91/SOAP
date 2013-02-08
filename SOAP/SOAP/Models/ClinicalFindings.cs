using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class ClinicalFindings
    {
        private int _id;
        private int _patientId;
        private DropdownValue _preOperationPainAssessment;
        private DropdownValue _postOperationPainAssessment;
        private decimal _temperature;
        private decimal _pulseRate;
        private decimal _respiratoryRate;
        private DropdownValue _cardiacAuscultation;
        private DropdownValue _pulseQuality;
        private string _mucousMembraneColor;
        private decimal _capillaryRefillTime;
        private DropdownValue _respiratoryAuscultation;
        private DropdownValue _physicalStatusClass;
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

        public DropdownValue PreOperationPainAssessment
        {
            get { return _preOperationPainAssessment; }
            set { _preOperationPainAssessment = value; }
        }

        public DropdownValue PostOperationPainAssessment
        {
            get { return _postOperationPainAssessment; }
            set { _postOperationPainAssessment = value; }
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

        public DropdownValue CardiacAuscultation
        {
            get { return _cardiacAuscultation; }
            set { _cardiacAuscultation = value; }
        }

        public DropdownValue PulseQuality
        {
            get { return _pulseQuality; }
            set { _pulseQuality = value; }
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

        public DropdownValue RespiratoryAuscultationId
        {
            get { return _respiratoryAuscultation; }
            set { _respiratoryAuscultation = value; }
        }

        public DropdownValue PhysicalStatusClass
        {
            get { return _physicalStatusClass; }
            set { _physicalStatusClass = value; }
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