using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class PriorAnesthesia
    {
        private int _id;
        private int _patientId;
        private DateTime _dateOfProblem;
        private string _problem;

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

        public DateTime DateOfProblem
        {
            get { return _dateOfProblem; }
            set { _dateOfProblem = value; }
        }

        public string Problem
        {
            get { return _problem; }
            set { _problem = value; }
        }

        public PriorAnesthesia()
        {
            _id = -1;
        }

        public bool ValidatePriorAnesthesia()
        {
            if (_id == 0 || _patientId == 0)
                return false;
            else
                return true;
        }
    }
}