using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class IVFluidType
    {
        private int _id;
        private int _patientId;
        private DropdownValue _fluidType;
        private decimal _dose;

        public enum LazyComponents
        {
            LOAD_FLUID_TYPE_WITH_DETAILS
        };

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

        public DropdownValue FluidType
        {
            get { return _fluidType; }
            set { _fluidType = value; }
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
            if (_id == 0 || _patientId == 0 || _fluidType.Id == 0)
                return false;
            else
                return true;
        }
    }
}