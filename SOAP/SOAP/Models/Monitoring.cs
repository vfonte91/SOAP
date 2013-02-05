using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Monitoring
    {
        private int _id;
        private int _patientId;
        private int _equipmentId;

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

        public int EquipmentId
        {
            get { return _equipmentId; }
            set { _equipmentId = value; }
        }

        public Monitoring()
        {
            _id = -1;
        }

        public bool ValidateMonitoring()
        {
            if (_id == 0 || _patientId == 0 || _equipmentId == 0)
                return false;
            else
                return true;
        }
    }
}