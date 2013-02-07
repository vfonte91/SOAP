using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class DrugInformation
    {
        private int _id;
        private int _drugId;
        private string _drugName;
        private float _doseMinRange;
        private float _doseMaxRange;
        private float _doseMax;
        private string _doseUnits;
        private string _route;
        private float _concentration;
        private string _concentrationUnits;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
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

        public float DoseMinRange
        {
            get { return _doseMinRange; }
            set { _doseMinRange = value; }
        }

        public float DoseMaxRange
        {
            get { return _doseMaxRange; }
            set { _doseMaxRange = value; }
        }

        public float DoseMax
        {
            get { return _doseMax; }
            set { _doseMax = value; }
        }

        public string DoseUnits
        {
            get { return _doseUnits; }
            set { _doseUnits = value; }
        }

        public string Route
        {
            get { return _route; }
            set { _route = value; }
        }

        public float Concentration
        {
            get { return _concentration; }
            set { _concentration = value; }
        }

        public string ConcentrationUnits
        {
            get { return _concentrationUnits; }
            set { _concentrationUnits = value; }
        }

        public DrugInformation()
        {
            _id = -1;
        }
        public bool ValidateDrugInformation()
        {
            if (_id == 0 || _drugId == 0)
                return false;
            else
                return true;
        }
    }
}