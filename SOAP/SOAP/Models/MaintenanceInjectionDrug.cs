using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class MaintenanceInjectionDrug
    {
        private int _id;
        private int _patientId;
        private int _drugId;
        private string _drugName;
        private int _routeOfAdministrationId;
        private string _routeOfAdministrationName;
        private decimal _dosage;
        private decimal _dose;
        private DrugInformation _drug;

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

        public int DrugId
        {
            get { return _drugId; }
            set { _drugId = value; }
        }

        public string DrugName
        {
            get { return _drugName; }
            set { _drugName = value; }
        }

        public int RouteOfAdministrationId
        {
            get { return _routeOfAdministrationId; }
            set { _routeOfAdministrationId = value; }
        }

        public string RouteOfAdministrationName
        {
            get { return _routeOfAdministrationName; }
            set { _routeOfAdministrationName = value; }
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
            if (_id == 0 || _patientId == 0 || _drugId == 0)
                return false;
            else
                return true;
        }
    }
}