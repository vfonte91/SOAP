using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class CurrentMedication
    {
        private int _id;
        private int _patientId;
        private int _medicationId;

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

        public int MedicationId
        {
            get { return _medicationId; }
            set { _medicationId = value; }
        }

        public CurrentMedication()
        {
            _id = -1;
        }

        public bool ValidateCurrentMedication()
        {
            if (_id == 0 || _patientId == 0 || _medicationId == 0)
                return false;
            else
                return true;
        }
    }
}