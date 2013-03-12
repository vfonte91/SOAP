using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class IntraoperativeAnalgesia
    {
        private int _id;
        private int _patientId;
        private DropdownValue _analgesia;

        public enum LazyComponents
        {
            LOAD_ANALGESIA_WITH_DETAILS
        };

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

        public DropdownValue Analgesia
        {
            get { return _analgesia; }
            set { _analgesia = value; }
        }

        public IntraoperativeAnalgesia()
        {
            _id = -1;
        }

        public bool ValidateIntraoperativeAnalgesia()
        {
            if (_id == 0 || _patientId == 0 || _analgesia.Id == 0)
                return false;
            else
                return true;
        }
    }
}