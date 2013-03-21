﻿using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Bloodwork
    {
        private int _id;
        private int _patientId;
        private string _bloodworkInfo;
        private decimal _value;

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

        public string BloodworkName
        {
            get { return _bloodworkInfo; }
            set { _bloodworkInfo = value; }
        }

        public decimal Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Bloodwork()
        {
            _id = -1;
        }

        public bool ValidateBloodwork()
        {
            if (_id == 0 || _patientId == 0)
                return false;
            else
                return true;
        }
    }
}