﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using SOAP.Models;
using SOAP.Models.Callbacks;

namespace SOAP.Controllers
{
    public class Service
    {
        private ServiceAdo service = new ServiceAdo();

        #region LOGIN

        public ASFUser DoLogin(MembershipInfo user)
        {
            return service.DoLogin(user);
        }

        public ASFUser GetUser(MembershipInfo user)
        {
            return service.GetUser(user);
        }

        public bool ChangePassword(ASFUser user, string oldpassword)
        {
            if (service.UpdateMembershipPassword(user.Member, oldpassword))
                return true;
            else
                return false;
        }

        #endregion

        #region GET
        public Patient GetPatient(int patientId)
        {
            Patient pat = new Patient();
            pat.PatientId = patientId;
            pat.PatientInfo = GetPatientInformation(patientId);
            pat.ClinicalFindings = GetClinicalFindings(patientId);
            pat.BloodworkGroup = GetBloodwork(patientId);
            pat.Maintenance = GetMaintenance(patientId);
            pat.Monitoring = GetMonitoring(patientId);
            return pat;
        }

        public PatientInformation GetPatientInformation(int patientId)
        {
            PatientInformation.LazyComponents[] list =
            {
                PatientInformation.LazyComponents.LOAD_CLINICIAN_DETAIL,
                PatientInformation.LazyComponents.LOAD_STUDENT_DETAIL,
                PatientInformation.LazyComponents.LOAD_TEMPERAMENT_DETAIL,
                PatientInformation.LazyComponents.LOAD_POSTOP_PAIN_DETAIL,
                PatientInformation.LazyComponents.LOAD_PREOP_PAIN_DETAIL
            };
            PatientInformation patInfo = service.GetPatientInformation(patientId, list);
            patInfo.Procedure = service.GetProcedure(patientId, Procedure.LazyComponents.LOAD_PROCEDURE_WITH_DETAIL);
            return patInfo;
        }

        public ClinicalFindings GetClinicalFindings(int patientId)
        {
            ClinicalFindings.LazyComponents[] list = 
            {
                ClinicalFindings.LazyComponents.LOAD_CARDIAC_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_PHYSICAL_STATUS_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_PULSE_QUALITY_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS
            };
            ClinicalFindings clinicalFinding = service.GetClinicalFindings(patientId, list);
            clinicalFinding.CurrentMedications = GetCurrentMedications(patientId);
            clinicalFinding.PriorAnesthesia = GetPriorAnesthesia(patientId);
            clinicalFinding.AnesthesiaConcerns = GetAnesthesiaConcerns(patientId);
            return clinicalFinding;
        }

        public AnestheticPlan GetAnestheticPlan(int patientId)
        {
            AnestheticPlan anesPlan = new AnestheticPlan();
            anesPlan.PreMedications = GetAnestheticPreMedications(patientId);
            anesPlan.InjectionPlans = GetAnestheticPlanInjection(patientId);
            anesPlan.InhalantPlans = GetAnestheticPlanInhalant(patientId);
            return anesPlan;
        }

        public Maintenance GetMaintenance(int patientId)
        {
            Maintenance maint = new Maintenance();
            maint.MaintenanceInjectionDrugs = GetMaintenanceInjectionDrugs(patientId);
            maint.MaintenanceInhalantDrugs = GetMaintenanceInhalantDrugs(patientId);
            maint.IntraOperativeAnalgesias = GetIntraOperativeAnalgesia(patientId);
            maint.OtherAnestheticDrugs = GetOtherAnestheticDrugs(patientId);
            return maint;
        }

        public List<Monitoring> GetMonitoring(int patientId)
        {
            return service.GetMonitoring(patientId, Monitoring.LazyComponents.LOAD_EQUIPMENT_WITH_DETAIL);
        }

        public List<CurrentMedication> GetCurrentMedications(int patientId)
        {
            return service.GetCurrentMedications(patientId, CurrentMedication.LazyComponents.LOAD_CURRENT_MEDICATIONS_WITH_DETAILS);
        }

        public List<PriorAnesthesia> GetPriorAnesthesia(int patientId)
        {
            return service.GetPriorAnesthesia(patientId);
        }

        public List<AnesthesiaConcern> GetAnesthesiaConcerns(int patientId)
        {
            return service.GetAnesthesiaConcerns(patientId, AnesthesiaConcern.LazyComponents.LOAD_CONCERN_WITH_DETAILS);
        }

        public List<Bloodwork> GetBloodwork(int patientId)
        {
            return service.GetBloodwork(patientId, Bloodwork.LazyComponents.LOAD_BLOODWORK_INFO);
        }

        public List<AnestheticPlanPremedication> GetAnestheticPreMedications(int patientId)
        {
            AnestheticPlanPremedication.LazyComponents[] list = 
            {
                AnestheticPlanPremedication.LazyComponents.LOAD_DRUG_WITH_DETAILS,
                AnestheticPlanPremedication.LazyComponents.LOAD_ROUTE_WITH_DETAILS
            };
            return service.GetAnestheticPlanPremedication(patientId, list);
        }

        public List<AnestheticPlanInjection> GetAnestheticPlanInjection(int patientId)
        {
            AnestheticPlanInjection.LazyComponents[] list = 
            {
                AnestheticPlanInjection.LazyComponents.LOAD_DRUG_INFORMATION,
                AnestheticPlanInjection.LazyComponents.LOAD_ROUTE_WITH_DETAILS
            };
            return service.GetAnestheticPlanInjection(patientId, list);
        }

        public List<AnestheticPlanInhalant> GetAnestheticPlanInhalant(int patientId)
        {
            return service.GetAnestheticPlanInhalant(patientId, AnestheticPlanInhalant.LazyComponents.LOAD_DRUG_INFORMATION);
        }

        public List<MaintenanceInjectionDrug> GetMaintenanceInjectionDrugs(int patientId)
        {
            MaintenanceInjectionDrug.LazyComponents[] list = 
            {
                MaintenanceInjectionDrug.LazyComponents.LOAD_DRUG_INFORMATION,
                MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS
            };
            return service.GetMaintenanceInjectionDrugs(patientId, list);
        }

        public List<MaintenanceInhalantDrug> GetMaintenanceInhalantDrugs(int patientId)
        {
            MaintenanceInhalantDrug.LazyComponents[] list = 
            {
                MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_SETAILS,
                MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS,
                MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS
            };
            return service.GetMaintenanceInhalantDrugs(patientId, list);
        }

        public List<IntraoperativeAnalgesia> GetIntraOperativeAnalgesia(int patientId)
        {
            return service.GetIntraoperativeAnalgesia(patientId, IntraoperativeAnalgesia.LazyComponents.LOAD_ANALGESIA_WITH_DETAILS);
        }

        public List<OtherAnestheticDrug> GetOtherAnestheticDrugs(int patientId)
        {
            return service.GetOtherAnestheticDrugs(patientId, OtherAnestheticDrug.LazyComponents.LOAD_DRUG_WITH_DETAIL);
        }
        #endregion

        #region CREATE

        public void CreatePatient(Patient pat)
        {
            service.CreatePatient(pat);

            if (pat.ClinicalFindings != null)
                CreateClinicalFindings(pat.ClinicalFindings);

            if (pat.BloodworkGroup != null)
                CreateBloodwork(pat.BloodworkGroup);

            if (pat.AnestheticPlan != null)
                CreateAnestheticPlan(pat.AnestheticPlan);

            if (pat.Maintenance != null)
                CreateMaintenance(pat.Maintenance);

            if (pat.Monitoring != null)
                CreateMonitoring(pat.Monitoring);
        }

        public void CreateClinicalFindings(ClinicalFindings clinicalFindings)
        {
            service.CreateClinicalFinding(clinicalFindings);

            if (clinicalFindings.CurrentMedications != null)
                CreateCurrentMedications(clinicalFindings.CurrentMedications);

            if (clinicalFindings.PriorAnesthesia != null)
                CreatePriorAnesthesia(clinicalFindings.PriorAnesthesia);

            if (clinicalFindings.AnesthesiaConcerns != null)
                CreateAnesthesiaConcerns(clinicalFindings.AnesthesiaConcerns);
        }

        public void CreateCurrentMedications(List<CurrentMedication> meds)
        {
            foreach (CurrentMedication c in meds)
            {
                service.CreateCurrentMedication(c);
            }
        }

        public void CreatePriorAnesthesia(List<PriorAnesthesia> priors)
        {
            foreach (PriorAnesthesia p in priors)
            {
                service.CreatePriorAnesthesia(p);
            }
        }

        public void CreateAnesthesiaConcerns(List<AnesthesiaConcern> concerns)
        {
            foreach (AnesthesiaConcern a in concerns)
            {
                service.CreateAnesthesiaConcern(a);
            }
        }

        public void CreateBloodwork(List<Bloodwork> blood)
        {
            foreach (Bloodwork b in blood)
            {
                service.CreateBloodwork(b);
            }
        }

        public void CreateAnestheticPlan(AnestheticPlan a)
        {
            if (a.InhalantPlans != null)
                CreateAnestheticInhalantPlans(a.InhalantPlans);

            if (a.InjectionPlans != null)
                CreateAnestheticInjectionPlans(a.InjectionPlans);

            if (a.PreMedications != null)
                CreateAnestheticPremedications(a.PreMedications);
        }

        public void CreateAnestheticInhalantPlans(List<AnestheticPlanInhalant> inhalants)
        {
            foreach (AnestheticPlanInhalant a in inhalants)
            {
                service.CreateAnestheticPlanInhalant(a);
            }
        }

        public void CreateAnestheticInjectionPlans(List<AnestheticPlanInjection> injects)
        {
            foreach (AnestheticPlanInjection a in injects)
            {
                service.CreateAnestheticPlanInjection(a);
            }
        }

        public void CreateAnestheticPremedications(List<AnestheticPlanPremedication> premeds)
        {
            foreach (AnestheticPlanPremedication a in premeds)
            {
                service.CreateAnestheticPlanPremedication(a);
            }
        }

        public void CreateMaintenance(Maintenance m)
        {
            if (m.IntraOperativeAnalgesias != null)
                CreateIntraOperativeAnalgesias(m.IntraOperativeAnalgesias);

            if (m.MaintenanceInhalantDrugs != null)
                CreateMaintenanceInhalantDrugs(m.MaintenanceInhalantDrugs);

            if (m.MaintenanceInjectionDrugs != null)
                CreateMaintenanceInjectionDrugs(m.MaintenanceInjectionDrugs);

            if (m.OtherAnestheticDrugs != null)
                CreateOtherAnestheticDrugs(m.OtherAnestheticDrugs);

        }

        public void CreateIntraOperativeAnalgesias(List<IntraoperativeAnalgesia> intras)
        {
            foreach (IntraoperativeAnalgesia b in intras)
            {
                service.CreateIntraoperativeAnalgesia(b);
            }
        }

        public void CreateMaintenanceInhalantDrugs(List<MaintenanceInhalantDrug> drugs)
        {
            foreach (MaintenanceInhalantDrug a in drugs)
            {
                service.CreateMaintenanceInhalantDrug(a);
            }
        }

        public void CreateMaintenanceInjectionDrugs(List<MaintenanceInjectionDrug> drugs)
        {
            foreach (MaintenanceInjectionDrug a in drugs)
            {
                service.CreateMaintenanceInjectionDrug(a);
            }
        }

        public void CreateOtherAnestheticDrugs(List<OtherAnestheticDrug> drugs)
        {
            foreach (OtherAnestheticDrug o in drugs)
            {
                service.CreateOtherAnestheticDrug(o);
            }
        }

        public void CreateMonitoring(List<Monitoring> monitors)
        {
            foreach (Monitoring m in monitors)
            {
                service.CreateMonitoring(m);
            }
        }

        public bool CreateASFUser(ASFUser user)
        {
            int id = service.CreateMembership(user.Member);
            user.UserId = id;
            return service.CreateASFUser(user);
        }

        #endregion

        #region SAVE

        public void SavePatient(Patient pat)
        {
            service.UpdatePatient(pat);

            if (pat.ClinicalFindings != null)
                SaveClinicalFindings(pat.ClinicalFindings);

            if (pat.BloodworkGroup != null)
                SaveBloodwork(pat.BloodworkGroup);

            if (pat.AnestheticPlan != null)
                SaveAnestheticPlan(pat.AnestheticPlan);

            if (pat.Maintenance != null)
                SaveMaintenance(pat.Maintenance);

            if (pat.Monitoring != null)
                SaveMonitoring(pat.Monitoring);
        }

        public void SaveClinicalFindings(ClinicalFindings clinicalFindings)
        {
            service.UpdateClinicalFinding(clinicalFindings);

            if (clinicalFindings.CurrentMedications != null)
                SaveCurrentMedications(clinicalFindings.CurrentMedications);

            if (clinicalFindings.PriorAnesthesia != null)
                SavePriorAnesthesia(clinicalFindings.PriorAnesthesia);

            if (clinicalFindings.AnesthesiaConcerns != null)
                SaveAnesthesiaConcerns(clinicalFindings.AnesthesiaConcerns);
        }

        public void SaveCurrentMedications(List<CurrentMedication> meds)
        {
            foreach (CurrentMedication c in meds)
            {
                service.UpdateCurrentMedication(c);
            }
        }

        public void SavePriorAnesthesia(List<PriorAnesthesia> priors)
        {
            foreach (PriorAnesthesia p in priors)
            {
                service.UpdatePriorAnesthesia(p);
            }
        }

        public void SaveAnesthesiaConcerns(List<AnesthesiaConcern> concerns)
        {
            foreach (AnesthesiaConcern a in concerns)
            {
                service.UpdateAnesthesiaConcern(a);
            }
        }

        public void SaveBloodwork(List<Bloodwork> blood)
        {
            foreach (Bloodwork b in blood)
            {
                service.UpdateBloodwork(b);
            }
        }

        public void SaveAnestheticPlan(AnestheticPlan a)
        {
            if (a.InhalantPlans != null)
                SaveAnestheticInhalantPlans(a.InhalantPlans);

            if (a.InjectionPlans != null)
                SaveAnestheticInjectionPlans(a.InjectionPlans);

            if (a.PreMedications != null)
                SaveAnestheticPremedications(a.PreMedications);
        }

        public void SaveAnestheticInhalantPlans(List<AnestheticPlanInhalant> inhalants)
        {
            foreach (AnestheticPlanInhalant a in inhalants)
            {
                service.UpdateAnestheticPlanInhalant(a);
            }
        }

        public void SaveAnestheticInjectionPlans(List<AnestheticPlanInjection> injects)
        {
            foreach (AnestheticPlanInjection a in injects)
            {
                service.UpdateAnestheticPlanInjection(a);
            }
        }

        public void SaveAnestheticPremedications(List<AnestheticPlanPremedication> premeds)
        {
            foreach (AnestheticPlanPremedication a in premeds)
            {
                service.UpdateAnestheticPlanPremedication(a);
            }
        }

        public void SaveMaintenance(Maintenance m)
        {
            if (m.IntraOperativeAnalgesias != null)
                SaveIntraOperativeAnalgesias(m.IntraOperativeAnalgesias);

            if (m.MaintenanceInhalantDrugs != null)
                SaveMaintenanceInhalantDrugs(m.MaintenanceInhalantDrugs);

            if (m.MaintenanceInjectionDrugs != null)
                SaveMaintenanceInjectionDrugs(m.MaintenanceInjectionDrugs);

            if (m.OtherAnestheticDrugs != null)
                SaveOtherAnestheticDrugs(m.OtherAnestheticDrugs);

        }

        public void SaveIntraOperativeAnalgesias(List<IntraoperativeAnalgesia> intras)
        {
            foreach (IntraoperativeAnalgesia b in intras)
            {
                service.UpdateIntraoperativeAnalgesia(b);
            }
        }

        public void SaveMaintenanceInhalantDrugs(List<MaintenanceInhalantDrug> drugs)
        {
            foreach (MaintenanceInhalantDrug a in drugs)
            {
                service.UpdateMaintenanceInhalantDrug(a);
            }
        }

        public void SaveMaintenanceInjectionDrugs(List<MaintenanceInjectionDrug> drugs)
        {
            foreach (MaintenanceInjectionDrug a in drugs)
            {
                service.UpdateMaintenanceInjectionDrug(a);
            }
        }

        public void SaveOtherAnestheticDrugs(List<OtherAnestheticDrug> drugs)
        {
            foreach (OtherAnestheticDrug o in drugs)
            {
                service.UpdateOtherAnestheticDrug(o);
            }
        }

        public void SaveMonitoring(List<Monitoring> monitors)
        {
            foreach (Monitoring m in monitors)
            {
                service.UpdateMonitoring(m);
            }
        }

        public void SaveASFUser(ASFUser user)
        {
            service.UpdateASFUser(user);
        }

        public void Promote(ASFUser user)
        {
            service.Promote(user);
        }

        public void Demote(ASFUser user)
        {
            service.Demote(user);
        }

        #endregion

        #region DELETE

        public void DeleteASFUser(ASFUser user)
        {
            service.DeleteASPNetMembership(user.Member);
            service.DeleteASFUser(user);
        }

        public void DeletePatient(Patient pat)
        {
            DeleteMonitoring(pat.Monitoring);
            DeleteMaintenance(pat.Maintenance);
            DeleteAnestheticPlan(pat.AnestheticPlan);
            DeleteBloodworkGroup(pat.BloodworkGroup);
            DeleteClinicalFindings(pat.ClinicalFindings);
            service.DeletePatient(pat);
        }

        public void DeleteMonitoring(List<Monitoring> monitor)
        {
            foreach (Monitoring m in monitor)
            {
                service.DeleteMonitoring(m);
            }
        }

        public void DeleteMaintenance(Maintenance maint)
        {
            DeleteMaintenanceInjectionDrugs(maint.MaintenanceInjectionDrugs);
            DeleteMaintenanceInhalantDrugs(maint.MaintenanceInhalantDrugs);
            DeleteIntraoperativeAnalgesia(maint.IntraOperativeAnalgesias);
            DeleteOtherAnestheticDrugs(maint.OtherAnestheticDrugs);
        }

        public void DeleteMaintenanceInjectionDrugs(List<MaintenanceInjectionDrug> injects)
        {
            foreach (MaintenanceInjectionDrug inject in injects)
            {
                service.DeleteMaintenanceInjectionDrug(inject);
            }
        }

        public void DeleteMaintenanceInhalantDrugs(List<MaintenanceInhalantDrug> inhalants)
        {
            foreach (MaintenanceInhalantDrug inhalant in inhalants)
            {
                service.DeleteMaintenanceInhalantDrug(inhalant);
            }
        }

        public void DeleteIntraoperativeAnalgesia(List<IntraoperativeAnalgesia> analgesias)
        {
            foreach (IntraoperativeAnalgesia analgesia in analgesias)
            {
                service.DeleteIntraoperativeAnalgesia(analgesia);
            }
        }

        public void DeleteOtherAnestheticDrugs(List<OtherAnestheticDrug> otherDrugs)
        {
            foreach (OtherAnestheticDrug otherDrug in otherDrugs)
            {
                service.DeleteOtherAnestheticDrug(otherDrug);
            }
        }

        public void DeleteAnestheticPlan(AnestheticPlan anesPlan)
        {
            DeleteAnestheticPlanPremedications(anesPlan.PreMedications);
            DeleteAnestheticPlanInjections(anesPlan.InjectionPlans);
            DeleteAnestheticPlanInhalants(anesPlan.InhalantPlans);
        }

        public void DeleteAnestheticPlanPremedications(List<AnestheticPlanPremedication> premeds)
        {
            foreach (AnestheticPlanPremedication premed in premeds)
            {
                service.DeleteAnestheticPlanPremedication(premed);
            }
        }

        public void DeleteAnestheticPlanInjections(List<AnestheticPlanInjection> injections)
        {
            foreach (AnestheticPlanInjection inject in injections)
            {
                service.DeleteAnestheticPlanInjection(inject);
            }
        }

        public void DeleteAnestheticPlanInhalants(List<AnestheticPlanInhalant> inhalants)
        {
            foreach (AnestheticPlanInhalant inhalant in inhalants)
            {
                service.DeleteAnestheticPlanInhalant(inhalant);
            }
        }

        public void DeleteBloodworkGroup(List<Bloodwork> bloodworkGroup)
        {
            foreach (Bloodwork b in bloodworkGroup)
            {
                service.DeleteBloodwork(b);
            }
        }

        public void DeleteClinicalFindings(ClinicalFindings c)
        {
            DeleteCurrentMedications(c.CurrentMedications);
            DeletePriorAnesthesia(c.PriorAnesthesia);
            DeleteAnesthesiaConcerns(c.AnesthesiaConcerns);
            service.DeleteClinicalFinding(c);
        }

        public void DeleteCurrentMedications(List<CurrentMedication> medications)
        {
            foreach (CurrentMedication c in medications)
            {
                service.DeleteCurrentMedication(c);
            }
        }

        public void DeletePriorAnesthesia(List<PriorAnesthesia> anes)
        {
            foreach (PriorAnesthesia p in anes)
            {
                service.DeletePriorAnesthesia(p);
            }
        }

        public void DeleteAnesthesiaConcerns(List<AnesthesiaConcern> concerns)
        {
            foreach (AnesthesiaConcern a in concerns)
            {
                service.DeleteAnesthesiaConcern(a);
            }
        }

        #endregion
    }
}