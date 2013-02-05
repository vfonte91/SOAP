using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AdministrationSet
    {
        private int _id;
        private int _patientId;
        private int _miniDripFlag;
        private int _maxiDripFlag;

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

        public int MiniDripFlag
        {
            get { return _miniDripFlag; }
            set { _miniDripFlag = value; }
        }

        public int MaxiDripFlag
        {
            get { return _maxiDripFlag; }
            set { _maxiDripFlag = value; }
        }

        public AdministrationSet()
        {
            _id = -1;
        }

        public bool ValidateAdministrationSet()
        {
            if (_id == 0 || _patientId == 0)
                return false;
            else
                return true;
        }
    }
}