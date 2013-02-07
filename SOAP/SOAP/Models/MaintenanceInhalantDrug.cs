using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class MaintenanceInhalantDrug
    {
        private int _id;
        private int _patientId;
        private int _drugId;
        private string _drugName;
        private char _inductionReqFlag;
        private decimal _inductionDose;
        private decimal _inductionOxygenFlowRate;
        private char _maintenanceReqFlag;
        private decimal _maintenanceDose;
        private decimal _maintenanceOxygenFlowRate;
        private char _equipmentReqFlag;
        private int _breathingSystemId;
        private string _breathingSystemName;
        private int _breathingBagSizeId;
        private string _breathingBagSizeName;

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

        public int DrugId
        {
            get { return _drugId; }
            set { _drugId = value; }
        }

        public string DrugName
        {
            get { return _drugName; }
            set { _drugName = value; }
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

        public int BreathingSystemId
        {
            get { return _breathingSystemId; }
            set { _breathingSystemId = value; }
        }

        public string BreathingSystemName
        {
            get { return _breathingSystemName; }
            set { _breathingSystemName = value; }
        }

        public int BreathingBagSizeId
        {
            get { return _breathingBagSizeId; }
            set { _breathingBagSizeId = value; }
        }

        public string BreathingBagSizeName
        {
            get { return _breathingBagSizeName; }
            set { _breathingBagSizeName = value; }
        }

        public MaintenanceInhalantDrug()
        {
            _id = -1;
        }

        public bool ValidateMaintenanceInhalantDrug()
        {
            if (_id == 0 || _patientId == 0 || _drugId == 0)
                return false;
            else
                return true;
        }
    }
}