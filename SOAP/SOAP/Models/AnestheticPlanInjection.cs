﻿using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class AnestheticPlanInjection
    {
        private int _id;
        private int _patientId;
        private DropdownValue _route;
        private decimal _dosage;
        private decimal _dose;
        private DrugInformation _drug;

        public enum LazyComponents {
            LOAD_ROUTE_WITH_DETAILS,
            LOAD_DRUG_INFORMATION
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

        public decimal Dosage
        {
            get { return _dosage; }
            set { _dosage = value; }
        }

        public decimal Dose
        {
            get { return _dose; }
            set { _dose = value; }
        }

        public DrugInformation Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public AnestheticPlanInjection()
        {
            _id = -1;
            _route = new DropdownValue();
            _drug = new DrugInformation();
        }

        public bool ValidateAnestheticPlanInjection()
        {
            if (_id == 0 || _patientId == 0 || _drug.Id == 0)
                return false;
            else
                return true;
        }
    }
}