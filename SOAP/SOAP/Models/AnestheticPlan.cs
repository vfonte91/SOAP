using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnestheticPlan
    {
        private List<AnestheticPlanPremedication> _preMedications;
        private AnestheticPlanInjection _injectionPlan;
        private AnestheticPlanInhalant _inhalantPlan;

        public List<AnestheticPlanPremedication> PreMedications
        {
            get { return _preMedications; }
            set { _preMedications = value; }
        }

        public AnestheticPlanInjection InjectionPlan
        {
            get { return _injectionPlan; }
            set { _injectionPlan = value; }
        }

        public AnestheticPlanInhalant InhalantPlan
        {
            get { return _inhalantPlan; }
            set { _inhalantPlan = value; }
        }

        public AnestheticPlan()
        {
            _inhalantPlan = new AnestheticPlanInhalant();
            _injectionPlan = new AnestheticPlanInjection();
            _preMedications = new List<AnestheticPlanPremedication>();
        }
    }
}