using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class MaintenanceInhalantDrug
    {
        private int _id;
        private int _patientId;
        private DropdownValue _drug;
        private char _inductionReqFlag;
        private decimal _inductionDose;
        private decimal _inductionOxygenFlowRate;
        private char _maintenanceReqFlag;
        private decimal _maintenanceDose;
        private decimal _maintenanceOxygenFlowRate;
        private char _equipmentReqFlag;
        private DropdownValue _breathingSystem;
        private DropdownValue _breathingBagSize;

        public enum LazyComponents
        {
            LOAD_DRUG_WITH_DETAILS,
            LOAD_BREATHING_SYSTEM_WITH_DETAILS,
            LOAD_BREATHING_BAG_SIZE_WITH_SETAILS
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

        public DropdownValue Drug
        {
            get { return _drug; }
            set { _drug = value; }
        }

        public char InductionReqFlag
        {
            get { return _inductionReqFlag; }
            set { _inductionReqFlag = value; }
        }

        public decimal InductionDose
        {
            get { return _inductionDose; }
            set { _inductionDose = value; }
        }

        public decimal InductionOxygenFlowRate
        {
            get { return _inductionOxygenFlowRate; }
            set { _inductionOxygenFlowRate = value; }
        }

        public char MaintenanceReqFlag
        {
            get { return _maintenanceReqFlag; }
            set { _maintenanceReqFlag = value; }
        }

        public decimal MaintenanceDose
        {
            get { return _maintenanceDose; }
            set { _maintenanceDose = value; }
        }

        public decimal MaintenanceOxygenFlowRate
        {
            get { return _maintenanceOxygenFlowRate; }
            set { _maintenanceOxygenFlowRate = value; }
        }

        public char EquipmentReqFlag
        {
            get { return _equipmentReqFlag; }
            set { _equipmentReqFlag = value; }
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

        public MaintenanceInhalantDrug()
        {
            _id = -1;
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