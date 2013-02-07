using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Patient
    {
        private int _patientId;
        private int _studentId;
        private string _studentFullName;
        private int _clinicianId;
        private string _clinicianFullName;
        private int _temperamentId;
        private string _temperamentName;
        private char _formCompleted;
        private DateTime _dateSeenOn;
        private int _cageOrStallNumber;
        private decimal _bodyWeight;
        private int _ageInYears;
        private int _ageInMonths;
        private ClinicalFindings _clinicalFindings;
        private List<AnesthesiaConcern> _anesthesiaConcerns;

        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public int StudentId
        {
            get { return _studentId; }
            set { _studentId = value; }
        }

        public string StudentFullName
        {
            get { return _studentFullName; }
            set { _studentFullName = value; }
        }

        public int ClinicianId
        {
            get { return _clinicianId; }
            set { _clinicianId = value; }
        }

        public string ClinicianFullName
        {
            get { return _clinicianFullName; }
            set { _clinicianFullName = value; }
        }

        public int TemperamentId
        {
            get { return _temperamentId; }
            set { _temperamentId = value; }
        }

        public string TemperamentName
        {
            get { return _temperamentName; }
            set { _temperamentName = value; }
        }

        public char FormCompleted
        {
            get { return _formCompleted; }
            set { _formCompleted = value; }
        }

        public DateTime DateSeenOn
        {
            get { return _dateSeenOn; }
            set { _dateSeenOn = value; }
        }

        public int CageOrStallNumber
        {
            get { return _cageOrStallNumber; }
            set { _cageOrStallNumber = value; }
        }

        public decimal BodyWeight
        {
            get { return _bodyWeight; }
            set { _bodyWeight = value; }
        }

        public int AgeInYears
        {
            get { return _ageInYears; }
            set { _ageInYears = value; }
        }

        public int AgeInMonths
        {
            get { return _ageInMonths; }
            set { _ageInMonths = value; }
        }

        public ClinicalFindings ClinicalFindings
        {
            get { return _clinicalFindings; }
            set { _clinicalFindings = value; }
        }

        public List<AnesthesiaConcern> AnesthesiaConcerns
        {
            get { return _anesthesiaConcerns; }
            set { _anesthesiaConcerns = value; }
        }

        public Patient()
        {
            _patientId = -1;
            _formCompleted = 'N';
            _anesthesiaConcerns = new List<AnesthesiaConcern>();
        }

        public bool ValidatePatient()
        {
            if (_patientId == 0 || _studentId == 0 || _clinicianId == 0)
                return false;
            else
                return true;
        }
    }
}