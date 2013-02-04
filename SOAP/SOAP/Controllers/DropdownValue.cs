using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class DropdownValue
    {
        private int _id;
        private DropdownCategory _category;
        private string _label;
        private char _otherFlag;
        private string _description;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public DropdownCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public char OtherFlag
        {
            get { return _otherFlag; }
            set { _otherFlag = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DropdownValue()
        {
            _id = -1;
        }

        public bool ValidateDropdownValue()
        {
            if (_id == 0 || _label == null || _category == null)
                return false;
            else
                return true;
        }
    }
}