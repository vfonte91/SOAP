using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnesthesiaConcern
    {
        private int _id;
        private int _patientId;
        private DropdownValue _concern;

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

        public DropdownValue Concern
        {
            get { return _concern; }
            set { _concern = value; }
        }

        public AnesthesiaConcern()
        {
            _id = -1;
        }

        public bool ValidateAnesthesiaConcerns()
        {
            if (_id == 0 || _patientId == 0 || _concern.Id == 0)
                return false;
            else
                return true;
        }
    }
}