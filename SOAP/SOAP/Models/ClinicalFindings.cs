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
        private DropdownValue _mucousMembraneColor;
        private DropdownValue _capillaryRefillTime;
        private DropdownValue _respiratoryAuscultation;
        private DropdownValue _physicalStatusClassification;
        private string _reasonForClassification;
        private string _currentMedications;
        private PriorAnesthesia _priorAnesthesia;
        private List<AnesthesiaConcern> _anesthesiaConcerns;

        public enum LazyComponents
        {
            LOAD_CARDIAC_WITH_DETAILS,
            LOAD_PULSE_QUALITY_WITH_DETAILS,
            LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS,
            LOAD_PHYSICAL_STATUS_WITH_DETAILS,
            LOAD_MUCOUS_MEMBRANE_WITH_DETAILS,
            LOAD_CAP_REFILL_WITH_DETAILS
        }

        public ClinicalFindings()
        {
            _id = -1;
            _anesthesiaConcerns = new List<AnesthesiaConcern>();
            _priorAnesthesia = new PriorAnesthesia();
            _cardiacAuscultation = new DropdownValue();
            _pulseQuality = new DropdownValue();
            _respiratoryAuscultation = new DropdownValue();
            _physicalStatusClassification = new DropdownValue();
            _mucousMembraneColor = new DropdownValue();
            _capillaryRefillTime = new DropdownValue();
        }

        public bool ContainsValue()
        {
            if (_temperature != 0 || _pulseRate != 0 || _respiratoryRate != 0 || _cardiacAuscultation.Id != -1 ||
                _pulseQuality.Id != -1 || _mucousMembraneColor.Id != -1 || _capillaryRefillTime.Id != -1 || _respiratoryAuscultation.Id != -1 ||
                _physicalStatusClassification.Id != -1 || _reasonForClassification != null || _priorAnesthesia.DateOfProblem != DateTime.MinValue ||
                _priorAnesthesia.Problem != null || _anesthesiaConcerns.Count > 0)
                return true;
            else
                return false;
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

        public DropdownValue MucousMembraneColor
        {
            get { return _mucousMembraneColor; }
            set { _mucousMembraneColor = value; }
        }

        public DropdownValue CapillaryRefillTime
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

        public string CurrentMedications
        {
            get { return _currentMedications; }
            set { _currentMedications = value; }
        }

        public PriorAnesthesia PriorAnesthesia
        {
            get { return _priorAnesthesia; }
            set { _priorAnesthesia = value; }
        }

        public List<AnesthesiaConcern> AnesthesiaConcerns
        {
            get { return _anesthesiaConcerns; }
            set { _anesthesiaConcerns = value; }
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