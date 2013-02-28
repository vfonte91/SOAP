using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Maintenance
    {
        private List<MaintenanceInjectionDrug> _maintenanceInjectionDrugs;
        private List<MaintenanceInhalantDrug> _maintenanceInhalantDrugs;
        private List<IntraoperativeAnalgesia> _intraOperativeAnalgesias;
        private List<OtherAnestheticDrug> _otherAnestheticDrugs;

        public List<MaintenanceInjectionDrug> MaintenanceInjectionDrugs
        {
            get { return _maintenanceInjectionDrugs; }
            set { _maintenanceInjectionDrugs = value; }
        }

        public List<MaintenanceInhalantDrug> MaintenanceInhalantDrugs
        {
            get { return _maintenanceInhalantDrugs; }
            set { _maintenanceInhalantDrugs = value; }
        }

        public List<IntraoperativeAnalgesia> IntraOperativeAnalgesias
        {
            get { return _intraOperativeAnalgesias; }
            set { _intraOperativeAnalgesias = value; }
        }

        public List<OtherAnestheticDrug> OtherAnestheticDrugs
        {
            get { return _otherAnestheticDrugs; }
            set { _otherAnestheticDrugs = value; }
        }

        public Maintenance()
        {
            _maintenanceInjectionDrugs = new List<MaintenanceInjectionDrug>();
            _maintenanceInhalantDrugs = new List<MaintenanceInhalantDrug>();
            _intraOperativeAnalgesias = new List<IntraoperativeAnalgesia>();
            _otherAnestheticDrugs = new List<OtherAnestheticDrug>();
        }


    }

}