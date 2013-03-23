using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class PatientInformation
    {
        private int _patientId;
        private ASFUser _student;
        private ASFUser _clinician;
        private char _formCompleted;
        private Procedure _procedure;
        private decimal _bodyWeight;
        private DropdownValue _temperament;
        private int _ageInYears;
        private int _ageInMonths;
        private DateTime _dateSeenOn;
        private int _cageOrStallNumber;
        private DropdownValue _preOperationPainAssessment;
        private DropdownValue _postOperationPainAssessment;
        private DateTime _procedureDate;

        public enum LazyComponents
        {
            LOAD_STUDENT_DETAIL,
            LOAD_CLINICIAN_DETAIL,
            LOAD_TEMPERAMENT_DETAIL,
            LOAD_PREOP_PAIN_DETAIL,
            LOAD_POSTOP_PAIN_DETAIL
        };

        public PatientInformation()
        {
            _student = new ASFUser();
            _clinician = new ASFUser();
            _procedure = new Procedure();
            _temperament = new DropdownValue();
            _preOperationPainAssessment = new DropdownValue();
            _postOperationPainAssessment = new DropdownValue();
            _ageInMonths = -1;
            _ageInYears = -1;
            _cageOrStallNumber = -1;
            _dateSeenOn = DateTime.Now;
        }

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

        public char FormCompleted
        {
            get { return _formCompleted; }
            set { _formCompleted = value; }
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

        public DateTime ProcedureDate
        {
            get { return _procedureDate; }
            set { _procedureDate = value; }
        }


    }

}