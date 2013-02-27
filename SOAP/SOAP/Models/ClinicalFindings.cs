using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class ClinicalFindings
    {
        private int _id;
        private int _patientId;
        private decimal _temperature;
        private decimal _pulseRate;
        private decimal _respiratoryRate;
        private DropdownValue _cardiacAuscultation;
        private DropdownValue _pulseQuality;
        private string _mucousMembraneColor;
        private decimal _capillaryRefillTime;
        private DropdownValue _respiratoryAuscultation;
        private DropdownValue _physicalStatusClassification;
        private string _reasonForClassification;
        private List<CurrentMedication> _currentMedications;
        private List<PriorAnesthesia> _priorAnesthesia;
        private List<AnesthesiaConcern> _anesthesiaConcerns;

        public enum LazyComponents
        {
            LOAD_CARDIAC_WITH_DETAILS,
            LOAD_PULSE_QUALITY_WITH_DETAILS,
            LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS,
            LOAD_PHYSICAL_STATUS_WITH_DETAILS
        }

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

        public DropdownValue RespiratoryAuscultation
        {
            get { return _respiratoryAuscultation; }
            set { _respiratoryAuscultation = value; }
        }

        public DropdownValue PhysicalStatusClassification
        {
            get { return _physicalStatusClassification; }
            set { _physicalStatusClassification = value; }
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

        public List<PriorAnesthesia> PriorAnesthesia
        {
            get { return _priorAnesthesia; }
            set { _priorAnesthesia = value; }
        }

        public List<AnesthesiaConcern> AnesthesiaConcerns
        {
            get { return _anesthesiaConcerns; }
            set { _anesthesiaConcerns = value; }
        }

        public ClinicalFindings()
        {
            _id = -1;
            _currentMedications = new List<CurrentMedication>();
            _priorAnesthesia = new List<PriorAnesthesia>();
            _anesthesiaConcerns = new List<AnesthesiaConcern>();
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