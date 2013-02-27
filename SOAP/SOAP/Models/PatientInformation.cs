using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class PatientInformation
    {
        private int _patientId;
        private Procedure _procedure;
        private decimal _bodyWeight;
        private DropdownValue _temperament;
        private int _ageInYears;
        private int _ageInMonths;
        private DateTime _dateSeenOn;
        private int _cageOrStallNumber;
        private DropdownValue _preOperationPainAssessment;
        private DropdownValue _postOperationPainAssessment;

        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public Procedure Procedure
        {
            get { return _procedure; }
            set { _procedure = value; }
        }

        public decimal BodyWeight
        {
            get { return _bodyWeight; }
            set { _bodyWeight = value; }
        }

        public DropdownValue Temperament
        {
            get { return _temperament; }
            set { _temperament = value; }
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


    }

}