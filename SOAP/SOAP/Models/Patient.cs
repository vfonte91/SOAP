﻿using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class Patient
    {
        private int _patientId;
        private ASFUser _student;
        private ASFUser _clinician;
        private char _formCompleted;
        private PatientInformation _patientInfo;
        private ClinicalFindings _clinicalFindings;
        private List<Bloodwork> _bloodworkGroup;
        private AnestheticPlan _anestheticPlan;
        private Maintenance _maintenance;
        private List<Monitoring> _monitoring;

        public int PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public ASFUser Student
        {
            get { return _student; }
            set { _student = value; }
        }

        public ASFUser Clinician
        {
            get { return _clinician; }
            set { _clinician = value; }
        }

        public char FormCompleted
        {
            get { return _formCompleted; }
            set { _formCompleted = value; }
        }

        public PatientInformation PatientInfo
        {
            get { return _patientInfo; }
            set { _patientInfo = value; }
        }

        public ClinicalFindings ClinicalFindings
        {
            get { return _clinicalFindings; }
            set { _clinicalFindings = value; }
        }

        public List<Bloodwork> BloodworkGroup
        {
            get { return _bloodworkGroup; }
            set { _bloodworkGroup = value; }
        }

        public AnestheticPlan AnestheticPlan
        {
            get { return _anestheticPlan; }
            set { _anestheticPlan = value; }
        }

        public Maintenance Maintenance
        {
            get { return _maintenance; }
            set { _maintenance = value; }
        }

        public List<Monitoring> Monitoring
        {
            get { return _monitoring; }
            set { _monitoring = value; }
        }

        public Patient()
        {
            _patientId = -1;
            _formCompleted = 'N';
        }

        public bool ValidatePatient()
        {
            if (_patientId == 0 || _student.UserId == 0 || _clinician.UserId == 0)
                return false;
            else
                return true;
        }
    }
}