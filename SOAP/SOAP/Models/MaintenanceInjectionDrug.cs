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
        private DrugInformation _drug;

        public enum LazyComponents
        {
            LOAD_ROUTE_WITH_DETAILS,
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

        public DrugInformation Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public MaintenanceInjectionDrug()
        {
            _id = -1;
        }

        public bool ValidateMaintenanceInjectionDrug()
        {
            if (_id == 0 || _patientId == 0 || _drug.Drug.Id == 0)
                return false;
            else
                return true;
        }
    }
}