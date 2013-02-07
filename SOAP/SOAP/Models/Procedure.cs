using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Procedure
    {
        private int _id;
        private int _patientId;
        private int _procedureId;
        private string _procedureName;

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

        public int ProcedureId
        {
            get { return _procedureId; }
            set { _procedureId = value; }
        }

        public string ProcedureName
        {
            get { return _procedureName; }
            set { _procedureName = value; }
        }

        public Procedure()
        {
            _id = -1;
        }

        public bool ValidateProcedure()
        {
            if (_id == 0 || _patientId == 0 || _procedureId == 0)
                return false;
            else
                return true;
        }
    }
}