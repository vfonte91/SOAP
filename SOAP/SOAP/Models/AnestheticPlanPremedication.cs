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
        private DropdownValue _route;
        private DropdownValue _sedativedrug;
        private decimal _sedativedosage;
        private DropdownValue _opioiddrug;
        private decimal _opioiddosage;
        private DropdownValue _anticholinergicdrug;
        private decimal _anticholinergicdosage;
        private decimal _ketaminedosage;

        public enum LazyComponents
        {
            LOAD_ROUTE_WITH_DETAILS,
            LOAD_SEDATIVE_DRUG_WITH_DETAILS,
            LOAD_OPIOID_DRUG_WITH_DETAILS,
            LOAD_ANTICHOLINERGIC_DRUG_WITH_DETAILS
        };

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

        public DropdownValue Route
        {
            get { return _route; }
            set { _route = value; }
        }

        public DropdownValue SedativeDrug
        {
            get { return _sedativedrug; }
            set { _sedativedrug = value; }
        }

        public decimal SedativeDosage
        {
            get { return _sedativedosage; }
            set { _sedativedosage = value; }
        }

        public DropdownValue OpioidDrug
        {
            get { return _opioiddrug; }
            set { _opioiddrug = value; }
        }

        public decimal OpioidDosage
        {
            get { return _opioiddosage; }
            set { _opioiddosage = value; }
        }

        public DropdownValue AnticholinergicDrug
        {
            get { return _anticholinergicdrug; }
            set { _anticholinergicdrug = value; }
        }

        public decimal AnticholinergicDosage
        {
            get { return _anticholinergicdosage; }
            set { _anticholinergicdosage = value; }
        }

        public decimal KetamineDosage
        {
            get { return _ketaminedosage; }
            set { _ketaminedosage = value; }
        }

        public AnestheticPlanPremedication()
        {
            _id = -1;
            _route = new DropdownValue();
            _sedativedrug = new DropdownValue();
            _sedativedosage = -1;
            _opioiddrug = new DropdownValue();
            _opioiddosage = -1;
            _anticholinergicdrug = new DropdownValue();
            _anticholinergicdosage = -1;
            _ketaminedosage = -1;
        }

        public bool HasValues()
        {
            return (_route.Id != -1 && _sedativedrug.Id != -1 && _opioiddrug.Id != -1 && _anticholinergicdrug.Id != -1 && _sedativedosage != 0.0M
                         && _opioiddosage != 0.0M && _anticholinergicdosage != 0.0M && _ketaminedosage != 0.0M);
        }

        public bool ValidateAnestheticPlanPremedication()
        {
            if (_id == 0 || _patientId == 0 || _sedativedrug.Id == 0 || _opioiddrug.Id == 0 || _anticholinergicdrug.Id == 0)
                return false;
            else
                return true;
        }
    }
}