using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnestheticPlan
    {
        private List<AnestheticPlanPremedication> _preMedications;
        private List<AnestheticPlanInjection> _injectionPlans;
        private List<AnestheticPlanInhalant> _inhalantPlans;

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

        public AnestheticPlan()
        {
            _inhalantPlans = new List<AnestheticPlanInhalant>();
            _injectionPlans = new List<AnestheticPlanInjection>();
            _preMedications = new List<AnestheticPlanPremedication>();
        }
    }
}