using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnestheticPlanInhalant
    {
        private int _id;
        private int _patientId;
        private decimal _dose;
        private decimal _flowRate;
        private DrugInformation _drug;

        public enum LazyComponents
        {
            LOAD_DRUG_INFORMATION
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

        public decimal Dose
        {
            get { return _dose; }
            set { _dose = value; }
        }

        public decimal FlowRate
        {
            get { return _flowRate; }
            set { _flowRate = value; }
        }

        public DrugInformation Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public AnestheticPlanInhalant()
        {
            _id = -1;
            _drug = new DrugInformation();
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