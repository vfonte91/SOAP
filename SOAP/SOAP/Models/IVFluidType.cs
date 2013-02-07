using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class IVFluidType
    {
        private int _id;
        private int _patientId;
        private int _fluidTypeId;
        private string _fluidTypeName;
        private decimal _dose;

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

        public int FluidTypeId
        {
            get { return _fluidTypeId; }
            set { _fluidTypeId = value; }
        }

        public string FluidTypeName
        {
            get { return _fluidTypeName; }
            set { _fluidTypeName = value; }
        }

        public decimal Dose
        {
            get { return _dose; }
            set { _dose = value; }
        }

        public IVFluidType()
        {
            _id = -1;
        }

        public bool ValidateIVFluidType()
        {
            if (_id == 0 || _patientId == 0 || _fluidTypeId == 0)
                return false;
            else
                return true;
        }
    }
}