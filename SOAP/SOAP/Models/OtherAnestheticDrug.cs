﻿using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class OtherAnestheticDrug
    {
        private int _id;
        private int _patientId;
        private DropdownValue _drug;

        public enum LazyComponents
        {
            LOAD_DRUG_WITH_DETAIL
        };

        public OtherAnestheticDrug()
        {
            _id = -1;
            _drug = new DropdownValue();
        }

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

        public DropdownValue Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public bool ValidateOtherAnestheticDrug()
        {
            if (_id == 0 || _patientId == 0 || _drug.Id == 0)
                return false;
            else
                return true;
        }
    }
}