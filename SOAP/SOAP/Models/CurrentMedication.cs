﻿using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class CurrentMedication
    {
        private int _id;
        private int _patientId;
        private DropdownValue _medication;

        public enum LazyComponents
        {
            LOAD_CURRENT_MEDICATIONS_WITH_DETAILS
        }

        public CurrentMedication()
        {
            _id = -1;
            _medication = new DropdownValue();
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

        public DropdownValue Medication
        {
            get { return _medication; }
            set { _medication = value; }
        }

        public bool ValidateCurrentMedication()
        {
            if (_id == 0 || _patientId == 0 || _medication.Id == 0)
                return false;
            else
                return true;
        }
    }
}