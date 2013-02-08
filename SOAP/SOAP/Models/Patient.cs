using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Patient
    {
        private int _patientId;
        private ASFUser _student;
        private ASFUser _clinician;
        private DropdownValue _temperament;
        private char _formCompleted;
        private DateTime _dateSeenOn;
        private int _cageOrStallNumber;
        private decimal _bodyWeight;
        private int _ageInYears;
        private int _ageInMonths;
        private ClinicalFindings _clinicalFindings;
        private AnestheticPlan _anestheticPlan;

        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public ASFUser Student
        {
            get { return _student; }
            set { _student = value; }
        }

        public ASFUser Clinician
        {
            get { return _clinician; }
            set { _clinician = value; }
        }

        public DropdownValue Temperament
        {
            get { return _temperament; }
            set { _temperament = value; }
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

        public AnestheticPlan AnestheticPlan
        {
            get { return _anestheticPlan; }
            set { _anestheticPlan = value; }
        }

        public Patient()
        {
            _patientId = -1;
            _formCompleted = 'N';
        }

        public bool ValidatePatient()
        {
            if (_patientId == 0 || _student.UserId == 0 || _clinician.UserId == 0)
                return false;
            else
                return true;
        }
    }
}