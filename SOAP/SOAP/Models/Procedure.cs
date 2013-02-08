using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Procedure
    {
        private int _id;
        private int _patientId;
        private DropdownValue _procedureInformation;

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

        public DropdownValue ProcedureInformation
        {
            get { return _procedureInformation; }
            set { _procedureInformation = value; }
        }

        public Procedure()
        {
            _id = -1;
        }

        public bool ValidateProcedure()
        {
            if (_id == 0 || _patientId == 0 || _procedureInformation.Id == 0)
                return false;
            else
                return true;
        }
    }
}