using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class OtherAnestheticDrug
    {
        private int _id;
        private int _patientId;
        private int _drugId;

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

        public int DrugId
        {
            get { return _drugId; }
            set { _drugId = value; }
        }

        public OtherAnestheticDrug()
        {
            _id = -1;
        }

        public bool ValidateOtherAnestheticDrug()
        {
            if (_id == 0 || _patientId == 0 || _drugId == 0)
                return false;
            else
                return true;
        }
    }
}