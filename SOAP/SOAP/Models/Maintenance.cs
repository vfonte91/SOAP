using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Maintenance
    {
        private MaintenanceInjectionDrug _maintenanceInjectionDrugs;
        private MaintenanceInhalantDrug _maintenanceInhalantDrugs;

        public MaintenanceInjectionDrug MaintenanceInjectionDrugs
        {
            get { return _maintenanceInjectionDrugs; }
            set { _maintenanceInjectionDrugs = value; }
        }

        public MaintenanceInhalantDrug MaintenanceInhalantDrugs
        {
            get { return _maintenanceInhalantDrugs; }
            set { _maintenanceInhalantDrugs = value; }
        }

        public Maintenance()
        {
            _maintenanceInjectionDrugs = new MaintenanceInjectionDrug();
            _maintenanceInhalantDrugs = new MaintenanceInhalantDrug();
        }


    }

}