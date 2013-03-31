using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class MaintenanceOther
    {
        private int _id;
        private int _patientId;
        private string _otherAnestheticDrug;
        private DropdownValue _intraoperativeAnalgesia;
        private DropdownValue _iVFluidType;

        public enum LazyComponents
        {
            LOAD_INTRAOP_WITH_DETAILS,
            LOAD_IV_WITH_DETAILS
        };

        public MaintenanceOther()
        {
            _id = -1;
            _intraoperativeAnalgesia = new DropdownValue();
            _iVFluidType = new DropdownValue();
        }

        public bool HasValues()
        {
            return (_otherAnestheticDrug != null || _intraoperativeAnalgesia.Id != -1 || _iVFluidType.Id != -1);
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

        public string OtherAnestheticDrug
        {
            get { return _otherAnestheticDrug; }
            set { _otherAnestheticDrug = value; }
        }

        public DropdownValue IntraoperativeAnalgesia
        {
            get { return _intraoperativeAnalgesia; }
            set { _intraoperativeAnalgesia = value; }
        }

        public DropdownValue IVFluidType
        {
            get { return _iVFluidType; }
            set { _iVFluidType = value; }
        }

        public bool ValidateMaintenanceInhalantDrug()
        {
            if (_id == 0 || _patientId == 0)
                return false;
            else
                return true;
        }

    }
}
