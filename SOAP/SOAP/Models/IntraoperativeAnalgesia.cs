using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class IntraoperativeAnalgesia
    {
        private int _id;
        private int _patientId;
        private int _analgesiaId;
        private string _analgesiaName;

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

        public int AnalgesiaId
        {
            get { return _analgesiaId; }
            set { _analgesiaId = value; }
        }

        public string AnalgesiaName
        {
            get { return _analgesiaName; }
            set { _analgesiaName = value; }
        }

        public IntraoperativeAnalgesia()
        {
            _id = -1;
        }

        public bool ValidateIntraoperativeAnalgesia()
        {
            if (_id == 0 || _patientId == 0 || _analgesiaId == 0)
                return false;
            else
                return true;
        }
    }
}