using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class MaintenanceInjectionDrug
    {
        private int _id;
        private int _patientId;
        private DropdownValue _routeOfAdministration;
        private decimal _dosage;
        private decimal _dose;
        private DropdownValue _drug;
        private Boolean _checked;

        public enum LazyComponents
        {
            LOAD_ROUTE_WITH_DETAILS,
            LOAD_DRUG_INFORMATION
        };

        public MaintenanceInjectionDrug()
        {
            _id = -1;
            _routeOfAdministration = new DropdownValue();
            _drug = new DropdownValue();
            _checked = false;
        }

        public bool HasValues()
        {
            return (_routeOfAdministration.Id != -1 || _dosage != 0.0M || _dose != 0.0M || _drug.Id != -1);
        }

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

        public DropdownValue RouteOfAdministration
        {
            get { return _routeOfAdministration; }
            set { _routeOfAdministration = value; }
        }

        public decimal Dosage
        {
            get { return _dosage; }
            set { _dosage = value; }
        }

        public decimal Dose
        {
            get { return _dose; }
            set { _dose = value; }
        }

        public DropdownValue Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public Boolean Cheked
        {
            get { return _checked; }
            set { _checked = true; }
        }

        public bool ValidateMaintenanceInjectionDrug()
        {
            if (_id == -1 || _patientId == -1 || _drug.Id == -1)
                return false;
            else
                return true;
        }
    }
}