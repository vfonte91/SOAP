using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using SOAP.Models;
using SOAP.Models.Callbacks;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;

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
            maint.MaintenanceOther = GetMaintenanceOther(patientId);
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
                    blood.OtherValue = b.OtherValue;
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
                MaintenanceInjectionDrug.LazyComponents.LOAD_DRUG_INFORMATION,
                MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS
            };
            return service.GetMaintenanceInjectionDrugs(patientId, list);
        }

        public MaintenanceInhalantDrug GetMaintenanceInhalantDrugs(int patientId)
        {
            MaintenanceInhalantDrug.LazyComponents[] list = 
            {
                MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_DETAILS,
                MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS,
                MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS
            };
            return service.GetMaintenanceInhalantDrugs(patientId, list);
        }

        public MaintenanceOther GetMaintenanceOther(int patientId)
        {
            MaintenanceOther.LazyComponents[] list = 
            {
                MaintenanceOther.LazyComponents.LOAD_INTRAOP_WITH_DETAILS,
                MaintenanceOther.LazyComponents.LOAD_IV_WITH_DETAILS
            };
            return service.GetMaintenanceOther(patientId, list);
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

            if (pat.Monitoring.Count > 0)
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
            if (blood.OtherType != null && blood.OtherValue != -1)
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
            if (pat.Maintenance.MaintenanceInhalantDrug.HasValues())
                CreateMaintenanceInhalantDrugs(pat);

            if (pat.Maintenance.MaintenanceInjectionDrug.HasValues())
                CreateMaintenanceInjectionDrugs(pat);

            if (pat.Maintenance.MaintenanceOther.HasValues())
                CreateMaintenanceOther(pat);

        }

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

        public void CreateMaintenanceOther(Patient pat)
        {
            pat.Maintenance.MaintenanceOther.PatientId = pat.PatientId;
            service.CreateMaintenanceOther(pat.Maintenance.MaintenanceOther);
        }

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
            {
                pat.ClinicalFindings.PatientId = pat.PatientId;
                SaveClinicalFindings(pat.ClinicalFindings);
            }

            if (pat.Bloodwork.HasValues())
            {
                pat.Bloodwork.PatientId = pat.PatientId;
                SaveBloodwork(pat.Bloodwork);
            }

            if (pat.AnestheticPlan != null)
            {
                if (pat.AnestheticPlan.InhalantPlan.HasValues())
                {
                    pat.AnestheticPlan.InhalantPlan.PatientId = pat.PatientId;
                    SaveAnestheticInhalantPlans(pat.AnestheticPlan.InhalantPlan);
                }

                if (pat.AnestheticPlan.InjectionPlan.HasValues())
                {
                    pat.AnestheticPlan.InjectionPlan.PatientId = pat.PatientId;
                    SaveAnestheticInjectionPlans(pat.AnestheticPlan.InjectionPlan);
                }

                if (pat.AnestheticPlan.PreMedications.Count > 0)
                {
                    SaveAnestheticPremedications(pat);
                }
            }

            if (pat.Maintenance != null)
            {
                if (pat.Maintenance.MaintenanceInhalantDrug.HasValues())
                {
                    pat.Maintenance.MaintenanceInhalantDrug.PatientId = pat.PatientId;
                    SaveMaintenanceInhalantDrugs(pat.Maintenance.MaintenanceInhalantDrug);
                }

                if (pat.Maintenance.MaintenanceInjectionDrug.HasValues())
                {
                    pat.Maintenance.MaintenanceInhalantDrug.PatientId = pat.PatientId;
                    SaveMaintenanceInjectionDrugs(pat.Maintenance.MaintenanceInjectionDrug);
                }

                if (pat.Maintenance.MaintenanceOther.HasValues())
                {
                    pat.Maintenance.MaintenanceOther.PatientId = pat.PatientId;
                    SaveMaintenanceOther(pat.Maintenance.MaintenanceOther);
                }
            }

            if (pat.Monitoring.Count > 0)
                SaveMonitoring(pat);
        }

        public void SaveClinicalFindings(ClinicalFindings clinicalFindings)
        {
            if (service.UpdateClinicalFinding(clinicalFindings) == 0)
            {
                service.CreateClinicalFinding(clinicalFindings);
            }

            if (clinicalFindings.PriorAnesthesia.DateOfProblem != DateTime.MinValue || clinicalFindings.PriorAnesthesia.Problem != null)
            {
                clinicalFindings.PriorAnesthesia.PatientId = clinicalFindings.PatientId;
                SavePriorAnesthesia(clinicalFindings.PriorAnesthesia);
            }

            if (clinicalFindings.AnesthesiaConcerns.Count > 0)
            {
                service.DeleteAnesthesiaConcern(clinicalFindings.PatientId);
                SaveAnesthesiaConcerns(clinicalFindings.AnesthesiaConcerns);
            }
        }

        //public void SaveCurrentMedications(List<CurrentMedication> meds)
        //{
        //    foreach (CurrentMedication c in meds)
        //    {
        //        service.UpdateCurrentMedication(c);
        //    }
        //}

        public void SavePriorAnesthesia(PriorAnesthesia priors)
        {
            if (service.UpdatePriorAnesthesia(priors) == 0)
                service.CreatePriorAnesthesia(priors);
        }

        public void SaveAnesthesiaConcerns(List<AnesthesiaConcern> concerns)
        {
            foreach (AnesthesiaConcern a in concerns)
            {
                service.CreateAnesthesiaConcern(a);
            }
        }

        public void SaveBloodwork(Bloodwork blood)
        {
            service.DeleteBloodwork(blood.PatientId);
            if (blood.Albumin != -1)
                //if (service.UpdateBloodwork(blood, "Albumin", blood.Albumin) == 0)
                service.CreateBloodwork(blood, "Albumin", blood.Albumin);
            if (blood.ALP != -1)
                //if (service.UpdateBloodwork(blood, "ALP", blood.ALP) == 0)
                service.CreateBloodwork(blood, "ALP", blood.ALP);
            if (blood.ALT != -1)
                //if (service.UpdateBloodwork(blood, "ALT", blood.ALT) == 0)
                service.CreateBloodwork(blood, "ALT", blood.ALT);
            if (blood.BUN != -1)
                //if (service.UpdateBloodwork(blood, "BUN", blood.BUN) == 0)
                service.UpdateBloodwork(blood, "BUN", blood.BUN);
            if (blood.Ca != -1)
                //if (service.UpdateBloodwork(blood, "Ca", blood.Ca) == 0)
                service.CreateBloodwork(blood, "Ca", blood.Ca);
            if (blood.Cl != -1)
                //if (service.UpdateBloodwork(blood, "Cl", blood.Cl) == 0)
                service.CreateBloodwork(blood, "Cl", blood.Cl);
            if (blood.CREAT != -1)
                if (service.UpdateBloodwork(blood, "CREAT", blood.CREAT) == 0)
                    service.CreateBloodwork(blood, "CREAT", blood.CREAT);
            if (blood.Globulin != -1)
                //if (service.UpdateBloodwork(blood, "Globulin", blood.Globulin) == 0)
                service.CreateBloodwork(blood, "Globulin", blood.Globulin);
            if (blood.Glucose != -1)
                //if (service.UpdateBloodwork(blood, "Glucose", blood.Glucose) == 0)
                service.CreateBloodwork(blood, "Glucose", blood.Glucose);
            if (blood.iCa != -1)
                //if (service.UpdateBloodwork(blood, "iCa", blood.iCa) == 0)
                service.CreateBloodwork(blood, "iCa", blood.iCa);
            if (blood.K != -1)
                //if (service.UpdateBloodwork(blood, "K", blood.K) == 0)
                service.CreateBloodwork(blood, "K", blood.K);
            if (blood.NA != -1)
                //if (service.UpdateBloodwork(blood, "NA", blood.NA) == 0)
                service.CreateBloodwork(blood, "NA", blood.NA);
            if (blood.PCV != -1)
                //if (service.UpdateBloodwork(blood, "PCV", blood.PCV) == 0)
                service.CreateBloodwork(blood, "PCV", blood.PCV);
            if (blood.TP != -1)
                //if (service.UpdateBloodwork(blood, "TP", blood.TP) == 0)
                service.CreateBloodwork(blood, "TP", blood.TP);
            if (blood.USG != -1)
                //if (service.UpdateBloodwork(blood, "USG", blood.USG) == 0)
                service.CreateBloodwork(blood, "USG", blood.USG);
            if (blood.WBC != -1)
                //if (service.UpdateBloodwork(blood, "WBC", blood.WBC) == 0)
                service.CreateBloodwork(blood, "WBC", blood.WBC);
            if (blood.OtherType != null && blood.OtherValue != -1)
                //if (service.UpdateBloodwork(blood, blood.OtherType, blood.OtherValue) == 0)
                service.CreateBloodwork(blood, blood.OtherType, blood.OtherValue);
        }

        public void SaveAnestheticInhalantPlans(AnestheticPlanInhalant inhalants)
        {
            service.DeleteAnestheticPlanInjection(inhalants.PatientId);
            if (service.UpdateAnestheticPlanInhalant(inhalants) == 0)
                service.CreateAnestheticPlanInhalant(inhalants);
        }

        public void SaveAnestheticInjectionPlans(AnestheticPlanInjection injects)
        {
            service.DeleteAnestheticPlanInhalant(injects.PatientId);
            if (service.UpdateAnestheticPlanInjection(injects) == 0)
                service.CreateAnestheticPlanInjection(injects);
        }

        public void SaveAnestheticPremedications(Patient pat)
        {
            service.DeleteAnestheticPlanPremedication(pat.PatientId);
            foreach (AnestheticPlanPremedication a in pat.AnestheticPlan.PreMedications)
            {
                a.PatientId = pat.PatientId;
                service.CreateAnestheticPlanPremedication(a);
            }
        }

        public void SaveMaintenanceInhalantDrugs(MaintenanceInhalantDrug drugs)
        {
            service.DeleteMaintenanceInjectionDrug(drugs.PatientId);
            if (service.UpdateMaintenanceInhalantDrug(drugs) == 0)
                service.CreateMaintenanceInhalantDrug(drugs);
        }

        public void SaveMaintenanceInjectionDrugs(MaintenanceInjectionDrug drugs)
        {
            service.DeleteMaintenanceInhalantDrug(drugs.PatientId);
            if (service.UpdateMaintenanceInjectionDrug(drugs) == 0)
                service.CreateMaintenanceInjectionDrug(drugs);
        }

        public void SaveMaintenanceOther(MaintenanceOther drugs)
        {
            if (service.UpdateMaintenanceOther(drugs) == 0)
                service.CreateMaintenanceOther(drugs);
        }

        public void SaveMonitoring(Patient pat)
        {
            service.DeleteMonitoring(pat.PatientId);
            foreach (Monitoring m in pat.Monitoring)
            {
                m.PatientId = pat.PatientId;
                if (service.UpdateMonitoring(m) == 0)
                    service.CreateMonitoring(m);
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
            DeleteMaintenanceOther(pat);
        }

        public void DeleteMaintenanceInjectionDrugs(Patient pat)
        {
            service.DeleteMaintenanceInjectionDrug(pat.PatientId);
        }

        public void DeleteMaintenanceInhalantDrugs(Patient pat)
        {
            service.DeleteMaintenanceInhalantDrug(pat.PatientId);
        }

        public void DeleteMaintenanceOther(Patient pat)
        {
            service.DeleteMaintenanceOther(pat.PatientId);
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

        public void Export(Patient p)
        {
            //CALL SAVE FORM
            //CALL GET FORM
           
            #region PDF OUTPUT

            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(0f, 0f, 0f, 0f);

            MemoryStream memStream = new MemoryStream();

            try
            {
                var imgpath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Test.jpg");
                var pdfpath = System.Web.HttpContext.Current.Server.MapPath("~/PDFs/SOAP.pdf");
                Font NinePointFont = new Font(Font.NORMAL, 9f);

                //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream("../PDFs/SOAP.pdf", FileMode.Create));
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfpath, FileMode.Create));

                doc.Open();


                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgpath);
                doc.Add(img);


                PdfContentByte cb = writer.DirectContent;
                ColumnText ct = new ColumnText(cb);


                //string A1 = p.PatientInfo.Student.ToString();

                Phrase Anesthetist = new Phrase(p.PatientInfo.Student.ToString());
                Phrase Date = new Phrase("10-10-2000");
                Phrase Clinician = new Phrase("PET DETECTIVE");
                Phrase Stall = new Phrase("Central 354");
                Phrase Procedure = new Phrase("Fix the Pet.");
                Phrase BodyWeight = new Phrase("20");
                Phrase Age = new Phrase("3.4");
                Phrase Temperament = new Phrase("rabid, very dangerous..");

                Phrase Temp = new Phrase("101.8");
                Phrase Pulse = new Phrase("160");
                Phrase Response = new Phrase("XXXXX");

                Phrase CardiacAuscultation = new Phrase("What is this...?");
                Phrase PulseQuality = new Phrase("ok i guess");
                Phrase MucousMembraneColor = new Phrase("gross, dont ask");
                Phrase CapillaryRefillTime = new Phrase("6 hours 15 minutes");
                Phrase RespiratoryAuscultation = new Phrase("What is this?");
                Phrase PhysicalStatusClassification = new Phrase("III");

                Phrase ReasonForClassification = new Phrase("Reason for Classification");
                Phrase CurrentMedications = new Phrase("Current Medications");
                Phrase AnesthesiaConcerns = new Phrase("This is a concern, this is another concern, this is a last concern.");

                Phrase Drug1 = new Phrase("DRUG 1");
                Phrase Route1 = new Phrase("Route 1");
                Phrase Dosage1 = new Phrase("Dosage 1");
                Phrase DoseMg1 = new Phrase("DoseMg 1");
                Phrase DoseMl1 = new Phrase("DoseMl 1");

                Phrase Injectable1 = new Phrase("Injectable 1");

                Phrase MaintenaceInjectable = new Phrase("DRUG #5");
                Phrase InhalantType = new Phrase("Drug X");
                Phrase InhalantInductionPercent = new Phrase("90.84%");
                Phrase InhalantMaintenancePercent = new Phrase("64.82%");

                Phrase OxygenFlowRate = new Phrase("875.9876");

                Phrase BreathingSystem = new Phrase("x");

                Phrase IntraoperativeAnalgesia = new Phrase("DRUG X, DRUG Y, DRUG Z.. . .");
                Phrase OtherAnestheticDrugs = new Phrase(" DRUG A, DRUG B, DRUG C");
                Phrase Monitoring = new Phrase("ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ");

                Phrase IVFluidType = new Phrase("IV TYPE X");
                Phrase IVDoseMl1 = new Phrase("XX.XX");
                Phrase IVDoseDrops1 = new Phrase("XXX");

                Phrase MiniDrip = new Phrase("X");
                Phrase MaxiDrip = new Phrase("X");

                Phrase PreOpPainAssessment = new Phrase("This is a new pre op pain assessment. . . . ");
                Phrase PostOpPainAssessment = new Phrase("This is a new post op pain assessment. . .");

                Phrase PVC = new Phrase("XX.X", NinePointFont);
                Phrase TP = new Phrase("XX.X", NinePointFont);
                Phrase Alb = new Phrase("XX.X", NinePointFont);
                Phrase Glob = new Phrase("XX.X", NinePointFont);
                Phrase WBC = new Phrase("XX.X", NinePointFont);
                Phrase Na = new Phrase("XX.X", NinePointFont);
                Phrase K = new Phrase("XX.X", NinePointFont);
                Phrase Cl = new Phrase("XX.X", NinePointFont);
                Phrase Ca = new Phrase("XX.X", NinePointFont);
                Phrase iCa = new Phrase("XX.X", NinePointFont);
                Phrase Glucose = new Phrase("XX.X", NinePointFont);
                Phrase ALT = new Phrase("XX.X", NinePointFont);
                Phrase ALP = new Phrase("XX.X", NinePointFont);
                Phrase BUN = new Phrase("XX.X", NinePointFont);
                Phrase CREAT = new Phrase("XX.X", NinePointFont);
                Phrase USG = new Phrase("XX.X", NinePointFont);

                Phrase OtherProblems = new Phrase("OTHER PROBLEMS");
                Phrase OtherAnesthesia = new Phrase("SOMETHING. . .");





                //parameters of SetSimpleColumn: (Phrase, leftmargin coordinate, bottommargin coordinate, box width, box height, line height, alignment)     
                //NOTE: all dimensions start at the BOTTOM LEFT of the PDF... why? I dont fucking know, its retarded.
                //NOTE: bottommargin determines x coordinate

                //                          LEFT     BOT     WIDTH    HEIGHT   LINE HEIGHT          ALIGN

                #region TOP HALF
                ct.SetSimpleColumn(Anesthetist, 122, 752, 280, 767, 15, Element.ALIGN_LEFT);
                ct.Go();   //ANESTHETIST

                ct.SetSimpleColumn(Date, 77, 736, 290, 751, 15, Element.ALIGN_LEFT);
                ct.Go();   //DATE

                ct.SetSimpleColumn(Clinician, 108, 723, 280, 738, 15, Element.ALIGN_LEFT);
                ct.Go();   //Clinician

                ct.SetSimpleColumn(Stall, 185, 709, 280, 724, 15, Element.ALIGN_LEFT);
                ct.Go();   //Stall

                ct.SetSimpleColumn(Procedure, 116, 683, 280, 698, 15, Element.ALIGN_LEFT);
                ct.Go();   //Procedure

                ct.SetSimpleColumn(BodyWeight, 127, 655, 161, 670, 15, Element.ALIGN_LEFT);
                ct.Go();  //Body Weight

                ct.SetSimpleColumn(Age, 235, 655, 280, 670, 15, Element.ALIGN_LEFT);
                ct.Go();  //Age

                ct.SetSimpleColumn(Temperament, 138, 628, 280, 643, 15, Element.ALIGN_LEFT);
                ct.Go();  //Temperment

                ct.SetSimpleColumn(Temp, 63, 598, 107, 613, 15, Element.ALIGN_LEFT);
                ct.Go();  //Temp

                ct.SetSimpleColumn(Pulse, 135, 598, 165, 613, 15, Element.ALIGN_LEFT);
                ct.Go();  //Pulse

                ct.SetSimpleColumn(Response, 207, 598, 250, 613, 15, Element.ALIGN_LEFT);
                ct.Go();  //Response

                ct.SetSimpleColumn(CardiacAuscultation, 136, 584, 250, 599, 15, Element.ALIGN_LEFT);
                ct.Go();  //Cardiac Auscultation

                ct.SetSimpleColumn(PulseQuality, 101, 571, 250, 586, 15, Element.ALIGN_LEFT);
                ct.Go();  //PulseQuality

                ct.SetSimpleColumn(MucousMembraneColor, 155, 557, 250, 572, 15, Element.ALIGN_LEFT);
                ct.Go();  //Mucous Membrane Color

                ct.SetSimpleColumn(CapillaryRefillTime, 139, 543, 250, 558, 15, Element.ALIGN_LEFT);
                ct.Go();  //Capillary Refill Time

                ct.SetSimpleColumn(RespiratoryAuscultation, 154, 529, 250, 544, 15, Element.ALIGN_LEFT);
                ct.Go();  //Respiratory Auscultation

                ct.SetSimpleColumn(PhysicalStatusClassification, 176, 516, 250, 531, 15, Element.ALIGN_LEFT);
                ct.Go();  //Physical Status Classification

                ct.SetSimpleColumn(ReasonForClassification, 40, 447, 250, 502, 15, Element.ALIGN_LEFT);
                ct.Go();  //Physical Status Classification

                ct.SetSimpleColumn(CurrentMedications, 30, 446, 250, 476, 15, Element.ALIGN_LEFT);
                ct.Go();  //CurrentMedication

                #endregion

                #region DRUGS
                //       LEFT     BOT     WIDTH    HEIGHT   LINE HEIGHT          ALIGN

                ct.SetSimpleColumn(AnesthesiaConcerns, 40, 400, 550, 425, 15, Element.ALIGN_LEFT);
                ct.Go();  //Physical Status Classification

                ct.SetSimpleColumn(Drug1, 95, 363, 200, 378, 15, Element.ALIGN_LEFT);
                ct.Go();  //Drug

                ct.SetSimpleColumn(Route1, 201, 363, 270, 378, 15, Element.ALIGN_LEFT);
                ct.Go();  //Route

                ct.SetSimpleColumn(Dosage1, 271, 363, 385, 378, 15, Element.ALIGN_LEFT);
                ct.Go();  //Dosage

                ct.SetSimpleColumn(DoseMg1, 385, 363, 455, 378, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseMG

                ct.SetSimpleColumn(DoseMl1, 456, 363, 550, 378, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseML


                ct.SetSimpleColumn(Drug1, 95, 348, 200, 363, 15, Element.ALIGN_LEFT);
                ct.Go();  //Drug

                ct.SetSimpleColumn(Route1, 201, 348, 270, 363, 15, Element.ALIGN_LEFT);
                ct.Go();  //Route

                ct.SetSimpleColumn(Dosage1, 271, 348, 385, 363, 15, Element.ALIGN_LEFT);
                ct.Go();  //Dosage

                ct.SetSimpleColumn(DoseMg1, 385, 348, 455, 363, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseMG

                ct.SetSimpleColumn(DoseMl1, 456, 348, 550, 363, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseML

                ct.SetSimpleColumn(Drug1, 95, 333, 200, 348, 15, Element.ALIGN_LEFT);
                ct.Go();  //Drug

                ct.SetSimpleColumn(Route1, 201, 333, 270, 348, 15, Element.ALIGN_LEFT);
                ct.Go();  //Route

                ct.SetSimpleColumn(Dosage1, 271, 333, 385, 348, 15, Element.ALIGN_LEFT);
                ct.Go();  //Dosage

                ct.SetSimpleColumn(DoseMg1, 385, 333, 455, 348, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseMG

                ct.SetSimpleColumn(DoseMl1, 456, 333, 550, 348, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseML

                #endregion

                #region INJECTABLES

                ct.SetSimpleColumn(Drug1, 95, 320, 200, 335, 15, Element.ALIGN_LEFT);
                ct.Go();  //Drug

                ct.SetSimpleColumn(Route1, 201, 320, 270, 335, 15, Element.ALIGN_LEFT);
                ct.Go();  //Route

                ct.SetSimpleColumn(Dosage1, 271, 320, 385, 335, 15, Element.ALIGN_LEFT);
                ct.Go();  //Dosage

                ct.SetSimpleColumn(DoseMg1, 385, 320, 455, 335, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseMG

                ct.SetSimpleColumn(DoseMl1, 456, 320, 550, 335, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseML

                ct.SetSimpleColumn(Drug1, 95, 305, 200, 320, 15, Element.ALIGN_LEFT);
                ct.Go();  //Drug

                ct.SetSimpleColumn(Route1, 201, 305, 270, 320, 15, Element.ALIGN_LEFT);
                ct.Go();  //Route

                ct.SetSimpleColumn(Dosage1, 271, 305, 385, 320, 15, Element.ALIGN_LEFT);
                ct.Go();  //Dosage

                ct.SetSimpleColumn(DoseMg1, 385, 305, 455, 320, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseMG

                ct.SetSimpleColumn(DoseMl1, 456, 305, 550, 320, 15, Element.ALIGN_LEFT);
                ct.Go();  //DoseML
                #endregion

                #region MIDDLE HALF

                ct.SetSimpleColumn(DoseMl1, 180, 295, 550, 310, 15, Element.ALIGN_LEFT);
                ct.Go();  //MaintenaceInjectable

                ct.SetSimpleColumn(InhalantType, 180, 280, 275, 295, 15, Element.ALIGN_LEFT);
                ct.Go();  //InhalantType

                ct.SetSimpleColumn(InhalantInductionPercent, 334, 280, 390, 295, 15, Element.ALIGN_LEFT);
                ct.Go();  //InhalantInductionPercent

                ct.SetSimpleColumn(InhalantMaintenancePercent, 495, 280, 536, 295, 15, Element.ALIGN_LEFT);
                ct.Go();  //InhalantMaintenancePercent

                ct.SetSimpleColumn(OxygenFlowRate, 210, 267, 275, 282, 15, Element.ALIGN_LEFT);
                ct.Go();  //OxygenFlowRate

                ct.SetSimpleColumn(InhalantInductionPercent, 334, 267, 390, 282, 15, Element.ALIGN_LEFT);
                ct.Go();  //InhalantInductionPercent

                ct.SetSimpleColumn(InhalantMaintenancePercent, 495, 267, 536, 282, 15, Element.ALIGN_LEFT);
                ct.Go();  //InhalantMaintenancePercent

                ct.SetSimpleColumn(InhalantMaintenancePercent, 495, 267, 536, 282, 15, Element.ALIGN_LEFT);
                ct.Go();  //InhalantMaintenancePercent

                #endregion

                #region LOWERHALF


                //TODO: Add if statement for either or checkbox option

                ct.SetSimpleColumn(BreathingSystem, 193, 200, 210, 270, 15, Element.ALIGN_LEFT);
                ct.Go();  //BreathingSystem

                ct.SetSimpleColumn(BreathingSystem, 237, 200, 275, 270, 15, Element.ALIGN_LEFT);
                ct.Go();  //BreathingSystem

                ct.SetSimpleColumn(IntraoperativeAnalgesia, 35, 225, 575, 240, 15, Element.ALIGN_LEFT);
                ct.Go();  //IntraoperativeAnalgesia

                ct.SetSimpleColumn(OtherAnestheticDrugs, 145, 212, 575, 227, 15, Element.ALIGN_LEFT);
                ct.Go();  //OtherAnestheticDrugs

                ct.SetSimpleColumn(Monitoring, 101, 145, 575, 200, 15, Element.ALIGN_LEFT);
                ct.Go();  //Monitoring


                //                          LEFT     BOT     WIDTH    HEIGHT   LINE HEIGHT          ALIGN

                ct.SetSimpleColumn(IVFluidType, 105, 130, 232, 145, 15, Element.ALIGN_LEFT);
                ct.Go();  //IVFluidType

                ct.SetSimpleColumn(IVDoseMl1, 278, 130, 333, 145, 15, Element.ALIGN_LEFT);
                ct.Go();  //IVDoseMl1

                ct.SetSimpleColumn(IVDoseDrops1, 373, 130, 445, 145, 15, Element.ALIGN_LEFT);
                ct.Go();  //IVDoseDrops

                ct.SetSimpleColumn(IVFluidType, 105, 116, 232, 131, 15, Element.ALIGN_LEFT);
                ct.Go();  //IVFluidType

                ct.SetSimpleColumn(IVDoseMl1, 278, 116, 333, 131, 15, Element.ALIGN_LEFT);
                ct.Go();  //IVDoseMl1

                ct.SetSimpleColumn(IVDoseDrops1, 373, 116, 445, 131, 15, Element.ALIGN_LEFT);
                ct.Go();  //IVDoseDrops

                ct.SetSimpleColumn(MaxiDrip, 127, 103, 135, 118, 15, Element.ALIGN_LEFT);
                ct.Go();  //MaxiDrip

                ct.SetSimpleColumn(MiniDrip, 271, 103, 280, 118, 15, Element.ALIGN_LEFT);
                ct.Go();  //MiniDrip

                ct.SetSimpleColumn(PreOpPainAssessment, 275, 583, 575, 613, 15, Element.ALIGN_LEFT);
                ct.Go();    //PreOpPainAssessment

                ct.SetSimpleColumn(PostOpPainAssessment, 275, 550, 575, 573, 15, Element.ALIGN_LEFT);
                ct.Go();    //PostOpPainAssessment

                #endregion


                #region OTHER MISC. DRUGS

                //                          LEFT     BOT     WIDTH    HEIGHT   LINE HEIGHT          ALIGN

                ct.SetSimpleColumn(PVC, 340, 530, 377, 545, 15, Element.ALIGN_LEFT);
                ct.Go();    //PVC

                ct.SetSimpleColumn(TP, 388, 530, 545, 545, 15, Element.ALIGN_LEFT);
                ct.Go();    //TP

                ct.SetSimpleColumn(Alb, 443, 530, 550, 545, 15, Element.ALIGN_LEFT);
                ct.Go();    //Alb

                ct.SetSimpleColumn(Glob, 490, 530, 550, 545, 15, Element.ALIGN_LEFT);
                ct.Go();    //Glob

                ct.SetSimpleColumn(WBC, 542, 530, 575, 545, 15, Element.ALIGN_LEFT);
                ct.Go();    //WBC



                ct.SetSimpleColumn(Na, 340, 515, 500, 530, 15, Element.ALIGN_LEFT);
                ct.Go();    //Na

                ct.SetSimpleColumn(K, 388, 515, 550, 530, 15, Element.ALIGN_LEFT);
                ct.Go();    //K

                ct.SetSimpleColumn(Cl, 443, 515, 550, 530, 15, Element.ALIGN_LEFT);
                ct.Go();    //Cl

                ct.SetSimpleColumn(Ca, 490, 515, 550, 530, 15, Element.ALIGN_LEFT);
                ct.Go();    //Ca

                ct.SetSimpleColumn(iCa, 542, 515, 575, 530, 15, Element.ALIGN_LEFT);
                ct.Go();    //iCa




                ct.SetSimpleColumn(Glucose, 355, 503, 440, 518, 15, Element.ALIGN_LEFT);
                ct.Go();    //Glucose

                ct.SetSimpleColumn(ALT, 448, 503, 545, 518, 15, Element.ALIGN_LEFT);
                ct.Go();    //ALT

                ct.SetSimpleColumn(ALP, 515, 503, 550, 518, 15, Element.ALIGN_LEFT);
                ct.Go();    //ALP


                ct.SetSimpleColumn(BUN, 343, 448, 440, 503, 15, Element.ALIGN_LEFT);
                ct.Go();    //BUN

                ct.SetSimpleColumn(CREAT, 435, 448, 545, 503, 15, Element.ALIGN_LEFT);
                ct.Go();    //CREAT

                ct.SetSimpleColumn(USG, 512, 448, 550, 503, 15, Element.ALIGN_LEFT);
                ct.Go();    //USG

                ct.SetSimpleColumn(OtherProblems, 380, 475, 550, 490, 15, Element.ALIGN_LEFT);
                ct.Go();    //Other Problems

                ct.SetSimpleColumn(OtherAnesthesia, 477, 462, 580, 477, 15, Element.ALIGN_LEFT);
                ct.Go();    //OtherAnesthesia (Prior)






                #endregion

            }

            catch
            {
            }
            finally
            {
                doc.Close();
            }

            #endregion






        }
    }
}