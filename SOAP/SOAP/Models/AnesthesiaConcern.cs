using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnesthesiaConcern
    {
        private int _id;
        private int _patientId;
        private int _concernId;
        private string _concern;

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

        public int ConcernId
        {
            get { return _concernId; }
            set { _concernId = value; }
        }

        public string Concern
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
            if (_id == 0 || _patientId == 0 || _concernId == 0)
                return false;
            else
                return true;
        }
    }
}