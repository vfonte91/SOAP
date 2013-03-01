using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using SOAP.Models;
using SOAP.Models.Callbacks;

namespace SOAP.Controllers
{
    public class ServiceAdo
    {
        private string connString;

        public ServiceAdo()
        {
            connString = ConfigurationManager.ConnectionStrings["SOAP_DB_CONNECTION"].ConnectionString;
        }

        #region READ
        public List<AdministrationSet> GetAdministrationSets(int patientId)
        {
            List<AdministrationSet> aSets = new List<AdministrationSet>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Abstract database field names as much as possible in case of field name changes
                string sql = BuildAdministrationSQL();
                string from = @"FROM dbo.Administration_Set_To_Patient AS a";
                string where = @" WHERE a.PatientId = @PatientId ";

                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        // Create a callback for everytime you have to read something from database.
                        // We do this, so we only have to change string values in one place if we change things.                      
                        aSets.Add(new AdministrationSetCallback().ProcessRow(read));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return aSets;
        }

        // LazyComponents is an optional enum parameter. They are set, if we want to load extra data from the database.
        // Like loading description from the dropdown_value table. Look in AnesthesiaConcern table for information on enum
        public List<AnesthesiaConcern> GetAnesthesiaConcerns(int patientId, params AnesthesiaConcern.LazyComponents[] lazyComponents)
        {
            List<AnesthesiaConcern> anesthesiaConcerns = new List<AnesthesiaConcern>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = BuildAnesthesiaConcernSQL();

                string from = @"FROM dbo.Anesthesia_Concerns_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (AnesthesiaConcern.LazyComponents a in lazyComponents)
                {
                    if (a == AnesthesiaConcern.LazyComponents.LOAD_CONCERN_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.ConcernId = b.Id ";
                    }
                }

                sql = sql + from + where; 

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        anesthesiaConcerns.Add(new AnestheticConcernCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return anesthesiaConcerns;
        }

        public List<AnestheticPlanInjection> GetAnestheticPlanInjection(int patientId, params AnestheticPlanInjection.LazyComponents[] lazyComponents)
        {
            List<AnestheticPlanInjection> anesPlanInject = new List<AnestheticPlanInjection>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildAnestheticPlanInjectionSQL();

                string from = @"FROM dbo.Anesthetic_Plan_Injection AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (AnestheticPlanInjection.LazyComponents a in lazyComponents)
                {
                    if (a == AnestheticPlanInjection.LazyComponents.LOAD_DRUG_INFORMATION)
                    {
                        sql += @", b.Id b.DoseMinRange, b.DoseMaxRange, b.DoseMax, b.DoseUnits, b.Route, b.Concentration, b.ConcentrationUnits, 
                                   d.CategoryId, d.Label, d.OtherFlag, d.Description";
                        from += @" LEFT OUTER JOIN dbo.Drug_Information as b ON a.DrugId = b.DrugId 
                                   LEFT OUTER JOIN dbo.Dropdown_Types d on d.DrugId = b.DrugId";
                    }

                    else if (a == AnestheticPlanInjection.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId, c.Label, c.OtherFlag, c.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.RouteId = c.Id ";
                    }
                }

                
                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        anesPlanInject.Add(new AnestheticPlanInjectionCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return anesPlanInject;
        }

        public List<AnestheticPlanInhalant> GetAnestheticPlanInhalant(int patientId, params AnestheticPlanInhalant.LazyComponents[] lazyComponents)
        {
            List<AnestheticPlanInhalant> anesPlanInhalant = new List<AnestheticPlanInhalant>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildAnestheticPlanInhalantSQL();

                string from = @"FROM dbo.Anesthetic_Plan_Inhalant AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (AnestheticPlanInjection.LazyComponents a in lazyComponents)
                {
                    if (a == AnestheticPlanInjection.LazyComponents.LOAD_DRUG_INFORMATION)
                    {
                        sql += @", b.Id b.DoseMinRange, b.DoseMaxRange, b.DoseMax, b.DoseUnits, b.Route, b.Concentration, b.ConcentrationUnits, 
                                   d.CategoryId, d.Label, d.OtherFlag, d.Description";
                        from += @" LEFT OUTER JOIN dbo.Drug_Information as b ON a.DrugId = b.DrugId 
                                   LEFT OUTER JOIN dbo.Dropdown_Types d on d.DrugId = b.DrugId";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        anesPlanInhalant.Add(new AnestheticPlanInhalantCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return anesPlanInhalant;
        }

        public List<AnestheticPlanPremedication> GetAnestheticPlanPremedication(int patientId, params AnestheticPlanPremedication.LazyComponents[] lazyComponents)
        {
            List<AnestheticPlanPremedication> anesPlanPremed = new List<AnestheticPlanPremedication>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildAnestheticPlanPremedicationSQL();

                string from = @"FROM dbo.Anesthetic_Plan_Premed AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (AnestheticPlanPremedication.LazyComponents a in lazyComponents)
                {
                    if (a == AnestheticPlanPremedication.LazyComponents.LOAD_DRUG_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.DrugId = b.Id ";
                    }

                    else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId, c.Label, c.OtherFlag, c.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.RouteId = c.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        anesPlanPremed.Add(new AnestheticPlanPremedicationCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return anesPlanPremed;
        }

        public List<Bloodwork> GetBloodwork(int patientId, params Bloodwork.LazyComponents[] lazyComponents)
        {
            List<Bloodwork> bloodworkGroup = new List<Bloodwork>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildBloodworkSQL();

                string from = @"FROM dbo.Bloodwork_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (Bloodwork.LazyComponents a in lazyComponents)
                {
                    if (a == Bloodwork.LazyComponents.LOAD_BLOODWORK_INFO)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.BloodworkId = b.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        bloodworkGroup.Add(new BloodworkCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return bloodworkGroup;
        }

        public ClinicalFindings GetClinicalFindings(int patientId, params ClinicalFindings.LazyComponents[] lazyComponents)
        {
            ClinicalFindings clinincalFindings = new ClinicalFindings();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildClinicalFindingsSQL();

                string from = @"FROM dbo.Clinical_Findings AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (ClinicalFindings.LazyComponents a in lazyComponents)
                {
                    if (a == ClinicalFindings.LazyComponents.LOAD_CARDIAC_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.CardiacAuscultationId = b.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_PHYSICAL_STATUS_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId, c.Label, c.OtherFlag, c.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.PhysicalStatusClassId = c.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_PULSE_QUALITY_WITH_DETAILS)
                    {
                        sql += @", d.CategoryId, d.Label, d.OtherFlag, d.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.PulseQualityId = d.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS)
                    {
                        sql += @", e.CategoryId, e.Label, e.OtherFlag, e.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as e ON a.RespiratoryAuscultationId = e.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        clinincalFindings = new ClinicalFindingsCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return clinincalFindings;
        }

        public List<CurrentMedication> GetCurrentMedications(int patientId, params CurrentMedication.LazyComponents[] lazyComponents)
        {
            List<CurrentMedication> meds = new List<CurrentMedication>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildCurrentMedicationsSQL();

                string from = @"FROM dbo.Current_Medications_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (CurrentMedication.LazyComponents a in lazyComponents)
                {
                    if (a == CurrentMedication.LazyComponents.LOAD_CURRENT_MEDICATIONS_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.MedicationId = b.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        meds.Add(new CurrentMedicationCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return meds;
        }

        public List<DropdownCategory> GetDropdownCategories(params DropdownCategory.LazyComponents[] lazyComponents)
        {
            List<DropdownCategory> dropdowns = new List<DropdownCategory>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildDropdownCategorySQL();

                string from = @"FROM dbo.Dropdown_Categories AS a";

                string where = @"";

                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        dropdowns.Add(new DropdownCategoryCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return dropdowns;
        }

        public List<DropdownValue> GetDropdownCategories(int categoryId, params DropdownValue.LazyComponents[] lazyComponents)
        {
            List<DropdownValue> values = new List<DropdownValue>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildDropdownValueSQL();

                string from = @"FROM dbo.Dropdown_Types AS a";

                string where = @" WHERE a.CategoryId = @CategoryId ";

                sql = sql + from + where;

                foreach (DropdownValue.LazyComponents a in lazyComponents)
                {
                    if (a == DropdownValue.LazyComponents.LOAD_DROPDOWN_CATEGORY)
                    {
                        sql += @", b.ShortName, b.LongName";
                        from += @" INNER JOIN dbo.Dropdown_Categories as b ON a.CategoryId = b.Id ";
                    }
                }

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        values.Add(new DropdownValueCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return values;
        }

        public List<IntraoperativeAnalgesia> GetIntraoperativeAnalgesia(int patientId, params IntraoperativeAnalgesia.LazyComponents[] lazyComponents)
        {
            List<IntraoperativeAnalgesia> intraOp = new List<IntraoperativeAnalgesia>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildIntraoperativeAnalgesiaSQL();

                string from = @"FROM dbo.Intraoperative_Analgesia_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (IntraoperativeAnalgesia.LazyComponents a in lazyComponents)
                {
                    if (a == IntraoperativeAnalgesia.LazyComponents.LOAD_ANALGESIA_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.Analgesia = b.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        intraOp.Add(new IntraoperativeAnalgesiaCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return intraOp;
        }

        public List<IVFluidType> GetIVFluidTypes(int patientId, params IVFluidType.LazyComponents[] lazyComponents)
        {
            List<IVFluidType> ivs = new List<IVFluidType>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildIVFluidTypeSQL();

                string from = @"FROM dbo.IV_Fluid_Type_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (IVFluidType.LazyComponents a in lazyComponents)
                {
                    if (a == IVFluidType.LazyComponents.LOAD_FLUID_TYPE_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.FluidTypeId = b.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        ivs.Add(new IVFluidTypeCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return ivs;
        }

        public List<MaintenanceInhalantDrug> GetMaintenanceInhalantDrugs(int patientId, params MaintenanceInhalantDrug.LazyComponents[] lazyComponents)
        {
            List<MaintenanceInhalantDrug> maintInhalantDrugs = new List<MaintenanceInhalantDrug>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildMaintenanceInhalantDrugSQL();

                string from = @"FROM dbo.Maintenance_Inhalant_Drugs_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (MaintenanceInhalantDrug.LazyComponents a in lazyComponents)
                {
                    if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.DrugId = b.Id ";
                    }
                    else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_SETAILS)
                    {
                        sql += @", c.CategoryId, c.Label, c.OtherFlag, c.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.BreathingSystemId = c.Id ";
                    }
                    else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS)
                    {
                        sql += @", d.CategoryId, d.Label, d.OtherFlag, d.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.BreathingBagSizeId = d.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        maintInhalantDrugs.Add(new MaintenanceInhalantDrugCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return maintInhalantDrugs;
        }

        public List<MaintenanceInjectionDrug> GetMaintenanceInjectionDrugs(int patientId, params MaintenanceInjectionDrug.LazyComponents[] lazyComponents)
        {
            List<MaintenanceInjectionDrug> maintInjectDrugs = new List<MaintenanceInjectionDrug>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildMaintenanceInjectionDrugSQL();

                string from = @"FROM dbo.Maintenance_Injection_Drugs_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (MaintenanceInjectionDrug.LazyComponents a in lazyComponents)
                {
                    if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_DRUG_INFORMATION)
                    {
                        sql += @", b.Id b.DoseMinRange, b.DoseMaxRange, b.DoseMax, b.DoseUnits, b.Route, b.Concentration, b.ConcentrationUnits, 
                                   d.CategoryId, d.Label, d.OtherFlag, d.Description";
                        from += @" LEFT OUTER JOIN dbo.Drug_Information as b ON a.DrugId = b.DrugId 
                                   LEFT OUTER JOIN dbo.Dropdown_Types d on d.DrugId = b.DrugId";
                    }
                    else if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId, c.Label, c.OtherFlag, c.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.RouteOfAdministrationId = c.Id ";
                    }
                }


                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        maintInjectDrugs.Add(new MaintenanceInjectionDrugCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return maintInjectDrugs;
        }

        public List<Monitoring> GetMonitoring(int patientId, params Monitoring.LazyComponents[] lazyComponents)
        {
            List<Monitoring> monitoring = new List<Monitoring>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = BuildMonitoringSQL();

                string from = @"FROM dbo.Monitoring_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (Monitoring.LazyComponents a in lazyComponents)
                {
                    if (a == Monitoring.LazyComponents.LOAD_EQUIPMENT_WITH_DETAIL)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.EquipmentId = b.Id ";
                    }
                }

                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        monitoring.Add(new MonitoringCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return monitoring;
        }

        public List<OtherAnestheticDrug> GetOtherAnestheticDrugs(int patientId, params OtherAnestheticDrug.LazyComponents[] lazyComponents)
        {
            List<OtherAnestheticDrug> otherAnesDrugs = new List<OtherAnestheticDrug>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = BuildOtherAnestheticDrugSQL();

                string from = @"FROM dbo.Other_Anesthetic_Drugs_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (OtherAnestheticDrug.LazyComponents a in lazyComponents)
                {
                    if (a == OtherAnestheticDrug.LazyComponents.LOAD_DRUG_WITH_DETAIL)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.DrugId = b.Id ";
                    }
                }

                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        otherAnesDrugs.Add(new OtherAnestheticDrugCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return otherAnesDrugs;
        }

        public PatientInformation GetPatientInformation(int patientId, params PatientInformation.LazyComponents[] lazyComponents)
        {
            PatientInformation patientInfo = new PatientInformation();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = BuildPatientInformationSQL();

                string from = @"FROM dbo.Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (PatientInformation.LazyComponents a in lazyComponents)
                {
                    if (a == PatientInformation.LazyComponents.LOAD_CLINICIAN_DETAIL)
                    {
                        sql += @", b.Username, b.FullName, b.Email ";
                        from += @" LEFT OUTER JOIN dbo.ASF_User as b ON a.ClinicianId = b.UserId ";
                    }
                    else if (a == PatientInformation.LazyComponents.LOAD_STUDENT_DETAIL)
                    {
                        sql += @", c.Username, c.FullName, c.Email ";
                        from += @" LEFT OUTER JOIN dbo.ASF_User as c ON a.StudentId = c.UserId ";
                    }
                    else if (a == PatientInformation.LazyComponents.LOAD_POSTOP_PAIN_DETAIL)
                    {
                        sql += @", d.CategoryId, d.Label, d.OtherFlag, d.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.PostOpPainAssessmentId = d.Id ";
                    }
                    else if (a == PatientInformation.LazyComponents.LOAD_PREOP_PAIN_DETAIL)
                    {
                        sql += @", e.CategoryId, e.Label, e.OtherFlag, e.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as e ON a.PreOpPainAssessmentId = e.Id ";
                    }

                    else if (a == PatientInformation.LazyComponents.LOAD_TEMPERAMENT_DETAIL)
                    {
                        sql += @", f.CategoryId, f.Label, f.OtherFlag, f.Description";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as f ON a.TemperamentId = f.Id ";
                    }
                }

                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        patientInfo = new PatientInformationCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return patientInfo;
        }

        public List<PriorAnesthesia> GetPriorAnesthesia(int patientId)
        {
            List<PriorAnesthesia> priorAnes = new List<PriorAnesthesia>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Abstract database field names as much as possible in case of field name changes
                string sql = BuildPriorAnesthesiaSQL();
                string from = @"FROM dbo.Prior_Anesthesia_To_Patient AS a";
                string where = @" WHERE a.PatientId = @PatientId ";

                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        // Create a callback for everytime you have to read something from database.
                        // We do this, so we only have to change string values in one place if we change things.                      
                        priorAnes.Add(new PriorAnesthesiaCallback().ProcessRow(read));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return priorAnes;
        }

        public List<Procedure> GetAnesthesiaConcerns(int patientId, params Procedure.LazyComponents[] lazyComponents)
        {
            List<Procedure> procedures = new List<Procedure>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = BuildProcedureSQL();

                string from = @"FROM dbo.Procedure_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (Procedure.LazyComponents a in lazyComponents)
                {
                    if (a == Procedure.LazyComponents.LOAD_PROCEDURE_WITH_DETAIL)
                    {
                        sql += @", b.CategoryId, b.Label, b.OtherFlag, b.Description";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.ProcedureId = b.Id ";
                    }
                }

                sql = sql + from + where;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        procedures.Add(new ProcedureCallback().ProcessRow(read, lazyComponents));
                    }
                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
            }
            return procedures;
        }

        #endregion

        #region SQL_STATEMENTs
        private string BuildAdministrationSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.MiniDripFlag, a.MaxiDripFlag ";
        }

        private string BuildAnesthesiaConcernSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.ConcernId ";
        }

        private string BuildAnestheticPlanInjectionSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.RouteId, a.Dosage ";
        }

        private string BuildAnestheticPlanInhalantSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.Dose, a.FlowRate ";
        }

        private string BuildAnestheticPlanPremedicationSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.RouteId, a.Dosage ";
        }

        private string BuildBloodworkSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.BloodworkId, a.Value ";
        }

        private string BuildClinicalFindingsSQL()
        {
            return @"SELECT a.Id, a.PatientId, PreOpPainAssessmentId, PostOpPainAssessmentId, Temperature ";
        }

        private string BuildCurrentMedicationsSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.MedicationId ";
        }

        private string BuildDropdownCategorySQL()
        {
            return @"SELECT a.Id, a.ShortName, a.LongName ";
        }

        private string BuildDropdownValueSQL()
        {
            return @"SELECT a.Id, a.CategoryId, a.Label, a.OtherFlag, a.Description ";
        }

        private string BuildIntraoperativeAnalgesiaSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.AnalgesiaId ";
        }

        private string BuildIVFluidTypeSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.FluidTypeId, a.Dose ";
        }

        private string BuildMaintenanceInhalantDrugSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.InductionReqFlag, a.InductionDose, a.InductionOxygenFlowRate, 
                a.MaintenanceReqFlag, a.MaintenanceDose, a.MaintenanceOxygenFlowRate, a.EquipmentReqFlag, a.BreathingSystemId, a.BreathingBagSizeId ";
        }

        private string BuildMaintenanceInjectionDrugSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.RouteOfAdministrationId, a.Dosage ";
        }

        private string BuildMonitoringSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.EquipmentId ";
        }

        private string BuildOtherAnestheticDrugSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId ";
        }

        private string BuildPatientInformationSQL()
        {
            return @"SELECT a.PatientId, a.StudentId, a.ClinicianId, a.FormCompleted, a.TemperamentId, a.DateSeenOn, a. CageOrStallNumber, 
                    a.BodyWeight, a.AgeInYears, a.AgeInMonths, a.PreOpPainAssessmentId, a.PostOpPainAssessmentId ";
        }

        private string BuildPriorAnesthesiaSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DateOfProblem, a.Problem ";
        }

        private string BuildProcedureSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.ProcedureId ";
        }
        #endregion
    }
}