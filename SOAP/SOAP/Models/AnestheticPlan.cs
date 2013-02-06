using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnestheticPlan
    {
        private List<AnestheticPlanPremedication> _preMedications;
        private List<AnestheticPlanInjection> _injectionPlans;
        private List<AnestheticPlanInhalant> _inhalantPlans;
        private List<MaintenanceInjectionDrug> _maintenanceInjectionDrugs;
        private List<MaintenanceInhalantDrug> _maintenanceInhalantDrugs;
        private List<IntraoperativeAnalgesia> _intraOperativeAnalgesias;
        private List<OtherAnestheticDrug> _otherAnestheticDrugs;
        private List<Monitoring> _monitoring;
        private List<IVFluidType> _iVFluidTypes;
        private List<AdministrationSet> _administrationSet;

        public List<AnestheticPlanPremedication> PreMedications
        {
            get { return _preMedications; }
            set { _preMedications = value; }
        }

        public List<AnestheticPlanInjection> InjectionPlans
        {
            get { return _injectionPlans; }
            set { _injectionPlans = value; }
        }

        public List<AnestheticPlanInhalant> InhalantPlans
        {
            get { return _inhalantPlans; }
            set { _inhalantPlans = value; }
        }

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

        public List<Monitoring> Monitoring
        {
            get { return _monitoring; }
            set { _monitoring = value; }
        }

        public List<IVFluidType> IVFluidTypes
        {
            get { return _iVFluidTypes; }
            set { _iVFluidTypes = value; }
        }

        public List<AdministrationSet> AdministrationSet
        {
            get { return _administrationSet; }
            set { _administrationSet = value; }
        }

        public AnestheticPlan()
        {
            _administrationSet = new List<AdministrationSet>();
            _inhalantPlans = new List<AnestheticPlanInhalant>();
            _injectionPlans = new List<AnestheticPlanInjection>();
            _intraOperativeAnalgesias = new List<IntraoperativeAnalgesia>();
            _iVFluidTypes = new List<IVFluidType>();
            _maintenanceInhalantDrugs = new List<MaintenanceInhalantDrug>();
            _maintenanceInjectionDrugs = new List<MaintenanceInjectionDrug>();
            _monitoring = new List<Monitoring>();
            _otherAnestheticDrugs = new List<OtherAnestheticDrug>();
            _preMedications = new List<AnestheticPlanPremedication>();
        }
    }
}