using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Maintenance
    {
        private MaintenanceInjectionDrug _maintenanceInjectionDrug;
        private MaintenanceInhalantDrug _maintenanceInhalantDrug;

        public MaintenanceInjectionDrug MaintenanceInjectionDrug
        {
            get { return _maintenanceInjectionDrug; }
            set { _maintenanceInjectionDrug = value; }
        }

        public MaintenanceInhalantDrug MaintenanceInhalantDrug
        {
            get { return _maintenanceInhalantDrug; }
            set { _maintenanceInhalantDrug = value; }
        }

        public Maintenance()
        {
            _maintenanceInjectionDrug = new MaintenanceInjectionDrug();
            _maintenanceInhalantDrug = new MaintenanceInhalantDrug();
        }


    }

}