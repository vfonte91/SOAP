using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Monitoring
    {
        private int _id;
        private int _patientId;
        private DropdownValue _equipment;

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

        public DropdownValue Equipment
        {
            get { return _equipment; }
            set { _equipment = value; }
        }

        public Monitoring()
        {
            _id = -1;
        }

        public bool ValidateMonitoring()
        {
            if (_id == 0 || _patientId == 0 ||_equipment.Id == 0)
                return false;
            else
                return true;
        }
    }
}