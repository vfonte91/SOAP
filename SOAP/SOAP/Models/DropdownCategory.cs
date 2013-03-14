using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class DropdownCategory
    {
        private int _id;
        private string _shortName;
        private string _longName;
        private List<DropdownValue> _dropdownValues;

        public enum LazyComponents
        {
            LOAD_DROPDOWN_VALUES
        };

        public DropdownCategory()
        {
            _id = -1;
            _dropdownValues = new List<DropdownValue>();
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string ShortName
        {
            get { return _shortName; }
            set { _shortName = value; }
        }

        public string LongName
        {
            get { return _longName; }
            set { _longName = value; }
        }

        public bool ValidateDropdownCategory()
        {
            if (_id == 0 || _shortName == null)
                return false;
            else
                return true;
        }

        public List<DropdownValue> DropdownValues
        {
            get { return _dropdownValues; }
            set { _dropdownValues = value; }
        }
    }
}