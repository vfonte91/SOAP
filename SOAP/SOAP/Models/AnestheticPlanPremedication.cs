using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOAP.Models
{
    public class AnestheticPlanPremedication
    {
        private int _id;
        private int _patientId;
        private int _drugId;
        private string _drugName;
        private int _routeId;
        private string _route;
        private decimal _dosage;

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

        public int RouteId
        {
            get { return _routeId; }
            set { _routeId = value; }
        }

        public string Route
        {
            get { return _route; }
            set { _route = value; }
        }

        public decimal Dosage
        {
            get { return _dosage; }
            set { _dosage = value; }
        }

        public AnestheticPlanPremedication()
        {
            _id = -1;
        }

        public bool ValidateAnestheticPlanPremedication()
        {
            if (_id == 0 || _patientId == 0 || _drugId == 0)
                return false;
            else
                return true;
        }
    }
}