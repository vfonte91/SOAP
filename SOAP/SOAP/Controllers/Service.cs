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
            if (blood.OtherType != null && blood.OtherValue != -1)
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
            if (m.MaintenanceInhalantDrug.HasValues())
                SaveMaintenanceInhalantDrugs(m.MaintenanceInhalantDrug);

            if (m.MaintenanceInjectionDrug.HasValues())
                SaveMaintenanceInjectionDrugs(m.MaintenanceInjectionDrug);

            if (m.MaintenanceOther.HasValues())
                SaveMaintenanceOther(m.MaintenanceOther);

        }
        public void SaveMaintenanceInhalantDrugs(MaintenanceInhalantDrug drugs)
        {
            service.UpdateMaintenanceInhalantDrug(drugs);
        }

        public void SaveMaintenanceInjectionDrugs(MaintenanceInjectionDrug drugs)
        {
            service.UpdateMaintenanceInjectionDrug(drugs);
        }

        public void SaveMaintenanceOther(MaintenanceOther drugs)
        {
            service.UpdateMaintenanceOther(drugs);
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

        public void Page_Load(object sender, EventArgs e)
        {


            #region SIMPLE TEXT OUTPUT
            /*
        string path = Server.MapPath("PDFs");
        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        Font times = new Font(bfTimes, 9, Font.NORMAL);
        Document doc = new Document();

        try
        {
            doc.Open();

            doc.Add(new Paragraph("This is font size 9, times new roman type", times));
            
        }
        catch
        {

        }
        finally
        {
            doc.Close();
        }
*/
            #endregion

            #region WORD WRAPPING
            /*
        string path = Server.MapPath("PDFs");
        Document doc = new Document(PageSize.LETTER);

        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        Font times = new Font(bfTimes, 9, Font.NORMAL);

        try
        {
            PdfWriter.GetInstance(doc, new FileStream(path + "/WordWrapping.pdf", FileMode.Create));
            doc.Open();
            Chunk c1 = new Chunk("THHHHHIS IS A PHHHHHHHRASE?  ", times);

            Phrase phrase = new Phrase();
            phrase.Add(c1);

            for (int i = 1; i < 400; i++)
            {
                doc.Add(phrase);
            }

         }
        catch
        {

        }
        finally
        {
            doc.Close();
        }
        doc.Close();

*/
            #endregion

            #region LISTS
            /*
        string path = Server.MapPath("PDFs");
        Document doc = new Document(PageSize.LETTER);

        try
        {
            PdfWriter.GetInstance(doc, new FileStream(path + "/Lists.pdf", FileMode.Create));
            doc.Open();

            List list = new List(List.UNORDERED);

            list.Add("One");
            list.Add("Two");
            list.Add("Three");
            list.Add("Four");
            list.Add("Five");
            Paragraph paragraph = new Paragraph();
            string text = "Lists";
            paragraph.Add(text);
            doc.Add(paragraph);
            doc.Add(list);
        }
        catch (DocumentException dex)
        {
            Response.Write(dex.Message);
        }
        catch (IOException ioex)
        {
            Response.Write(ioex.Message);
        }
        finally
        {
            doc.Close();
        }
*/
            #endregion

            #region COORDINATE TEXT?
            /*
        string pdfpath = Server.MapPath("PDFs");
        Document doc = new Document(PageSize.LETTER);

        try
        {
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfpath + "/Graphics.pdf", FileMode.Create));
            doc.Open();

            PdfContentByte cb = writer.DirectContent;

            cb.MoveTo(doc.PageSize.Width / 2, doc.PageSize.Height / 2);
            cb.LineTo(doc.PageSize.Width / 2, doc.PageSize.Height);
            cb.Stroke();
            cb.MoveTo(0, doc.PageSize.Height / 2);
            cb.LineTo(doc.PageSize.Width, doc.PageSize.Height / 2);
            cb.Stroke();


            //top left corner square
            cb.MoveTo(100f, 700f);
            cb.LineTo(200f, 700f);
            cb.LineTo(200f, 600f);
            cb.LineTo(100f, 600f);
            cb.ClosePath();
            cb.Stroke();

            //easier way to make a square in right corner
            cb.Rectangle(doc.PageSize.Width - 200f, 600f, 100f, 100f);
            cb.Stroke();

            cb.SetColorStroke(new CMYKColor(1f, 0f, 0f, 0f));
            cb.SetColorFill(new CMYKColor(0f, 0f, 1f, 0f));

            ////Path closed and stroked square
            cb.MoveTo(70, 200);
            cb.LineTo(170, 200);
            cb.LineTo(170, 300);
            cb.LineTo(70, 300);
            cb.ClosePathStroke();

            //Filled, but not stroked or closed square
            cb.MoveTo(190, 200);
            cb.LineTo(290, 200);
            cb.LineTo(290, 300);
            cb.LineTo(190, 300);
            cb.Fill();

            //Path closed, stroked and filled square
            cb.MoveTo(430, 200);
            cb.LineTo(530, 200);
            cb.LineTo(530, 300);
            cb.LineTo(430, 300);
            cb.ClosePathFillStroke();

        }
        catch
        {

        }
        finally
        {
            doc.Close();
        }
*/
            #endregion

            #region IMAGE TO PDF
            /*
        string pdfpath = Server.MapPath("PDFs");
        string imagepath = Server.MapPath("Images");
        Document doc = new Document();
        try
        {

            PdfWriter.GetInstance(doc, new FileStream(pdfpath + "/SOAP_updated.pdf", FileMode.Create));
            doc.Open();

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font times = new Font(bfTimes, 12, Font.NORMAL);
            doc.Add(new Paragraph("OSU Anesthesia Vet SOAP Form:", times));

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagepath + "/AnesthesiaSoapPicture2.jpg");
            doc.Add(img);
        }
        catch (Exception ex)
        {
            //Log error?
        }
        finally
        {
            doc.Close();
        }

  */
            #endregion

            #region PAGE LAYOUT
            /*
        string pdfpath = Server.MapPath("PDFs");
        string imagepath = Server.MapPath("Images");
        FontFactory.RegisterDirectory("C:\\WINDOWS\\Fonts");

        Document doc = new Document();

        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        iTextSharp.text.Font font1 = new iTextSharp.text.Font(bfTimes, 9, iTextSharp.text.Font.NORMAL);
        BaseFont bfHal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
        iTextSharp.text.Font font2 = new iTextSharp.text.Font(bfHal, 12, iTextSharp.text.Font.BOLD);


        doc.SetMargins(45f, 45f, 60f, 60f);
        try
        {
            FileStream output = new FileStream(pdfpath + "/IrregularColumns.pdf", FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);
            doc.Open();
            PdfContentByte cb = writer.DirectContent;
            ColumnText ct = new ColumnText(cb);
            ct.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph heading = new Paragraph("Chapter 1", font1);
            heading.Leading = 40f;
            doc.Add(heading);
            iTextSharp.text.Image L = iTextSharp.text.Image.GetInstance(imagepath + "/l.gif");
            L.SetAbsolutePosition(doc.Left, doc.Top - 180);
            doc.Add(L);

            ct.AddText(new Phrase("orem ipsum dolor sit amet, consectetuer adipiscing elit. Suspendisse blandit blandit turpis. Nam in lectus ut dolor consectetuer bibendum. Morbi neque ipsum, laoreet id; dignissim et, viverra id, mauris. Nulla mauris elit, consectetuer sit amet, accumsan eget, congue ac, libero. Vivamus suscipit. Nunc dignissim consectetuer lectus. Fusce elit nisi; commodo non, facilisis quis, hendrerit eu, dolor? Suspendisse eleifend nisi ut magna. Phasellus id lectus! Vivamus laoreet enim et dolor. Integer arcu mauris, ultricies vel, porta quis, venenatis at, libero. Donec nibh est, adipiscing et, ullamcorper vitae, placerat at, diam. Integer ac turpis vel ligula rutrum auctor! Morbi egestas erat sit amet diam. Ut ut ipsum? Aliquam non sem. Nulla risus eros, mollis quis, blandit ut; luctus eget, urna. Vestibulum vestibulum dapibus erat. Proin egestas leo a metus?\n\n", font2));
            ct.AddText(new Phrase("Vivamus enim nisi, mollis in, sodales vel, convallis a, augue? Proin non enim. Nullam elementum euismod erat. Aliquam malesuada eleifend quam! Nulla facilisi. Aenean ut turpis ac est tempor malesuada. Maecenas scelerisque orci sit amet augue laoreet tempus. Duis interdum est ut eros. Fusce dictum dignissim elit. Morbi at dolor. Fusce magna. Nulla tellus turpis, mattis ut, eleifend a, adipiscing vitae, mauris. Pellentesque mattis lobortis mi.\n\n", font2));
            ct.AddText(new Phrase("Nullam sit amet metus scelerisque diam hendrerit porttitor. Aenean pellentesque, lorem a consectetuer consectetuer, nunc metus hendrerit quam, mattis ultrices lorem tellus lacinia massa. Aliquam sit amet odio. Proin mauris. Integer dictum quam a quam accumsan lacinia. Pellentesque pulvinar feugiat eros. Suspendisse rhoncus. Sed consectetuer leo eu nisi. Suspendisse massa! Sed suscipit lacus sit amet elit! Aliquam sollicitudin condimentum turpis. Nunc ut augue! Maecenas eu eros. Morbi in urna consectetuer ipsum vehicula tristique.\n\n", font2));
            ct.AddText(new Phrase("Donec imperdiet purus vel ligula. Vestibulum tempor, odio ut scelerisque eleifend, nulla sapien laoreet dui; vel aliquam arcu libero eu ante. Curabitur rutrum tristique mi. Sed lobortis iaculis arcu. Suspendisse mauris. Aliquam metus lacus, elementum quis, mollis non, consequat nec, tortor.\n", font2));
            ct.AddText(new Phrase("Quisque id diam. Ut egestas leo a elit. Nulla in metus. Aliquam iaculis turpis non augue. Donec a nunc? Phasellus eu eros. Nam luctus. Duis eu mi. Ut mollis. Nulla facilisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Aenean pede. Nulla facilisi. Vestibulum mattis adipiscing nulla. Praesent orci ante, mattis in, cursus eget, posuere sed, mauris.\n\n", font2));
            ct.AddText(new Phrase("Nulla facilisi. Nunc accumsan risus aliquet quam. Nam pellentesque! Aenean porttitor. Aenean congue ullamcorper velit. Phasellus suscipit placerat tellus. Vivamus diam odio, tempus quis, suscipit a, dictum eu; lectus. Sed vel nisl. Ut interdum urna eu nibh. Praesent vehicula, orci id venenatis ultrices, mauris urna mollis lacus, et blandit odio magna at enim. Pellentesque lorem felis, ultrices quis, gravida sed, pharetra vitae, quam. Mauris libero ipsum, pharetra a, faucibus aliquet, pellentesque in, mauris. Cras magna neque, interdum vel, varius nec; vulputate at, erat. Quisque vitae urna. Suspendisse potenti. Nulla luctus purus at turpis! Vestibulum vitae dui. Nullam odio.\n\n", font2));
            ct.AddText(new Phrase("Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed eget mi at sem iaculis hendrerit. Nulla facilisi. Etiam sed elit. In viverra dapibus sapien. Aliquam nisi justo, ornare non, ultricies vitae, aliquam sit amet, risus! Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Phasellus risus. Vestibulum pretium augue non mi. Sed magna. In hac habitasse platea dictumst. Quisque massa. Etiam viverra diam pharetra ante. Phasellus fringilla velit ut odio! Nam nec nulla.\n\n", font2));
            ct.AddText(new Phrase("Integer augue. Morbi orci. Sed quis nibh. Nullam ac magna id leo faucibus ornare. Vestibulum eget lectus sit amet nunc facilisis bibendum. Donec adipiscing convallis mi. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus enim. Mauris ligula lorem, pellentesque quis, semper sed, tristique sit amet, justo. Suspendisse potenti. Proin vitae enim. Morbi et nisi sit amet sapien ve.\n\n", font2));

            float gutter = 15f;
            float colwidth = (doc.Right - doc.Left - gutter) / 2;
            float[] left = { doc.Left + 90f , doc.Top - 80f,
                  doc.Left + 90f, doc.Top - 170f,
                  doc.Left, doc.Top - 170f,
                  doc.Left , doc.Bottom };

            float[] right = { doc.Left + colwidth, doc.Top - 80f,
                    doc.Left + colwidth, doc.Bottom };

            float[] left2 = { doc.Right - colwidth, doc.Top - 80f,
                    doc.Right - colwidth, doc.Bottom };

            float[] right2 = {doc.Right, doc.Top - 80f,
                    doc.Right, doc.Bottom };

            int status = 0;
            int i = 0;
            //Checks the value of status to determine if there is more text
            //If there is, status is 2, which is the value of NO_MORE_COLUMN
            while (ColumnText.HasMoreText(status))
            {
                if (i == 0)
                {
                    //Writing the first column
                    ct.SetColumns(left, right);
                    i++;
                }
                else
                {
                    //write the second column
                    ct.SetColumns(left2, right2);
                }
                //Needs to be here to prevent app from hanging
                ct.YLine = doc.Top - 80f;
                //Commit the content of the ColumnText to the document
                //ColumnText.Go() returns NO_MORE_TEXT (1) and/or NO_MORE_COLUMN (2)
                //In other words, it fills the column until it has either run out of column, or text, or both
                status = ct.Go();
            }
        }
        catch (Exception ex)
        {
            //Log(ex.Message);
        }
        finally
        {
            doc.Close();
        }

*/
            #endregion

            #region CUSTOM OFFSET FOR TEXT (WITH WORD WRAP)

        /*      
         
        Document doc = new Document(PageSize.LETTER);
        doc.SetMargins(0f, 0f, 0f, 0f);

        MemoryStream memStream = new MemoryStream();

        try
        {
            string path = Server.MapPath("PDFs");
            string imagepath = Server.MapPath("images");

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path + "/SOAP.pdf", FileMode.Create));
            doc.Open();

            PdfContentByte cb = writer.DirectContent;
            ColumnText ct = new ColumnText(cb);
            Phrase myText = new Phrase("This is a text phrase");
            string hi = "Hi";
            Phrase myText2 = new Phrase("This is another test phrase");
            Phrase myText3 = new Phrase(hi);

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagepath + "/Test.jpg");
            doc.Add(img);



//parameters of SetSimpleColumn: (Phrase, leftmargin coordinate, bottommargin coordinate, box width, box height, line height, alignment)     
            //NOTE: all dimensions start at the BOTTOM LEFT of the PDF... why? I dont fucking know, its retarded.
            //NOTE: bottommargin determines x coordinate

            ct.SetSimpleColumn(myText, 150, 100, 300, 700, 15, Element.ALIGN_LEFT);
            ct.Go();

            ct.SetSimpleColumn(myText2, 150, 100, 100, 317, 15, Element.ALIGN_LEFT);
            ct.Go();

            ct.SetSimpleColumn(myText3, 300, 100, 400, 317, 15, Element.ALIGN_LEFT);
            ct.Go();

        }

        catch
        {
        }
        finally
        {
        doc.Close();
        }

      */

            #endregion






        }
    }
}