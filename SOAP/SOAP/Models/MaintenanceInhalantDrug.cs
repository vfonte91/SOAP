using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class MaintenanceInhalantDrug
    {
        private int _id;
        private int _patientId;
        private DropdownValue _drug;
        private decimal _inductionPercentage;
        private decimal _inductionOxygenFlowRate;
        private decimal _maintenancePercentage;
        private decimal _maintenanceOxygenFlowRate;
        private DropdownValue _breathingSystem;
        private DropdownValue _breathingBagSize;
        private string _otherAnestheticDrug;
        private DropdownValue _intraoperativeAnalgesia;
        private DropdownValue _iVFluidType;

        public enum LazyComponents
        {
            LOAD_DRUG_WITH_DETAILS,
            LOAD_BREATHING_SYSTEM_WITH_DETAILS,
            LOAD_BREATHING_BAG_SIZE_WITH_DETAILS,
            LOAD_INTRAOP_WITH_DETAILS,
            LOAD_IV_WITH_DETAILS
        };

        public MaintenanceInhalantDrug()
        {
            _id = -1;
            _drug = new DropdownValue();
            _breathingBagSize = new DropdownValue();
            _breathingSystem = new DropdownValue();
            _intraoperativeAnalgesia = new DropdownValue();
            _iVFluidType = new DropdownValue();
        }

        public bool HasValues()
        {
            return (_drug.Id != -1 || _inductionPercentage != 0.0M || _inductionOxygenFlowRate != 0.0M || _maintenanceOxygenFlowRate != 0.0M ||
                    _maintenancePercentage != 0.0M || _breathingBagSize.Id != -1 || _breathingSystem.Id != -1 || _otherAnestheticDrug != null ||
                    _intraoperativeAnalgesia.Id != -1);
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

        public decimal InductionPercentage
        {
            get { return _inductionPercentage; }
            set { _inductionPercentage = value; }
        }

        public decimal InductionOxygenFlowRate
        {
            get { return _inductionOxygenFlowRate; }
            set { _inductionOxygenFlowRate = value; }
        }

        public decimal MaintenancePercentage
        {
            get { return _maintenancePercentage; }
            set { _maintenancePercentage = value; }
        }

        public decimal MaintenanceOxygenFlowRate
        {
            get { return _maintenanceOxygenFlowRate; }
            set { _maintenanceOxygenFlowRate = value; }
        }

        public DropdownValue BreathingSystem
        {
            get { return _breathingSystem; }
            set { _breathingSystem = value; }
        }

        public DropdownValue BreathingBagSize
        {
            get { return _breathingBagSize; }
            set { _breathingBagSize = value; }
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
            if (_id == 0 || _patientId == 0 || _drug.Id == 0)
                return false;
            else
                return true;
        }
    }
}