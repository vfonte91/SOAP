using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class DropdownCategory
    {
        private int _id;
        private string _shortName;
        private string _longName;
        private List<DropdownValue> _dropDownValues;

        public enum LazyComponents
        {
            LOAD_DROPDOWN_VALUES
        };

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

        public DropdownCategory()
        {
            _id = -1;
        }

        public bool ValidateDropdownCategory()
        {
            if (_id == 0 || _shortName == null)
                return false;
            else
                return true;
        }

        public List<DropdownValue> DropDownValues
        {
            get { return _dropDownValues; }
            set { _dropDownValues = value; }
        }
    }
}