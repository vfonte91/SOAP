using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Maintenance
    {
        private MaintenanceInjectionDrug _maintenanceInjectionDrug;
        private MaintenanceInhalantDrug _maintenanceInhalantDrug;
        private MaintenanceOther _maintenanceOther;

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

        public MaintenanceOther MaintenanceOther
        {
            get { return _maintenanceOther; }
            set { _maintenanceOther = value; }
        }

        public Maintenance()
        {
            _maintenanceInjectionDrug = new MaintenanceInjectionDrug();
            _maintenanceInhalantDrug = new MaintenanceInhalantDrug();
            _maintenanceOther = new MaintenanceOther();
        }


    }

}