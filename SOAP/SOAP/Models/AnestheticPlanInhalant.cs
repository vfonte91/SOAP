using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnestheticPlanInhalant
    {
        private int _id;
        private int _patientId;
        private decimal _percentage;
        private decimal _flowRate;
        private DropdownValue _drug;

        public enum LazyComponents
        {
            LOAD_DRUG_WITH_DETAILS
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

        public decimal Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
        }

        public decimal FlowRate
        {
            get { return _flowRate; }
            set { _flowRate = value; }
        }

        public DropdownValue Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public AnestheticPlanInhalant()
        {
            _id = -1;
            _drug = new DropdownValue();
        }

        public bool HasValues()
        {
            return (_percentage != 0.0M || _drug.Id != -1 || _flowRate != 0.0M);
        }

        public bool ValidateAnestheticPlanInhalant()
        {
            if (_id == 0 || _patientId == 0 || _drug.Id == 0)
                return false;
            else
                return true;
        }
    }
}