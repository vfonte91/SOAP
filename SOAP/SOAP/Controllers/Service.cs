using System;
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

        public bool CheckUserForForgotPassword(ASFUser user)
        {
            return service.CheckUserForForgotPassword(user);
        }

        public bool SaveForgottenPassword(ASFUser user)
        {
            return service.UpdateForgottenPassword(user.Member);
        }

        #endregion

        #region GET

        public List<ASFUser> GetASFUsers()
        {
            return service.GetASFUsers();
        }

        public List<Patient> GetForms(ASFUser user)
        {
            return service.GetForms(user);
        }

        public List<DropdownCategory> GetDropdownCategoriesWithValues()
        {
            List<DropdownCategory> cats = service.GetDropdownCategories();
            foreach (DropdownCategory dCat in cats)
            {
                dCat.DropdownValues = service.GetDropdownCategoryValues(dCat.Id);
            }
            return cats;
        }

        public List<DropdownValue> GetDropdownValues(int catId)
        {
            List<DropdownValue> values = service.GetDropdownCategoryValues(catId);
            return values;
        }

        public Patient GetPatient(int patientId)
        {
            Patient pat = new Patient();
            pat.PatientId = patientId;
            pat.PatientInfo = GetPatientInformation(patientId);
            pat.ClinicalFindings = GetClinicalFindings(patientId);
            pat.AnestheticPlan = GetAnestheticPlan(patientId);
            pat.Bloodwork = GetBloodwork(patientId);
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
                PatientInformation.LazyComponents.LOAD_PREOP_PAIN_DETAIL,
                PatientInformation.LazyComponents.LOAD_PROCEDURE_DETAIL
            };
            PatientInformation patInfo = service.GetPatientInformation(patientId, list);
            return patInfo;
        }

        public ClinicalFindings GetClinicalFindings(int patientId)
        {
            ClinicalFindings.LazyComponents[] list = 
            {
                ClinicalFindings.LazyComponents.LOAD_CARDIAC_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_PHYSICAL_STATUS_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_PULSE_QUALITY_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_MUCOUS_MEMBRANE_WITH_DETAILS,
                ClinicalFindings.LazyComponents.LOAD_CAP_REFILL_WITH_DETAILS
            };
            ClinicalFindings clinicalFinding = service.GetClinicalFindings(patientId, list);
            clinicalFinding.PriorAnesthesia = GetPriorAnesthesia(patientId);
            clinicalFinding.AnesthesiaConcerns = GetAnesthesiaConcerns(patientId);
            return clinicalFinding;
        }

        public AnestheticPlan GetAnestheticPlan(int patientId)
        {
            AnestheticPlan anesPlan = new AnestheticPlan();
            anesPlan.PreMedications = GetAnestheticPreMedications(patientId);
            anesPlan.InjectionPlan = GetAnestheticPlanInjection(patientId);
            anesPlan.InhalantPlan = GetAnestheticPlanInhalant(patientId);
            return anesPlan;
        }

        public Maintenance GetMaintenance(int patientId)
        {
            Maintenance maint = new Maintenance();
            maint.MaintenanceInjectionDrug = GetMaintenanceInjectionDrugs(patientId);
            maint.MaintenanceInhalantDrug = GetMaintenanceInhalantDrugs(patientId);
            //maint.IntraOperativeAnalgesias = GetIntraOperativeAnalgesia(patientId);
            //maint.OtherAnestheticDrugs = GetOtherAnestheticDrugs(patientId);
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

        public PriorAnesthesia GetPriorAnesthesia(int patientId)
        {
            return service.GetPriorAnesthesia(patientId);
        }

        public List<AnesthesiaConcern> GetAnesthesiaConcerns(int patientId)
        {
            return service.GetAnesthesiaConcerns(patientId, AnesthesiaConcern.LazyComponents.LOAD_CONCERN_WITH_DETAILS);
        }

        public Bloodwork GetBloodwork(int patientId)
        {
            Bloodwork blood = new Bloodwork();
            List<Bloodwork> bloodGroup = service.GetBloodwork(patientId);
            foreach (Bloodwork b in bloodGroup)
            {
                if (b.Albumin != -1)
                    blood.Albumin = b.Albumin;
                else if (b.ALP != -1)
                    blood.ALP = b.ALP;
                else if (b.ALT != -1)
                    blood.ALT = b.ALT;
                else if (b.BUN != -1)
                    blood.BUN = b.BUN;
                else if (b.Ca != -1)
                    blood.Ca = b.Ca;
                else if (b.Cl != -1)
                    blood.Cl = b.Cl;
                else if (b.CREAT != -1)
                    blood.CREAT = b.CREAT;
                else if (b.Globulin != -1)
                    blood.Globulin = b.Globulin;
                else if (b.Glucose != -1)
                    blood.Glucose = b.Glucose;
                else if (b.iCa != -1)
                    blood.iCa = b.iCa;
                else if (b.K != -1)
                    blood.K = b.K;
                else if (b.NA != -1)
                    blood.NA = b.NA;
                else if (b.PCV != -1)
                    blood.PCV = b.PCV;
                else if (b.TP != -1)
                    blood.TP = b.TP;
                else if (b.USG != -1)
                    blood.USG = b.USG;
                else if (b.WBC != -1)
                    blood.WBC = b.WBC;
                else if (b.OtherType != "" && b.OtherValue != -1)
                {
                    blood.OtherType = b.OtherType;
                    blood.OtherValue = blood.OtherValue;
                }
            }
            return blood;
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

        public AnestheticPlanInjection GetAnestheticPlanInjection(int patientId)
        {
            AnestheticPlanInjection.LazyComponents[] list = 
            {
                AnestheticPlanInjection.LazyComponents.LOAD_ROUTE_WITH_DETAILS
            };
            return service.GetAnestheticPlanInjection(patientId, list);
        }

        public AnestheticPlanInhalant GetAnestheticPlanInhalant(int patientId)
        {
            return service.GetAnestheticPlanInhalant(patientId, AnestheticPlanInhalant.LazyComponents.LOAD_DRUG_WITH_DETAILS);
        }

        public MaintenanceInjectionDrug GetMaintenanceInjectionDrugs(int patientId)
        {
            MaintenanceInjectionDrug.LazyComponents[] list = 
            {
                MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS,
                MaintenanceInjectionDrug.LazyComponents.LOAD_INTRAOP_WITH_DETAILS
            };
            return service.GetMaintenanceInjectionDrugs(patientId, list);
        }

        public MaintenanceInhalantDrug GetMaintenanceInhalantDrugs(int patientId)
        {
            MaintenanceInhalantDrug.LazyComponents[] list = 
            {
                MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_DETAILS,
                MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS,
                MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS,
                MaintenanceInhalantDrug.LazyComponents.LOAD_INTRAOP_WITH_DETAILS
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

        public void CreateDropdownType(DropdownValue val)
        {
            service.CreateDropdownType(val);
        }

        public void CreatePatient(Patient pat)
        {
            pat.PatientId = service.CreatePatient(pat);
            if (pat.ClinicalFindings.ContainsValue())
                CreateClinicalFindings(pat);

            if (pat.Bloodwork.HasValues())
                CreateBloodwork(pat);

            if (pat.AnestheticPlan != null)
                CreateAnestheticPlan(pat);

            if (pat.Maintenance != null)
                CreateMaintenance(pat);

            if (pat.Monitoring != null)
                CreateMonitoring(pat);
        }

        public void CreateClinicalFindings(Patient pat)
        {
            pat.ClinicalFindings.PatientId = pat.PatientId;
            service.CreateClinicalFinding(pat.ClinicalFindings);

            if (pat.ClinicalFindings.PriorAnesthesia.DateOfProblem != DateTime.MinValue || pat.ClinicalFindings.PriorAnesthesia.Problem != null)
                CreatePriorAnesthesia(pat);

            if (pat.ClinicalFindings.AnesthesiaConcerns.Count > 0)
                CreateAnesthesiaConcerns(pat);
        }

        public void CreatePriorAnesthesia(Patient pat)
        {
            pat.ClinicalFindings.PriorAnesthesia.PatientId = pat.PatientId;
            service.CreatePriorAnesthesia(pat.ClinicalFindings.PriorAnesthesia);
        }

        public void CreateAnesthesiaConcerns(Patient pat)
        {
            foreach (AnesthesiaConcern a in pat.ClinicalFindings.AnesthesiaConcerns)
            {
                a.PatientId = pat.PatientId;
                service.CreateAnesthesiaConcern(a);
            }
        }

        public void CreateBloodwork(Patient pat)
        {
            Bloodwork blood = pat.Bloodwork;
            blood.PatientId = pat.PatientId;
            if (blood.Albumin != -1)
                service.CreateBloodwork(blood, "Albumin", blood.Albumin);
            if (blood.ALP != -1)
                service.CreateBloodwork(blood, "ALP", blood.ALP);
            if (blood.ALT != -1)
                service.CreateBloodwork(blood, "ALT", blood.ALT);
            if (blood.BUN != -1)
                service.CreateBloodwork(blood, "BUN", blood.BUN);
            if (blood.Ca != -1)
                service.CreateBloodwork(blood, "Ca", blood.Ca);
            if (blood.Cl != -1)
                service.CreateBloodwork(blood, "Cl", blood.Cl);
            if (blood.CREAT != -1)
                service.CreateBloodwork(blood, "CREAT", blood.CREAT);
            if (blood.Globulin != -1)
                service.CreateBloodwork(blood, "Globulin", blood.Globulin);
            if (blood.Glucose != -1)
                service.CreateBloodwork(blood, "Glucose", blood.Glucose);
            if (blood.iCa != -1)
                service.CreateBloodwork(blood, "iCa", blood.iCa);
            if (blood.K != -1)
                service.CreateBloodwork(blood, "K", blood.K);
            if (blood.NA != -1)
                service.CreateBloodwork(blood, "NA", blood.NA);
            if (blood.PCV != -1)
                service.CreateBloodwork(blood, "PCV", blood.PCV);
            if (blood.TP != -1)
                service.CreateBloodwork(blood, "TP", blood.TP);
            if (blood.USG != -1)
                service.CreateBloodwork(blood, "USG", blood.USG);
            if (blood.WBC != -1)
                service.CreateBloodwork(blood, "WBC", blood.WBC);
            if (blood.OtherType != "" && blood.OtherValue != -1)
                service.CreateBloodwork(blood, blood.OtherType, blood.OtherValue);
        }

        public void CreateAnestheticPlan(Patient pat)
        {
            if (pat.AnestheticPlan.InhalantPlan.HasValues())
                CreateAnestheticInhalantPlans(pat);

            if (pat.AnestheticPlan.InjectionPlan.HasValues())
                CreateAnestheticInjectionPlans(pat);

            if (pat.AnestheticPlan.PreMedications.Count > 0)
                CreateAnestheticPremedications(pat);
        }

        public void CreateAnestheticInhalantPlans(Patient pat)
        {
            pat.AnestheticPlan.InhalantPlan.PatientId = pat.PatientId;
            service.CreateAnestheticPlanInhalant(pat.AnestheticPlan.InhalantPlan);
        }

        public void CreateAnestheticInjectionPlans(Patient pat)
        {
            pat.AnestheticPlan.InjectionPlan.PatientId = pat.PatientId;
            service.CreateAnestheticPlanInjection(pat.AnestheticPlan.InjectionPlan);
        }

        public void CreateAnestheticPremedications(Patient pat)
        {
            foreach (AnestheticPlanPremedication a in pat.AnestheticPlan.PreMedications)
            {
                if (a.HasValues())
                {
                    a.PatientId = pat.PatientId;
                    service.CreateAnestheticPlanPremedication(a);
                }
            }
        }

        public void CreateMaintenance(Patient pat)
        {
            //if (pat.Maintenance.IntraOperativeAnalgesias.Count > 0)
            //    CreateIntraOperativeAnalgesias(pat);

            if (pat.Maintenance.MaintenanceInhalantDrug.HasValues())
                CreateMaintenanceInhalantDrugs(pat);

            if (pat.Maintenance.MaintenanceInjectionDrug.HasValues())
                CreateMaintenanceInjectionDrugs(pat);

            //if (pat.Maintenance.OtherAnestheticDrugs.Count > 0)
            //    CreateOtherAnestheticDrugs(pat);

        }

        //public void CreateIntraOperativeAnalgesias(Patient pat)
        //{
        //    foreach (IntraoperativeAnalgesia b in pat.Maintenance.IntraOperativeAnalgesias)
        //    {
        //        b.PatientId = pat.PatientId;
        //        service.CreateIntraoperativeAnalgesia(b);
        //    }
        //}

        public void CreateMaintenanceInhalantDrugs(Patient pat)
        {
                pat.Maintenance.MaintenanceInhalantDrug.PatientId = pat.PatientId;
                service.CreateMaintenanceInhalantDrug(pat.Maintenance.MaintenanceInhalantDrug);
        }

        public void CreateMaintenanceInjectionDrugs(Patient pat)
        {
                pat.Maintenance.MaintenanceInjectionDrug.PatientId = pat.PatientId;
                service.CreateMaintenanceInjectionDrug(pat.Maintenance.MaintenanceInjectionDrug);
        }

        //public void CreateOtherAnestheticDrugs(Patient pat)
        //{
        //    foreach (OtherAnestheticDrug o in pat.Maintenance.OtherAnestheticDrugs)
        //    {
        //        o.PatientId = pat.PatientId;
        //        service.CreateOtherAnestheticDrug(o);
        //    }
        //}

        public void CreateMonitoring(Patient pat)
        {
            foreach (Monitoring m in pat.Monitoring)
            {
                m.PatientId = pat.PatientId;
                service.CreateMonitoring(m);
            }
        }

        public bool CreateASFUser(ASFUser user)
        {
            service.CreateMembership(user.Member);
            return service.CreateASFUser(user);
        }

        #endregion

        #region SAVE

        public void SaveDropdownValue(DropdownValue val)
        {
            service.UpdateDropdownType(val);
        }

        public void SavePatient(Patient pat)
        {
            service.UpdatePatient(pat);

            if (pat.ClinicalFindings.ContainsValue())
                SaveClinicalFindings(pat.ClinicalFindings);

            if (pat.Bloodwork.HasValues())
                SaveBloodwork(pat.Bloodwork);

            if (pat.AnestheticPlan != null)
                SaveAnestheticPlan(pat.AnestheticPlan);

            if (pat.Maintenance != null)
                SaveMaintenance(pat.Maintenance);

            if (pat.Monitoring.Count > 0)
                SaveMonitoring(pat.Monitoring);
        }

        public void SaveClinicalFindings(ClinicalFindings clinicalFindings)
        {
            service.UpdateClinicalFinding(clinicalFindings);

            if (clinicalFindings.PriorAnesthesia.DateOfProblem != DateTime.MinValue || clinicalFindings.PriorAnesthesia.Problem != null)
                SavePriorAnesthesia(clinicalFindings.PriorAnesthesia);

            if (clinicalFindings.AnesthesiaConcerns.Count > 0)
                SaveAnesthesiaConcerns(clinicalFindings.AnesthesiaConcerns);
        }

        public void SaveCurrentMedications(List<CurrentMedication> meds)
        {
            foreach (CurrentMedication c in meds)
            {
                service.UpdateCurrentMedication(c);
            }
        }

        public void SavePriorAnesthesia(PriorAnesthesia priors)
        {
            service.UpdatePriorAnesthesia(priors);
        }

        public void SaveAnesthesiaConcerns(List<AnesthesiaConcern> concerns)
        {
            foreach (AnesthesiaConcern a in concerns)
            {
                service.UpdateAnesthesiaConcern(a);
            }
        }

        public void SaveBloodwork(Bloodwork blood)
        {
            if (blood.Albumin != -1)
                service.UpdateBloodwork(blood, "Albumin", blood.Albumin);
            if (blood.ALP != -1)
                service.UpdateBloodwork(blood, "ALP", blood.ALP);
            if (blood.ALT != -1)
                service.UpdateBloodwork(blood, "ALT", blood.ALT);
            if (blood.BUN != -1)
                service.UpdateBloodwork(blood, "BUN", blood.BUN);
            if (blood.Ca != -1)
                service.UpdateBloodwork(blood, "Ca", blood.Ca);
            if (blood.Cl != -1)
                service.UpdateBloodwork(blood, "Cl", blood.Cl);
            if (blood.CREAT != -1)
                service.UpdateBloodwork(blood, "CREAT", blood.CREAT);
            if (blood.Globulin != -1)
                service.UpdateBloodwork(blood, "Globulin", blood.Globulin);
            if (blood.Glucose != -1)
                service.UpdateBloodwork(blood, "Glucose", blood.Glucose);
            if (blood.iCa != -1)
                service.UpdateBloodwork(blood, "iCa", blood.iCa);
            if (blood.K != -1)
                service.UpdateBloodwork(blood, "K", blood.K);
            if (blood.NA != -1)
                service.UpdateBloodwork(blood, "NA", blood.NA);
            if (blood.PCV != -1)
                service.UpdateBloodwork(blood, "PCV", blood.PCV);
            if (blood.TP != -1)
                service.UpdateBloodwork(blood, "TP", blood.TP);
            if (blood.USG != -1)
                service.UpdateBloodwork(blood, "USG", blood.USG);
            if (blood.WBC != -1)
                service.UpdateBloodwork(blood, "WBC", blood.WBC);
            if (blood.OtherType != "" && blood.OtherValue != -1)
                service.UpdateBloodwork(blood, blood.OtherType, blood.OtherValue);
        }

        public void SaveAnestheticPlan(AnestheticPlan a)
        {
            if (a.InhalantPlan.HasValues())
                SaveAnestheticInhalantPlans(a.InhalantPlan);

            if (a.InjectionPlan.HasValues())
                SaveAnestheticInjectionPlans(a.InjectionPlan);

            if (a.PreMedications.Count > 0)
                SaveAnestheticPremedications(a.PreMedications);
        }

        public void SaveAnestheticInhalantPlans(AnestheticPlanInhalant inhalants)
        {
            service.UpdateAnestheticPlanInhalant(inhalants);
        }

        public void SaveAnestheticInjectionPlans(AnestheticPlanInjection injects)
        {
            service.UpdateAnestheticPlanInjection(injects);
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
            //if (m.IntraOperativeAnalgesias.Id != -1)
            //    SaveIntraOperativeAnalgesias(m.IntraOperativeAnalgesias);

            if (m.MaintenanceInhalantDrug.HasValues())
                SaveMaintenanceInhalantDrugs(m.MaintenanceInhalantDrug);

            if (m.MaintenanceInjectionDrug.HasValues())
                SaveMaintenanceInjectionDrugs(m.MaintenanceInjectionDrug);

        }

        //public void SaveIntraOperativeAnalgesias(DropdownValue intras)
        //{
        //    service.UpdateIntraoperativeAnalgesia(intras);
        //}

        public void SaveMaintenanceInhalantDrugs(MaintenanceInhalantDrug drugs)
        {
            service.UpdateMaintenanceInhalantDrug(drugs);
        }

        public void SaveMaintenanceInjectionDrugs(MaintenanceInjectionDrug drugs)
        {
            service.UpdateMaintenanceInjectionDrug(drugs);
        }

        public void SaveOtherAnestheticDrugs(OtherAnestheticDrug drugs)
        {
            service.UpdateOtherAnestheticDrug(drugs);
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
            List<Patient> pats = service.GetPatientWithUserId(user);
            foreach (Patient pat in pats)
            {
                DeletePatient(pat);
            }
            user.Member.Username = user.Username;
            service.DeleteASFUser(user);
            service.DeleteASPNetMembership(user.Member);
        }

        public void DeleteDropdownValue(DropdownValue val)
        {
            service.DeleteDropdownType(val);
        }

        public void DeletePatient(Patient pat)
        {
            DeleteMonitoring(pat);
            DeleteMaintenance(pat);
            DeleteAnestheticPlan(pat);
            DeleteBloodworkGroup(pat);
            DeleteClinicalFindings(pat);
            service.DeletePatient(pat.PatientId);
        }

        public void DeleteMonitoring(Patient pat)
        {
            service.DeleteMonitoring(pat.PatientId);
        }

        public void DeleteMaintenance(Patient pat)
        {
            DeleteMaintenanceInjectionDrugs(pat);
            DeleteMaintenanceInhalantDrugs(pat);
            DeleteIntraoperativeAnalgesia(pat);
            DeleteOtherAnestheticDrugs(pat);
        }

        public void DeleteMaintenanceInjectionDrugs(Patient pat)
        {
            service.DeleteMaintenanceInjectionDrug(pat.PatientId);
        }

        public void DeleteMaintenanceInhalantDrugs(Patient pat)
        {
            service.DeleteMaintenanceInhalantDrug(pat.PatientId);
        }

        public void DeleteIntraoperativeAnalgesia(Patient pat)
        {
            service.DeleteIntraoperativeAnalgesia(pat.PatientId);
        }

        public void DeleteOtherAnestheticDrugs(Patient pat)
        {
            service.DeleteOtherAnestheticDrug(pat.PatientId);
        }

        public void DeleteAnestheticPlan(Patient pat)
        {
            DeleteAnestheticPlanPremedications(pat);
            DeleteAnestheticPlanInjections(pat);
            DeleteAnestheticPlanInhalants(pat);
        }

        public void DeleteAnestheticPlanPremedications(Patient pat)
        {
            service.DeleteAnestheticPlanPremedication(pat.PatientId);
        }

        public void DeleteAnestheticPlanInjections(Patient pat)
        {
            service.DeleteAnestheticPlanInjection(pat.PatientId);
        }

        public void DeleteAnestheticPlanInhalants(Patient pat)
        {
            service.DeleteAnestheticPlanInhalant(pat.PatientId);
        }

        public void DeleteBloodworkGroup(Patient pat)
        {
            service.DeleteBloodwork(pat.PatientId);
        }

        public void DeleteClinicalFindings(Patient pat)
        {
            DeleteCurrentMedications(pat);
            DeletePriorAnesthesia(pat);
            DeleteAnesthesiaConcerns(pat);
            service.DeleteClinicalFinding(pat.PatientId);
        }

        public void DeleteCurrentMedications(Patient pat)
        {
            service.DeleteCurrentMedication(pat.PatientId);
        }

        public void DeletePriorAnesthesia(Patient pat)
        {
            service.DeletePriorAnesthesia(pat.PatientId);
        }

        public void DeleteAnesthesiaConcerns(Patient pat)
        {
            service.DeleteAnesthesiaConcern(pat.PatientId);
        }

        #endregion
    }
}