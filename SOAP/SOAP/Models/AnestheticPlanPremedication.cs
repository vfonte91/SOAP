using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOAP.Models
{
    public class AnestheticPlanPremedication
    {
        private int _id;
        private int _patientId;
        private DropdownValue _drug;
        private DropdownValue _route;
        private decimal _dosage;

        public enum LazyComponents
        {
            LOAD_ROUTE_WITH_DETAILS,
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

        public DropdownValue Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public DropdownValue Route
        {
            get { return _route; }
            set { _route = value; }
        }

        public decimal Dosage
        {
            get { return _dosage; }
            set { _dosage = value; }
        }

        public AnestheticPlanPremedication()
        {
            _id = -1;
            _dosage = -1;
            _drug = new DropdownValue();
            _route = new DropdownValue();
        }

        public bool HasValues()
        {
            return (_route.Id != -1 && _drug.Id != -1 && _dosage != 0.0M);
        }

        public bool ValidateAnestheticPlanPremedication()
        {
            if (_id == 0 || _patientId == 0 || _drug.Id == 0)
                return false;
            else
                return true;
        }
    }
}