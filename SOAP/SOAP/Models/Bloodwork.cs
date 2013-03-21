using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Bloodwork
    {
        private int _id;
        private int _patientId;
        private decimal _pCV;
        private decimal _tP;
        private decimal _albumin;
        private decimal _globulin;
        private decimal _wBC;
        private decimal _nA;
        private decimal _k;
        private decimal _cl;
        private decimal _ca;
        private decimal _iCa;
        private decimal _glucose;
        private decimal _aLT;
        private decimal _aLP;
        private decimal _bUN;
        private decimal _cREAT;
        private decimal _uSG;
        private string _otherType;
        private decimal _otherValue;

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

        public decimal PCV
        {
            get { return _pCV; }
            set { _pCV = value; }
        }

        public decimal TP
        {
            get { return _tP; }
            set { _tP = value; }
        }

        public decimal Albumin
        {
            get { return _albumin; }
            set { _albumin = value; }
        }

        public decimal Globulin
        {
            get { return _globulin; }
            set { _globulin = value; }
        }

        public decimal WBC
        {
            get { return _wBC; }
            set { _wBC = value; }
        }

        public decimal NA
        {
            get { return _nA; }
            set { _nA = value; }
        }

        public decimal K
        {
            get { return _k; }
            set { _k = value; }
        }

        public decimal Cl
        {
            get { return _cl; }
            set { _cl = value; }
        }

        public decimal Ca
        {
            get { return _ca; }
            set { _ca = value; }
        }

        public decimal iCa
        {
            get { return _iCa; }
            set { _iCa = value; }
        }

        public decimal Glucose
        {
            get { return _glucose; }
            set { _glucose = value; }
        }

        public decimal ALT
        {
            get { return _aLT; }
            set { _aLT = value; }
        }

        public decimal ALP
        {
            get { return _aLP; }
            set { _aLP = value; }
        }

        public decimal BUN
        {
            get { return _bUN; }
            set { _bUN = value; }
        }

        public decimal CREAT
        {
            get { return _cREAT; }
            set { _cREAT = value; }
        }

        public decimal USG
        {
            get { return _uSG; }
            set { _uSG = value; }
        }

        public string OtherType
        {
            get { return _otherType; }
            set { _otherType = value; }
        }

        public decimal OtherValue
        {
            get { return _otherValue; }
            set { _otherValue = value; }
        }

        public Bloodwork()
        {
            _id = -1;
            _albumin = -1;
            _aLP = -1;
            _aLT = -1;
            _bUN = -1;
            _ca = -1;
            _cl = -1;
            _cREAT = -1;
            _globulin = -1;
            _glucose = -1;
            _iCa = -1;
            _k = -1;
            _nA = -1;
            _otherType = "";
            _otherValue = -1;
            _pCV = -1;
            _tP = -1;
            _uSG = -1;
            _wBC = -1;
        }

        public bool HasValues()
        {
            if (_albumin != -1 || _aLP != -1 || _aLT != -1 || _bUN != -1 || _ca != -1 ||
                _cl != -1 || _cREAT != -1 || _globulin != -1 || _glucose != -1 ||
                _iCa != -1 || _k != -1 || _nA != -1 || _otherValue != -1 || _pCV != -1 ||
                _tP != -1 || _uSG != -1 || _wBC != -1)
                return true;
            else
                return false;
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