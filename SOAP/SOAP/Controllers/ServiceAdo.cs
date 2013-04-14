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

        #region Login

        public ASFUser DoLogin(MembershipInfo user)
        {
            ASFUser singleUser = new ASFUser();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // If user has correct password, then select user database
                string sql = BuildASFUserSQL();
                string sqlMember = @"SELECT a.UserId FROM dbo.aspnet_Membership as b WHERE b.Username = @Username AND b.Password = @Password";

                string fromUser = @"FROM dbo.ASF_User AS a";
                string whereUser = @" WHERE a.UserId = (" + sqlMember + ")";

                sql = sql + fromUser + whereUser;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.Int).Value = user.Username;
                cmd.Parameters.Add("@Password", SqlDbType.Int).Value = user.Password;

                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        singleUser = new ASFUserCallback().ProcessRow(read);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return singleUser;
        }

        public ASFUser GetUser(MembershipInfo user)
        {
            ASFUser singleUser = new ASFUser();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // If user has correct password, then select user database
                string sql = BuildASFUserSQL() + ", b.Password ";

                string fromUser = @"FROM dbo.ASF_User AS a INNER JOIN dbo.aspnet_Membership as b ON a.Username = b.Username ";
                string whereUser = @"WHERE a.Username = @Username";

                sql = sql + fromUser + whereUser;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;

                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        singleUser = new ASFUserCallback().ProcessRow(read);
                        singleUser.Member.Password = read["Password"].ToString() ;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return singleUser;
        }

        public string GetSecurityQuestion(string username)
        {
            string secQuest = "";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // If user has correct password, then select user database
                string sql = @"SELECT SecurityQuestion FROM dbo.aspnet_Membership WHERE Username = @Username ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;

                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        secQuest = read["SecurityQuestion"].ToString();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return secQuest;
        }

        public bool CheckSecurityAnswer(string username, string answer)
        {
            bool valid = false;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // If user has correct password, then select user database
                string sql = @"SELECT SecurityAnswer FROM dbo.aspnet_Membership WHERE Username = @Username ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;

                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        string actualAnswer = read["SecurityAnswer"].ToString();
                        valid = (answer.Equals(actualAnswer));
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return valid;
        }

        public bool CheckPassword(string username, string password)
        {
            bool valid = false;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // If user has correct password, then select user database
                string sql = @"SELECT Password FROM dbo.aspnet_Membership WHERE Username = @Username ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;

                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        string actualPassword = read["Password"].ToString();
                        valid = PasswordHash.ValidatePassword(password, actualPassword);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return valid;
        }

        #endregion

        #region READ

        public List<ASFUser> GetASFUsers()
        {
            List<ASFUser> users = new List<ASFUser>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"SELECT a.Username as 'a.Username', a.FullName as 'a.FullName', 
                               a.IsAdmin as 'a.IsAdmin', a.Email as 'a.Email' FROM dbo.ASF_User as a";
                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        users.Add(new ASFUserCallback().ProcessRow(read));
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return users;
        }

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
                catch (Exception e)
                {
                    throw e;
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description',
                                    b.Concentration as 'b.Concentration' ";
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
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return anesthesiaConcerns;
        }

        public AnestheticPlanInjection GetAnestheticPlanInjection(int patientId, params AnestheticPlanInjection.LazyComponents[] lazyComponents)
        {
            AnestheticPlanInjection anesPlanInject = new AnestheticPlanInjection();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildAnestheticPlanInjectionSQL();

                string from = @"FROM dbo.Anesthetic_Plan_Injection AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (AnestheticPlanInjection.LazyComponents a in lazyComponents)
                {
                    if (a == AnestheticPlanInjection.LazyComponents.LOAD_DRUG_INFORMATION)
                    {
                        sql += @", b.Id as 'b.Id', b.DoseMinRange as 'b.DoseMinRange', b.DoseMaxRange as 'b.DoseMaxRange', b.DoseMax as 'b.DoseMax', 
                              b.DoseUnits as 'b.DoseUnits', b.Route as 'b.Route', b.Concentration as 'b.Concentration', b.ConcentrationUnits as 'b.ConcentrationUnits', 
                                    d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description',
                                    d.Concentration as 'd.Concentration', d.MaxDosage as 'd.MaxDosage' ";
                        from += @" LEFT OUTER JOIN dbo.Drug_Information as b ON a.DrugId = b.DrugId 
                                   LEFT OUTER JOIN dbo.Dropdown_Types d on d.Id = b.DrugId";
                    }

                    else if (a == AnestheticPlanInjection.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description',
                                    c.Concentration as 'c.Concentration' ";
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
                        anesPlanInject = new AnestheticPlanInjectionCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return anesPlanInject;
        }

        public AnestheticPlanInhalant GetAnestheticPlanInhalant(int patientId, params AnestheticPlanInhalant.LazyComponents[] lazyComponents)
        {
            AnestheticPlanInhalant anesPlanInhalant = new AnestheticPlanInhalant();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildAnestheticPlanInhalantSQL();

                string from = @"FROM dbo.Anesthetic_Plan_Inhalant AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (AnestheticPlanInhalant.LazyComponents a in lazyComponents)
                {
                    if (a == AnestheticPlanInhalant.LazyComponents.LOAD_DRUG_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description',
                                b.Concentration as 'b.Concentration', b.MaxDosage as 'b.MaxDosage' ";
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
                        anesPlanInhalant = new AnestheticPlanInhalantCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return anesPlanInhalant;
        }

        public AnestheticPlanPremedication GetAnestheticPlanPremedication(int patientId, params AnestheticPlanPremedication.LazyComponents[] lazyComponents)
        {
            AnestheticPlanPremedication anesPlanPremed = new AnestheticPlanPremedication();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildAnestheticPlanPremedicationSQL();

                string from = @"FROM dbo.Anesthetic_Plan_Premed AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (AnestheticPlanPremedication.LazyComponents a in lazyComponents)
                {
                    if (a == AnestheticPlanPremedication.LazyComponents.LOAD_SEDATIVE_DRUG_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description',
                                    b.Concentration as 'b.Concentration', b.MaxDosage as 'b.MaxDosage' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.SedativeDrugId = b.Id ";
                    }
                    else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_OPIOID_DRUG_WITH_DETAILS)
                        {
                            sql += @", d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description',
                                    d.Concentration as 'd.Concentration', d.MaxDosage as 'd.MaxDosage' ";
                            from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.OpioidDrugId = d.Id ";
                        }

                    else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_ANTICHOLINERGIC_DRUG_WITH_DETAILS)
                    {
                        sql += @", e.CategoryId as 'e.CategoryId', e.Label as 'e.Label', e.OtherFlag as 'e.OtherFlag', e.Description as 'e.Description',
                                    e.Concentration as 'e.Concentration', e.MaxDosage as 'e.MaxDosage' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as e ON a.AnticholinergicDrugId = e.Id ";
                    }
                    else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description',
                                    c.Concentration as 'c.Concentration' ";
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
                        anesPlanPremed = new AnestheticPlanPremedicationCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return anesPlanPremed;
        }

        public List<Bloodwork> GetBloodwork(int patientId)
        {
            List<Bloodwork> bloodworkGroup = new List<Bloodwork>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildBloodworkSQL();

                string from = @"FROM dbo.Bloodwork_To_Patient AS a";

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
                        bloodworkGroup.Add(new BloodworkCallback().ProcessRow(read));
                    }
                }
                catch (Exception e)
                {
                    throw e;
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description',
                                    b.Concentration as 'b.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.CardiacAuscultationId = b.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_PHYSICAL_STATUS_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description',
                                    c.Concentration as 'c.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.PhysicalStatusClassId = c.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_PULSE_QUALITY_WITH_DETAILS)
                    {
                        sql += @", d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description',
                                    d.Concentration as 'd.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.PulseQualityId = d.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS)
                    {
                        sql += @", e.CategoryId as 'e.CategoryId', e.Label as 'e.Label', e.OtherFlag as 'e.OtherFlag', e.Description as 'e.Description',
                                    e.Concentration as 'e.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as e ON a.RespiratoryAuscultationId = e.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_MUCOUS_MEMBRANE_WITH_DETAILS)
                    {
                        sql += @", f.CategoryId as 'f.CategoryId', f.Label as 'f.Label', f.OtherFlag as 'f.OtherFlag', f.Description as 'f.Description',
                                    f.Concentration as 'f.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as f ON a.MucousMembraneColorId = f.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_CAP_REFILL_WITH_DETAILS)
                    {
                        sql += @", g.CategoryId as 'g.CategoryId', g.Label as 'g.Label', g.OtherFlag as 'g.OtherFlag', g.Description as 'g.Description',
                                    g.Concentration as 'g.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as g ON a.CapillaryRefillTimeId = g.Id ";
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
                catch (Exception e)
                {
                    throw e;
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description',
                                    b.Concentration as 'b.Concentration' ";
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
                catch (Exception e)
                {
                    throw e;
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

                sql = sql + from + where + " ORDER BY a.SHORTNAME ";

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
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return dropdowns;
        }

        public List<DropdownValue> GetDropdownCategoryValues(int categoryId, params DropdownValue.LazyComponents[] lazyComponents)
        {
            List<DropdownValue> values = new List<DropdownValue>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildDropdownValueSQL();

                string from = @"FROM dbo.Dropdown_Types AS a";

                string where = @" WHERE a.CategoryId = @CategoryId AND a.OtherFlag = 'N' ORDER BY a.LABEL ";

                sql = sql + from + where;

                foreach (DropdownValue.LazyComponents a in lazyComponents)
                {
                    if (a == DropdownValue.LazyComponents.LOAD_DROPDOWN_CATEGORY)
                    {
                        sql += @", b.ShortName as 'b.ShortName', b.LongName as 'b.LongName'";
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
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return values;
        }

        public List<Patient> GetForms(ASFUser user)
        {
            List<Patient> pats = new List<Patient>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = @"SELECT PatientId, DateSeenOn FROM dbo.Patient WHERE StudentId = @Username ORDER BY DateSeenOn DESC ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        Patient pat = new Patient();
                        pat.PatientId = Convert.ToInt32(read["PatientId"]);
                        pat.PatientInfo.DateSeenOn = Convert.ToDateTime(read["DateSeenOn"]);
                        pats.Add(pat);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return pats;
        }

        public MaintenanceInhalantDrug GetMaintenanceInhalantDrugs(int patientId, params MaintenanceInhalantDrug.LazyComponents[] lazyComponents)
        {
            MaintenanceInhalantDrug maintInhalantDrugs = new MaintenanceInhalantDrug();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildMaintenanceInhalantDrugSQL();

                string from = @"FROM dbo.Maintenance_Inhalant_Drugs_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (MaintenanceInhalantDrug.LazyComponents a in lazyComponents)
                {
                    if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_DRUG_WITH_DETAILS)
                    {
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description', 
                                    b.Concentration as 'b.Concentration', b.MaxDosage as 'b.MaxDosage' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.DrugId = b.Id ";
                    }
                    else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description',
                                    c.Concentration as 'c.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.BreathingSystemId = c.Id ";
                    }
                    else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS)
                    {
                        sql += @", d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description', 
                                    d.Concentration as 'd.Concentration' ";
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
                        maintInhalantDrugs = new MaintenanceInhalantDrugCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return maintInhalantDrugs;
        }

        public MaintenanceInjectionDrug GetMaintenanceInjectionDrugs(int patientId, params MaintenanceInjectionDrug.LazyComponents[] lazyComponents)
        {
            MaintenanceInjectionDrug maintInjectDrugs = new MaintenanceInjectionDrug();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildMaintenanceInjectionDrugSQL();

                string from = @"FROM dbo.Maintenance_Injection_Drugs_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (MaintenanceInjectionDrug.LazyComponents a in lazyComponents)
                {
                    if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_DRUG_INFORMATION)
                    {
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description', 
                                    b.Concentration as 'b.Concentration', b.MaxDosage as 'b.MaxDosage' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types b on b.Id = a.DrugId";
                    }
                    else if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description', 
                                    c.Concentration as 'c.Concentration' ";
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
                        maintInjectDrugs = new MaintenanceInjectionDrugCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return maintInjectDrugs;
        }

        public MaintenanceOther GetMaintenanceOther(int patientId, params MaintenanceOther.LazyComponents[] lazyComponents)
        {
            MaintenanceOther maintOther = new MaintenanceOther();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string sql = BuildMaintenanceOtherSQL();

                string from = @"FROM dbo.Maintenance_Other_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (MaintenanceOther.LazyComponents a in lazyComponents)
                {
                     if (a == MaintenanceOther.LazyComponents.LOAD_INTRAOP_WITH_DETAILS)
                    {
                        sql += @", e.CategoryId as 'e.CategoryId', e.Label as 'e.Label', e.OtherFlag as 'e.OtherFlag', e.Description as 'e.Description', 
                                    e.Concentration as 'e.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as e ON a.IntraoperativeAnalgesiaId = e.Id ";
                    }
                    else if (a == MaintenanceOther.LazyComponents.LOAD_IV_WITH_DETAILS)
                    {
                        sql += @", f.CategoryId as 'f.CategoryId', f.Label as 'f.Label', f.OtherFlag as 'f.OtherFlag', f.Description as 'f.Description', 
                                    f.Concentration as 'f.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as f ON a.IVFluidTypeId = f.Id ";
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
                        maintOther = new MaintenanceOtherCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return maintOther;
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description',
                                    b.Concentration as 'b.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.EquipmentId = b.Id ";
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
                catch (Exception e)
                {
                    throw e;
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description', 
                                b.Concentration as 'b.Concentration', b.MaxDosage as 'b.MaxDosage' ";
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
                catch (Exception e)
                {
                    throw e;
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
                        sql += @", b.Username as 'b.Username', b.FullName as 'b.FullName', b.Email as 'b.Email' ";
                        from += @" LEFT OUTER JOIN dbo.ASF_User as b ON a.ClinicianId = b.Username ";
                    }
                    else if (a == PatientInformation.LazyComponents.LOAD_STUDENT_DETAIL)
                    {
                        sql += @", c.Username as 'c.Username', c.FullName as 'c.FullName', c.Email as 'c.Email' ";
                        from += @" LEFT OUTER JOIN dbo.ASF_User as c ON a.StudentId = c.Username ";
                    }
                    else if (a == PatientInformation.LazyComponents.LOAD_POSTOP_PAIN_DETAIL)
                    {
                        sql += @", d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description', 
                                    d.Concentration as 'd.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.PostOpPainAssessmentId = d.Id ";
                    }
                    else if (a == PatientInformation.LazyComponents.LOAD_PREOP_PAIN_DETAIL)
                    {
                        sql += @", e.CategoryId as 'e.CategoryId', e.Label as 'e.Label', e.OtherFlag as 'e.OtherFlag', e.Description as 'e.Description',
                                    e.Concentration as 'e.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as e ON a.PreOpPainAssessmentId = e.Id ";
                    }

                    else if (a == PatientInformation.LazyComponents.LOAD_TEMPERAMENT_DETAIL)
                    {
                        sql += @", f.CategoryId as 'f.CategoryId', f.Label as 'f.Label', f.OtherFlag as 'f.OtherFlag', f.Description as 'f.Description',
                                    f.Concentration as 'f.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as f ON a.TemperamentId = f.Id ";
                    }

                    else if (a == PatientInformation.LazyComponents.LOAD_PROCEDURE_DETAIL)
                    {
                        sql += @", g.CategoryId as 'g.CategoryId', g.Label as 'g.Label', g.OtherFlag as 'g.OtherFlag', g.Description as 'g.Description',
                                    g.Concentration as 'g.Concentration' ";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as g ON a.ProcedureId = g.Id ";
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
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return patientInfo;
        }

        public List<Patient> GetPatientWithUserId(ASFUser user)
        {
            List<Patient> pats = new List<Patient>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"SELECT PatientId FROM dbo.Patient WHERE StudentId = @StudentId";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@StudentId", SqlDbType.NVarChar).Value = user.Username;
                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        Patient pat = new Patient();
                        pat.PatientId = Convert.ToInt32(read["PatientId"].ToString());
                        pats.Add(pat);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return pats;
        }

        public PriorAnesthesia GetPriorAnesthesia(int patientId)
        {
            PriorAnesthesia priorAnes = new PriorAnesthesia();
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
                        priorAnes = new PriorAnesthesiaCallback().ProcessRow(read);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return priorAnes;
        }

        public Procedure GetProcedure(int patientId, params Procedure.LazyComponents[] lazyComponents)
        {
            Procedure procedures = new Procedure();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = BuildProcedureSQL();

                string from = @"FROM dbo.Procedure_To_Patient AS a";

                string where = @" WHERE a.PatientId = @PatientId ";

                foreach (Procedure.LazyComponents a in lazyComponents)
                {
                    if (a == Procedure.LazyComponents.LOAD_PROCEDURE_WITH_DETAIL)
                    {
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description',
                                    b.Concentration as 'b.Concentration' ";
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
                        procedures = new ProcedureCallback().ProcessRow(read, lazyComponents);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return procedures;
        }

        #endregion

        #region CREATE

        public void CreateAdministrationSet(AdministrationSet adminSet)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Administration_Set_To_Patient (
                            PatientId, MiniDripFlag, MaxiDripFlag
                            ) VALUES (
                            @PatientId, @MiniDripFlag, @MaxiDripFlag
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = adminSet.PatientId;
                if (adminSet.MiniDripFlag == -1)
                    cmd.Parameters.Add("@MiniDripFlag", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MiniDripFlag", SqlDbType.Int).Value = adminSet.MiniDripFlag;

                if (adminSet.MaxiDripFlag == -1)
                    cmd.Parameters.Add("@MaxiDripFlag", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaxiDripFlag", SqlDbType.Int).Value = adminSet.MaxiDripFlag;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateAnesthesiaConcern(AnesthesiaConcern aConcern)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Anesthesia_Concerns_To_Patient (
                            PatientId, ConcernId
                            ) VALUES (
                            @PatientId, @ConcernId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = aConcern.PatientId;
                cmd.Parameters.Add("@ConcernId", SqlDbType.Int).Value = aConcern.Concern.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateAnestheticPlanInhalant(AnestheticPlanInhalant inhalant)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Anesthetic_Plan_Inhalant (
                            PatientId, DrugId, Percentage, FlowRate
                            ) VALUES (
                            @PatientId, @DrugId, @Percentage, @FlowRate
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = inhalant.PatientId;
                if (inhalant.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = inhalant.Drug.Id;
                if (inhalant.Percentage == 0.0M)
                    cmd.Parameters.Add("@Percentage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Percentage", SqlDbType.Decimal).Value = inhalant.Percentage;
                if (inhalant.FlowRate == 0.0M)
                    cmd.Parameters.Add("@FlowRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@FlowRate", SqlDbType.Decimal).Value = inhalant.FlowRate;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateAnestheticPlanInjection(AnestheticPlanInjection injection)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Anesthetic_Plan_Injection (
                            PatientId, DrugId, RouteId, Dosage
                            ) VALUES (
                            @PatientId, @DrugId, @RouteId, @Dosage
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = injection.PatientId;
                if (injection.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = injection.Drug.Id;
                if (injection.Route.Id == -1)
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = injection.Route.Id;
                if (injection.Dosage == 0.0M)
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = injection.Dosage;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateAnestheticPlanPremedication(AnestheticPlanPremedication premed)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Anesthetic_Plan_Premed (
                            PatientId, RouteId, SedativeDosage, SedativeDrugId, OpioidDosage, OpioidDrugId, AnticholinergicDrugId, AnticholinergicDosage, KetamineDosage
                            ) VALUES (
                            @PatientId, @RouteId, @SedativeDosage, @SedativeDrugId, @OpioidDosage, @OpioidDrugId, @AnticholinergicDrugId, @AnticholinergicDosage, @KetamineDosage
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = premed.PatientId;
                if (premed.SedativeDrug.Id == -1)
                    cmd.Parameters.Add("@SedativeDrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@SedativeDrugId", SqlDbType.Int).Value = premed.SedativeDrug.Id;
                if (premed.SedativeDosage == 0.0M)
                    cmd.Parameters.Add("@SedativeDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@SedativeDosage", SqlDbType.Decimal).Value = premed.SedativeDosage;
                if (premed.OpioidDosage == 0.0M)
                    cmd.Parameters.Add("@OpioidDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OpioidDosage", SqlDbType.Decimal).Value = premed.OpioidDosage;
                if (premed.OpioidDrug.Id == -1)
                    cmd.Parameters.Add("@OpioidDrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OpioidDrugId", SqlDbType.Int).Value = premed.OpioidDrug.Id;
                if (premed.AnticholinergicDrug.Id == -1)
                    cmd.Parameters.Add("@AnticholinergicDrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AnticholinergicDrugId", SqlDbType.Int).Value = premed.AnticholinergicDrug.Id;
                if (premed.AnticholinergicDosage == 0.0M)
                    cmd.Parameters.Add("@AnticholinergicDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AnticholinergicDosage", SqlDbType.Decimal).Value = premed.AnticholinergicDosage;
                if (premed.KetamineDosage == 0.0M)
                    cmd.Parameters.Add("@KetamineDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@KetamineDosage", SqlDbType.Decimal).Value = premed.KetamineDosage;
                if (premed.Route.Id == -1)
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = premed.Route.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public bool CreateASFUser(ASFUser user)
        {
            bool val = false;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.ASF_User (
                            Username, FullName, Email, IsAdmin
                            ) VALUES (
                            @Username, @FullName, @Email, @IsAdmin
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                if (user.FullName == null)
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = user.FullName;

                if (user.EmailAddress == null)
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = user.EmailAddress;

                cmd.Parameters.Add("@IsAdmin", SqlDbType.Bit).Value = 0;
                try
                {
                    conn.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        val = true;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return val;
        }

        public void CreateMembership(MembershipInfo member)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.aspnet_Membership (
                            Username, Password, PasswordFormat, PasswordSalt, IsApproved, IsLockedOut, CreateDate, LastLoginDate, LastPasswordChangedDate,
                            LastLockoutDate, FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart
                            ) VALUES (
                            @Username, @Password, @PasswordFormat, @PasswordSalt, @IsApproved, @IsLockedOut, @CreateDate, @LastLoginDate, @LastPasswordChangedDate,
                            @LastLockoutDate, @FailedPasswordAttemptCount, @FailedPasswordAttemptWindowStart, @FailedPasswordAnswerAttemptCount, @FailedPasswordAnswerAttemptWindowStart
                            )
                           SELECT SCOPE_IDENTITY() as Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = member.Username;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = member.Password;
                cmd.Parameters.Add("@PasswordFormat", SqlDbType.Int).Value = member.PasswordFormat;
                cmd.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar).Value = member.PasswordSalt;
                cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = member.IsApproved;
                cmd.Parameters.Add("@IsLockedOut", SqlDbType.Bit).Value = member.IsLockedOut;
                cmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = member.CreateDate;
                cmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = member.LastLoginDate;
                cmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = member.LastPasswordChangedDate;
                cmd.Parameters.Add("@LastLockoutDate", SqlDbType.DateTime).Value = member.LastLockoutDate;
                cmd.Parameters.Add("@FailedPasswordAttemptCount", SqlDbType.Int).Value = member.FailedPasswordAttemptCount;
                cmd.Parameters.Add("@FailedPasswordAttemptWindowStart", SqlDbType.DateTime).Value = member.FailedPasswordAttemptWindowStart;
                cmd.Parameters.Add("@FailedPasswordAnswerAttemptCount", SqlDbType.Int).Value = member.FailedPasswordAnswerAttemptCount;
                cmd.Parameters.Add("@FailedPasswordAnswerAttemptWindowStart", SqlDbType.DateTime).Value = member.FailedPasswordAnswerAttemptWindowStart;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateBloodwork(Bloodwork blood, string bloodworkName, decimal bloodworkValue)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Bloodwork_To_Patient (
                            PatientId, BloodworkName, Value
                            ) VALUES (
                            @PatientId, @BloodworkName, @Value
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = blood.PatientId;
                cmd.Parameters.Add("@BloodworkName", SqlDbType.NVarChar).Value = bloodworkName;
                cmd.Parameters.Add("@Value", SqlDbType.Decimal).Value = bloodworkValue;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateClinicalFinding(ClinicalFindings cFind)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Clinical_Findings (
                            PatientId, Temperature, PulseRate, RespiratoryRate, CardiacAuscultationId, PulseQualityId, MucousMembraneColorId, 
                            CapillaryRefillTimeId, RespiratoryAuscultationId, PhysicalStatusClassId, ReasonForClassification, CurrentMedications,
                            OtherAnestheticConcerns
                            ) VALUES (
                            @PatientId, @Temperature, @PulseRate, @RespiratoryRate, @CardiacAuscultationId, @PulseQualityId, @MucousMembraneColorId,
                            @CapillaryRefillTimeId, @RespiratoryAuscultationId, @PhysicalStatusClassId, @ReasonForClassification, @CurrentMedications,
                            @OtherAnestheticConcerns
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = cFind.PatientId;

                if (cFind.Temperature == 0.0M)
                    cmd.Parameters.Add("@Temperature", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Temperature", SqlDbType.Decimal).Value = cFind.Temperature;

                if (cFind.PulseRate == 0.0M)
                    cmd.Parameters.Add("@PulseRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PulseRate", SqlDbType.Decimal).Value = cFind.PulseRate;

                if (cFind.RespiratoryRate == 0.0M)
                    cmd.Parameters.Add("@RespiratoryRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RespiratoryRate", SqlDbType.Decimal).Value = cFind.RespiratoryRate;

                if (cFind.CardiacAuscultation.Id == -1)
                    cmd.Parameters.Add("@CardiacAuscultationId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CardiacAuscultationId", SqlDbType.Int).Value = cFind.CardiacAuscultation.Id;

                if (cFind.PulseQuality.Id == -1)
                    cmd.Parameters.Add("@PulseQualityId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PulseQualityId", SqlDbType.Int).Value = cFind.PulseQuality.Id;

                if (cFind.MucousMembraneColor.Id == -1)
                    cmd.Parameters.Add("@MucousMembraneColorId", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MucousMembraneColorId", SqlDbType.NVarChar).Value = cFind.MucousMembraneColor.Id;
                
                if (cFind.CapillaryRefillTime.Id == -1)
                    cmd.Parameters.Add("@CapillaryRefillTimeId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CapillaryRefillTimeId", SqlDbType.Int).Value = cFind.CapillaryRefillTime.Id;

                if (cFind.RespiratoryAuscultation.Id == -1)
                    cmd.Parameters.Add("@RespiratoryAuscultationId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RespiratoryAuscultationId", SqlDbType.Int).Value = cFind.RespiratoryAuscultation.Id;

                if (cFind.PhysicalStatusClassification.Id == -1)
                    cmd.Parameters.Add("@PhysicalStatusClassId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PhysicalStatusClassId", SqlDbType.Int).Value = cFind.PhysicalStatusClassification.Id;

                if (cFind.ReasonForClassification == null)
                    cmd.Parameters.Add("@ReasonForClassification", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ReasonForClassification", SqlDbType.NVarChar).Value = cFind.ReasonForClassification;

                if (cFind.CurrentMedications == null)
                    cmd.Parameters.Add("@CurrentMedications", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CurrentMedications", SqlDbType.NVarChar).Value = cFind.CurrentMedications;
                if (cFind.OtherAnestheticConcerns == null)
                    cmd.Parameters.Add("@OtherAnestheticConcerns", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OtherAnestheticConcerns", SqlDbType.NVarChar).Value = cFind.OtherAnestheticConcerns;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateCurrentMedication(CurrentMedication meds)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Current_Medications_To_Patient (
                            PatientId, MedicationId
                            ) VALUES (
                            @PatientId, @MedicationId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = meds.PatientId;
                cmd.Parameters.Add("@MedicationId", SqlDbType.Int).Value = meds.Medication.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateDropdownCategory(DropdownCategory cat)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Dropdown_Categories (
                            ShortName, LongName
                            ) VALUES (
                            @ShortName, @LongName
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@ShortName", SqlDbType.NVarChar).Value = cat.ShortName;
                cmd.Parameters.Add("@LongName", SqlDbType.NVarChar).Value = cat.LongName;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateDropdownType(DropdownValue val)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Dropdown_Types (
                            CategoryId, Label, OtherFlag, Description, Concentration, MaxDosage
                            ) VALUES (
                            @CategoryId, @Label, @OtherFlag, @Description, @Concentration, @MaxDosage
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = val.Category.Id;
                cmd.Parameters.Add("@Label", SqlDbType.NVarChar).Value = val.Label;
                cmd.Parameters.Add("@OtherFlag", SqlDbType.Char).Value = val.OtherFlag;
                if (val.Description == null)
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = val.Description;
                if (val.MaxDosage == -1)
                    cmd.Parameters.Add("@MaxDosage", SqlDbType.Float).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaxDosage", SqlDbType.Float).Value = val.MaxDosage;
                if (val.Concentration == -1)
                    cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = val.Concentration;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

//        public void CreateDrugInformation(DrugInformation drug)
//        {
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"INSERT INTO dbo.Drug_Information (
//                            DrugId, DoseMinRange, DoseMaxRange, DoseMax, DoseUnits, Route, Concentration, ConcentrationUnits
//                            ) VALUES (
//                            @DrugId, @DoseMinRange, @DoseMaxRange, @DoseMax, @DoseUnits, @Route, @Concentration, @ConcentrationUnits
//                            )";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = drug.Drug.Id;
//                cmd.Parameters.Add("@DoseMinRange", SqlDbType.Float).Value = drug.DoseMinRange;
//                cmd.Parameters.Add("@DoseMaxRange", SqlDbType.Float).Value = drug.DoseMaxRange;
//                cmd.Parameters.Add("@DoseMax", SqlDbType.Float).Value = drug.DoseMax;
//                cmd.Parameters.Add("@DoseUnits", SqlDbType.NVarChar).Value = drug.DoseUnits;
//                cmd.Parameters.Add("@Route", SqlDbType.NVarChar).Value = drug.Route;
//                cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = drug.Concentration;
//                cmd.Parameters.Add("@ConcentrationUnits", SqlDbType.NVarChar).Value = drug.ConcentrationUnits;
//                try
//                {
//                    conn.Open();
//                    cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

        public void CreateIntraoperativeAnalgesia(IntraoperativeAnalgesia opera)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Intraoperative_Analgesia_To_Patient (
                            PatientId, AnalgesiaId
                            ) VALUES (
                            @PatientId, @AnalgesiaId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = opera.PatientId;
                cmd.Parameters.Add("@AnalgesiaId", SqlDbType.Int).Value = opera.Analgesia.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateIVFluidType(IVFluidType iv)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.IV_Fluid_Type_To_Patient (
                            PatientId, FluidTypeId, Dose
                            ) VALUES (
                            @PatientId, @FluidTypeId, @Dose
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = iv.PatientId;
                cmd.Parameters.Add("@FluidTypeId", SqlDbType.Int).Value = iv.FluidType.Id;
                cmd.Parameters.Add("@Dose", SqlDbType.Decimal).Value = iv.Dose;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateMaintenanceInhalantDrug(MaintenanceInhalantDrug maintInhalant)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Maintenance_Inhalant_Drugs_To_Patient (
                            PatientId, DrugId, InductionDose, InductionOxygenFlowRate, 
                            MaintenanceDose, MaintenanceOxygenFlowRate, BreathingSystemId, BreathingBagSizeId
                            ) VALUES (
                            @PatientId, @DrugId, @InductionDose, @InductionOxygenFlowRate,
                            @MaintenanceDose, @MaintenanceOxygenFlowRate, @BreathingSystemId, @BreathingBagSizeId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = maintInhalant.PatientId;
                if (maintInhalant.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInhalant.Drug.Id;
                if (maintInhalant.InductionPercentage == 0.0M)
                    cmd.Parameters.Add("@InductionDose", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@InductionDose", SqlDbType.Decimal).Value = maintInhalant.InductionPercentage;
                if (maintInhalant.InductionOxygenFlowRate == 0.0M)
                    cmd.Parameters.Add("@InductionOxygenFlowRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@InductionOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.InductionOxygenFlowRate;
                if (maintInhalant.MaintenancePercentage == 0.0M)
                    cmd.Parameters.Add("@MaintenanceDose", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaintenanceDose", SqlDbType.Decimal).Value = maintInhalant.MaintenancePercentage;
                if (maintInhalant.MaintenanceOxygenFlowRate == 0.0M)
                    cmd.Parameters.Add("@MaintenanceOxygenFlowRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaintenanceOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.MaintenanceOxygenFlowRate;
                if (maintInhalant.BreathingSystem.Id == -1)
                    cmd.Parameters.Add("@BreathingSystemId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@BreathingSystemId", SqlDbType.Int).Value = maintInhalant.BreathingSystem.Id;
                if (maintInhalant.BreathingBagSize.Id == -1)
                    cmd.Parameters.Add("@BreathingBagSizeId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@BreathingBagSizeId", SqlDbType.Int).Value = maintInhalant.BreathingBagSize.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateMaintenanceInjectionDrug(MaintenanceInjectionDrug maintInject)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Maintenance_Injection_Drugs_To_Patient (
                            PatientId, DrugId, RouteOfAdministrationId, Dosage
                            ) VALUES (
                            @PatientId, @DrugId, @RouteOfAdministrationId, @Dosage
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = maintInject.PatientId;
                if (maintInject.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInject.Drug.Id;
                if (maintInject.RouteOfAdministration.Id == -1)
                    cmd.Parameters.Add("@RouteOfAdministrationId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RouteOfAdministrationId", SqlDbType.Int).Value = maintInject.RouteOfAdministration.Id;
                if (maintInject.Dosage == 0.0M)
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = maintInject.Dosage;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void CreateMaintenanceOther(MaintenanceOther maintOther)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Maintenance_Other_To_Patient (
                            PatientId, OtherAnestheticDrugs, IntraoperativeAnalgesiaId, IVFluidTypeId
                            ) VALUES (
                            @PatientId, @OtherAnestheticDrugs, @IntraoperativeAnalgesiaId, @IVFluidTypeId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = maintOther.PatientId;
                if (maintOther.OtherAnestheticDrug == null)
                    cmd.Parameters.Add("@OtherAnestheticDrugs", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OtherAnestheticDrugs", SqlDbType.NVarChar).Value = maintOther.OtherAnestheticDrug;
                if (maintOther.IntraoperativeAnalgesia.Id == -1)
                    cmd.Parameters.Add("@IntraoperativeAnalgesiaId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@IntraoperativeAnalgesiaId", SqlDbType.Int).Value = maintOther.IntraoperativeAnalgesia.Id;
                if (maintOther.IVFluidType.Id == -1)
                    cmd.Parameters.Add("@IVFluidTypeId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@IVFluidTypeId", SqlDbType.Decimal).Value = maintOther.IVFluidType.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateMonitoring(Monitoring monitor)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Monitoring_To_Patient (
                            PatientId, EquipmentId, OtherEquipment
                            ) VALUES (
                            @PatientId, @EquipmentId, @OtherEquipment
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = monitor.PatientId;
                if (monitor.Equipment.Id == -1)
                    cmd.Parameters.Add("@EquipmentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@EquipmentId", SqlDbType.NVarChar).Value = monitor.Equipment.Id;

                if (monitor.OtherEquipment == null)
                    cmd.Parameters.Add("@OtherEquipment", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OtherEquipment", SqlDbType.NVarChar).Value = monitor.OtherEquipment;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateOtherAnestheticDrug(OtherAnestheticDrug otherDrugs)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Other_Anesthetic_Drugs_To_Patient (
                            PatientId, DrugId
                            ) VALUES (
                            @PatientId, @DrugId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = otherDrugs.PatientId;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = otherDrugs.Drug.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public int CreatePatient(Patient pat)
        {
            int ident = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Patient (
                            StudentId, ClinicianId, FormCompleted, TemperamentId, ProcedureDate, CageOrStallNumber, BodyWeight,
                            AgeInYears, AgeInMonths, PreOpPainAssessmentId, PostOpPainAssessmentId, DateSeenOn, ProcedureId, ProcedureOther
                            ) VALUES (
                            @StudentId, @ClinicianId, @FormCompleted, @TemperamentId, @ProcedureDate, @CageOrStallNumber, @BodyWeight,
                            @AgeInYears, @AgeInMonths, @PreOpPainAssessmentId, @PostOpPainAssessmentId, @DateSeenOn, @ProcedureId, @ProcedureOther)
                            SELECT SCOPE_IDENTITY() as Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@StudentId", SqlDbType.NVarChar).Value = pat.PatientInfo.Student.Username;
                cmd.Parameters.Add("@ClinicianId", SqlDbType.NVarChar).Value = pat.PatientInfo.Clinician.Username;
                cmd.Parameters.Add("@FormCompleted", SqlDbType.Char).Value = pat.PatientInfo.FormCompleted;

                if (pat.PatientInfo.Temperament.Id == -1)
                    cmd.Parameters.Add("@TemperamentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@TemperamentId", SqlDbType.Int).Value = pat.PatientInfo.Temperament.Id;

                cmd.Parameters.Add("@DateSeenOn", SqlDbType.DateTime).Value = DateTime.Now;

                if (pat.PatientInfo.ProcedureDate == DateTime.MinValue)
                    cmd.Parameters.Add("@ProcedureDate", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ProcedureDate", SqlDbType.DateTime).Value = pat.PatientInfo.ProcedureDate;

                if (pat.PatientInfo.CageOrStallNumber == -1)
                    cmd.Parameters.Add("@CageOrStallNumber", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CageOrStallNumber", SqlDbType.Int).Value = pat.PatientInfo.CageOrStallNumber;

                if (pat.PatientInfo.BodyWeight == -1)
                    cmd.Parameters.Add("@BodyWeight", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@BodyWeight", SqlDbType.Decimal).Value = pat.PatientInfo.BodyWeight;

                if (pat.PatientInfo.AgeInYears == -1)
                    cmd.Parameters.Add("@AgeInYears", SqlDbType.TinyInt).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AgeInYears", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInYears;

                if (pat.PatientInfo.AgeInMonths == -1)
                    cmd.Parameters.Add("@AgeInMonths", SqlDbType.TinyInt).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AgeInMonths", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInMonths;

                if (pat.PatientInfo.PreOperationPainAssessment.Id == -1)
                    cmd.Parameters.Add("@PreOpPainAssessmentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PreOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PreOperationPainAssessment.Id;

                if (pat.PatientInfo.PostOperationPainAssessment.Id == -1)
                    cmd.Parameters.Add("@PostOpPainAssessmentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PostOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PostOperationPainAssessment.Id;
                
                if (pat.PatientInfo.Procedure.Id == -1)
                    cmd.Parameters.Add("@ProcedureId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ProcedureId", SqlDbType.Int).Value = pat.PatientInfo.Procedure.Id;

                if (pat.PatientInfo.ProcedureOther == null)
                    cmd.Parameters.Add("@ProcedureOther", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ProcedureOther", SqlDbType.NVarChar).Value = pat.PatientInfo.ProcedureOther;

                try
                {
                    conn.Open();
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        ident = Convert.ToInt32(read["Id"]);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return ident;
        }

        public void CreatePriorAnesthesia(PriorAnesthesia priorAnes)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Prior_Anesthesia_To_Patient (
                            PatientId, DateOfProblem, Problem
                            ) VALUES (
                            @PatientId, @DateOfProblem, @Problem
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = priorAnes.PatientId;
                if (priorAnes.DateOfProblem != DateTime.MinValue)
                    cmd.Parameters.Add("@DateOfProblem", SqlDbType.DateTime).Value = priorAnes.DateOfProblem;
                else
                    cmd.Parameters.Add("@DateOfProblem", SqlDbType.DateTime).Value = DBNull.Value;
                cmd.Parameters.Add("@Problem", SqlDbType.NVarChar).Value = priorAnes.Problem;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateProcedure(Procedure proc)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Procedure_To_Patient (
                            PatientId, ProcedureId
                            ) VALUES (
                            @PatientId, @ProcedureId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = proc.PatientId;
                cmd.Parameters.Add("@ProcedureId", SqlDbType.DateTime).Value = proc.ProcedureInformation.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        #endregion

        #region UPDATE
        public int UpdateAdministrationSet(AdministrationSet adminSet)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Administration_Set_To_Patient SET
                            MiniDripFlag = @MiniDripFlag, MaxiDripFlag = @MaxiDripFlag
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = adminSet.PatientId;
                if (adminSet.MiniDripFlag == -1)
                    cmd.Parameters.Add("@MiniDripFlag", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MiniDripFlag", SqlDbType.Int).Value = adminSet.MiniDripFlag;

                if (adminSet.MaxiDripFlag == -1)
                    cmd.Parameters.Add("@MaxiDripFlag", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaxiDripFlag", SqlDbType.Int).Value = adminSet.MaxiDripFlag;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

//        public int UpdateAnesthesiaConcern(AnesthesiaConcern aConcern)
//        {
//            int returnNum = 0;
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"UPDATE dbo.Anesthesia_Concerns_To_Patient SET
//                            ConcernId = @ConcernId
//                            WHERE
//                            PatientId = @PatientId";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = aConcern.PatientId;
//                cmd.Parameters.Add("@ConcernId", SqlDbType.Int).Value = aConcern.Concern.Id;
//                try
//                {
//                    conn.Open();
//                    returnNum = cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//            return returnNum;
//        }

        public int UpdateAnestheticPlanInhalant(AnestheticPlanInhalant inhalant)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Anesthetic_Plan_Inhalant SET
                            DrugId = @DrugId, Percentage =  @Percentage, FlowRate = @FlowRate
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = inhalant.PatientId;
                if (inhalant.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = inhalant.Drug.Id;
                if (inhalant.Percentage == 0.0M)
                    cmd.Parameters.Add("@Percentage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Percentage", SqlDbType.Decimal).Value = inhalant.Percentage;
                if (inhalant.FlowRate == 0.0M)
                    cmd.Parameters.Add("@FlowRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@FlowRate", SqlDbType.Decimal).Value = inhalant.FlowRate;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int UpdateAnestheticPlanInjection(AnestheticPlanInjection injection)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Anesthetic_Plan_Injection SET
                            DrugId = @DrugId, RouteId = @RouteId, Dosage = @Dosage
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = injection.PatientId;
                if (injection.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = injection.Drug.Id;
                if (injection.Route.Id == -1)
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = injection.Route.Id;
                if (injection.Dosage == 0.0M)
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = injection.Dosage;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int UpdateAnestheticPlanPremedication(AnestheticPlanPremedication premed)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Anesthetic_Plan_Premed SET
                            RouteId = @RouteId, SedativeDosage = @SedativeDosage, SedativeDrugId = @SedativeDrugId, OpioidDosage = @OpioidDosage,
                            OpioidDrugId = @OpioidDrugId, AnticholinergicDosage = @AnticholinergicDosage, AnticholinergicDrugId = @AnticholinergicDrugId,
                            KetamineDosage = @KetamineDosage
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = premed.PatientId;
                if (premed.SedativeDrug.Id == -1)
                    cmd.Parameters.Add("@SedativeDrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@SedativeDrugId", SqlDbType.Int).Value = premed.SedativeDrug.Id;
                if (premed.SedativeDosage == 0.0M)
                    cmd.Parameters.Add("@SedativeDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@SedativeDosage", SqlDbType.Decimal).Value = premed.SedativeDosage;
                if (premed.OpioidDosage == 0.0M)
                    cmd.Parameters.Add("@OpioidDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OpioidDosage", SqlDbType.Decimal).Value = premed.OpioidDosage;
                if (premed.OpioidDrug.Id == -1)
                    cmd.Parameters.Add("@OpioidDrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OpioidDrugId", SqlDbType.Int).Value = premed.OpioidDrug.Id;
                if (premed.AnticholinergicDrug.Id == -1)
                    cmd.Parameters.Add("@AnticholinergicDrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AnticholinergicDrugId", SqlDbType.Int).Value = premed.AnticholinergicDrug.Id;
                if (premed.AnticholinergicDosage == 0.0M)
                    cmd.Parameters.Add("@AnticholinergicDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AnticholinergicDosage", SqlDbType.Decimal).Value = premed.AnticholinergicDosage;
                if (premed.KetamineDosage == 0.0M)
                    cmd.Parameters.Add("@KetamineDosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@KetamineDosage", SqlDbType.Decimal).Value = premed.KetamineDosage;
                if (premed.Route.Id == -1)
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = premed.Route.Id;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public void UpdateASFUser(ASFUser user)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.ASF_User SET
                            FullName = @FullName, Email = @Email
                            WHERE
                            Username = @Username";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                if (user.FullName == null)
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = user.FullName;

                if (user.EmailAddress == null)
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = user.EmailAddress;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public bool UpdateMembershipPassword(MembershipInfo member)
        {
            bool b = false;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.aspnet_Membership SET
                            Password = @Password
                            WHERE
                            Username = @Username ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = member.Password;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = member.Username;

                try
                {
                    conn.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                        b = true;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return b;
        }

        public void UpdateForgottenPassword(string user, string newPassword)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.aspnet_Membership SET
                            Password = @Password
                            WHERE
                            Username = @Username";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = newPassword;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public int UpdateBloodwork(Bloodwork blood, string bloodworkName, decimal bloodworkValue)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Bloodwork_To_Patient SET
                            BloodworkName = @BloodworkName, Value = @Value
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = blood.Id;
                cmd.Parameters.Add("@BloodworkName", SqlDbType.NVarChar).Value = bloodworkName;
                cmd.Parameters.Add("@Value", SqlDbType.Decimal).Value = bloodworkValue;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int UpdateClinicalFinding(ClinicalFindings cFind)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Clinical_Findings SET
                            Temperature = @Temperature, PulseRate = @PulseRate, RespiratoryRate = @RespiratoryRate, CardiacAuscultationId = @CardiacAuscultationId, 
                            PulseQualityId = @PulseQualityId, MucousMembraneColorId = @MucousMembraneColorId, CapillaryRefillTimeId = @CapillaryRefillTimeId, 
                            RespiratoryAuscultationId = @RespiratoryAuscultationId, PhysicalStatusClassId = @PhysicalStatusClassId, 
                            ReasonForClassification = @ReasonForClassification, CurrentMedications = @CurrentMedications, 
                            OtherAnestheticConcerns = @OtherAnestheticConcerns
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = cFind.PatientId;

                if (cFind.Temperature == 0.0M)
                    cmd.Parameters.Add("@Temperature", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Temperature", SqlDbType.Decimal).Value = cFind.Temperature;

                if (cFind.PulseRate == 0.0M)
                    cmd.Parameters.Add("@PulseRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PulseRate", SqlDbType.Decimal).Value = cFind.PulseRate;

                if (cFind.RespiratoryRate == 0.0M)
                    cmd.Parameters.Add("@RespiratoryRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RespiratoryRate", SqlDbType.Decimal).Value = cFind.RespiratoryRate;

                if (cFind.CardiacAuscultation.Id == -1)
                    cmd.Parameters.Add("@CardiacAuscultationId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CardiacAuscultationId", SqlDbType.Int).Value = cFind.CardiacAuscultation.Id;

                if (cFind.PulseQuality.Id == -1)
                    cmd.Parameters.Add("@PulseQualityId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PulseQualityId", SqlDbType.Int).Value = cFind.PulseQuality.Id;

                if (cFind.MucousMembraneColor.Id == -1)
                    cmd.Parameters.Add("@MucousMembraneColorId", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MucousMembraneColorId", SqlDbType.NVarChar).Value = cFind.MucousMembraneColor.Id;

                if (cFind.CapillaryRefillTime.Id == -1)
                    cmd.Parameters.Add("@CapillaryRefillTimeId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CapillaryRefillTimeId", SqlDbType.Int).Value = cFind.CapillaryRefillTime.Id;

                if (cFind.RespiratoryAuscultation.Id == -1)
                    cmd.Parameters.Add("@RespiratoryAuscultationId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RespiratoryAuscultationId", SqlDbType.Int).Value = cFind.RespiratoryAuscultation.Id;

                if (cFind.PhysicalStatusClassification.Id == -1)
                    cmd.Parameters.Add("@PhysicalStatusClassId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PhysicalStatusClassId", SqlDbType.Int).Value = cFind.PhysicalStatusClassification.Id;

                if (cFind.ReasonForClassification == null)
                    cmd.Parameters.Add("@ReasonForClassification", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ReasonForClassification", SqlDbType.NVarChar).Value = cFind.ReasonForClassification;

                if (cFind.CurrentMedications == null)
                    cmd.Parameters.Add("@CurrentMedications", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CurrentMedications", SqlDbType.NVarChar).Value = cFind.CurrentMedications;
                if (cFind.OtherAnestheticConcerns == null)
                    cmd.Parameters.Add("@OtherAnestheticConcerns", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OtherAnestheticConcerns", SqlDbType.NVarChar).Value = cFind.OtherAnestheticConcerns;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

//        public int UpdateCurrentMedication(CurrentMedication meds)
//        {
//            int returnNum = 0;
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"UPDATE dbo.Current_Medications_To_Patient SET
//                            MedicationId = @MedicationId
//                            WHERE
//                            Id = @Id";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = meds.Id;
//                cmd.Parameters.Add("@MedicationId", SqlDbType.Int).Value = meds.Medication.Id;
//                try
//                {
//                    conn.Open();
//                    returnNum = cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//            return returnNum;
//        }

        public void UpdateDropdownCategory(DropdownCategory cat)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Dropdown_Categories SET
                            ShortName = @ShortName, LongName = @LongName
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = cat.Id;
                cmd.Parameters.Add("@ShortName", SqlDbType.NVarChar).Value = cat.ShortName;
                cmd.Parameters.Add("@LongName", SqlDbType.NVarChar).Value = cat.LongName;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void UpdateDropdownType(DropdownValue val)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Dropdown_Types SET
                            Label = @Label, Description = @Description, Concentration = @Concentration, MaxDosage = @MaxDosage
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = val.Id;
                cmd.Parameters.Add("@Label", SqlDbType.NVarChar).Value = val.Label;
                if (val.Description == null)
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = val.Description;
                if (val.MaxDosage == -1)
                    cmd.Parameters.Add("@MaxDosage", SqlDbType.Float).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaxDosage", SqlDbType.Float).Value = val.MaxDosage;
                if (val.Concentration == -1)
                    cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = val.Concentration;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

//        public void UpdateDrugInformation(DrugInformation drug)
//        {
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"UPDATE dbo.Drug_Information SET
//                            DoseMinRange = @DoseMinRange, DoseMaxRange = @DoseMaxRange, DoseMax = @DoseMax, DoseUnits = @DoseUnits, 
//                            Route = @Route, Concentration = @Concentration, ConcentrationUnits = @ConcentrationUnits
//                            WHERE
//                            DrugId = @DrugId";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = drug.Drug.Id;
//                cmd.Parameters.Add("@DoseMinRange", SqlDbType.Float).Value = drug.DoseMinRange;
//                cmd.Parameters.Add("@DoseMaxRange", SqlDbType.Float).Value = drug.DoseMaxRange;
//                cmd.Parameters.Add("@DoseMax", SqlDbType.Float).Value = drug.DoseMax;
//                cmd.Parameters.Add("@DoseUnits", SqlDbType.NVarChar).Value = drug.DoseUnits;
//                cmd.Parameters.Add("@Route", SqlDbType.NVarChar).Value = drug.Route;
//                cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = drug.Concentration;
//                cmd.Parameters.Add("@ConcentrationUnits", SqlDbType.NVarChar).Value = drug.ConcentrationUnits;
//                try
//                {
//                    conn.Open();
//                    cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        public void UpdateIntraoperativeAnalgesia(DropdownValue opera)
//        {
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"UPDATE dbo.Intraoperative_Analgesia_To_Patient SET 
//                            AnalgesiaId = @AnalgesiaId
//                            WHERE
//                            Id = @Id";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = opera.Id;
//                cmd.Parameters.Add("@AnalgesiaId", SqlDbType.Int).Value = opera..Id;
//                try
//                {
//                    conn.Open();
//                    cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        public int UpdateIVFluidType(IVFluidType iv)
//        {
//            int returnNum = 0;
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"UPDATE dbo.IV_Fluid_Type_To_Patient SET
//                            FluidTypeId = @FluidTypeId, Dose = @Dose
//                            WHERE
//                            Id = @Id";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = iv.Id;
//                cmd.Parameters.Add("@FluidTypeId", SqlDbType.Int).Value = iv.FluidType.Id;
//                cmd.Parameters.Add("@Dose", SqlDbType.Decimal).Value = iv.Dose;
//                try
//                {
//                    conn.Open();
//                    returnNum = cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//            return returnNum;
//        }

        public int UpdateMaintenanceInhalantDrug(MaintenanceInhalantDrug maintInhalant)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Maintenance_Inhalant_Drugs_To_Patient SET
                            DrugId = @DrugId, InductionDose = @InductionDose, InductionOxygenFlowRate = @InductionOxygenFlowRate,
                            MaintenanceDose = @MaintenanceDose, MaintenanceOxygenFlowRate = @MaintenanceOxygenFlowRate, 
                            BreathingSystemId = @BreathingSystemId, BreathingBagSizeId = @BreathingBagSizeId
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = maintInhalant.PatientId;
                if (maintInhalant.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInhalant.Drug.Id;
                if (maintInhalant.InductionPercentage == 0.0M)
                    cmd.Parameters.Add("@InductionDose", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@InductionDose", SqlDbType.Decimal).Value = maintInhalant.InductionPercentage;
                if (maintInhalant.InductionOxygenFlowRate == 0.0M)
                    cmd.Parameters.Add("@InductionOxygenFlowRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@InductionOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.InductionOxygenFlowRate;
                if (maintInhalant.MaintenancePercentage == 0.0M)
                    cmd.Parameters.Add("@MaintenanceDose", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaintenanceDose", SqlDbType.Decimal).Value = maintInhalant.MaintenancePercentage;
                if (maintInhalant.MaintenanceOxygenFlowRate == 0.0M)
                    cmd.Parameters.Add("@MaintenanceOxygenFlowRate", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@MaintenanceOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.MaintenanceOxygenFlowRate;
                if (maintInhalant.BreathingSystem.Id == -1)
                    cmd.Parameters.Add("@BreathingSystemId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@BreathingSystemId", SqlDbType.Int).Value = maintInhalant.BreathingSystem.Id;
                if (maintInhalant.BreathingBagSize.Id == -1)
                    cmd.Parameters.Add("@BreathingBagSizeId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@BreathingBagSizeId", SqlDbType.Int).Value = maintInhalant.BreathingBagSize.Id;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int UpdateMaintenanceInjectionDrug(MaintenanceInjectionDrug maintInject)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Maintenance_Injection_Drugs_To_Patient SET
                            DrugId = @DrugId, RouteOfAdministrationId = @RouteOfAdministrationId, Dosage = @Dosage
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = maintInject.PatientId;
                if (maintInject.Drug.Id == -1)
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInject.Drug.Id;
                if (maintInject.RouteOfAdministration.Id == -1)
                    cmd.Parameters.Add("@RouteOfAdministrationId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@RouteOfAdministrationId", SqlDbType.Int).Value = maintInject.RouteOfAdministration.Id;
                if (maintInject.Dosage == 0.0M)
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = maintInject.Dosage;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }

                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int UpdateMaintenanceOther(MaintenanceOther maintOther)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Maintenance_Other_To_Patient SET
                            OtherAnestheticDrugs = @OtherAnestheticDrugs, IntraoperativeAnalgesiaId = @IntraoperativeAnalgesiaId, IVFluidTypeId = @IVFluidTypeId
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = maintOther.PatientId;
                if (maintOther.OtherAnestheticDrug == null)
                    cmd.Parameters.Add("@OtherAnestheticDrugs", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OtherAnestheticDrugs", SqlDbType.NVarChar).Value = maintOther.OtherAnestheticDrug;
                if (maintOther.IntraoperativeAnalgesia.Id == -1)
                    cmd.Parameters.Add("@IntraoperativeAnalgesiaId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@IntraoperativeAnalgesiaId", SqlDbType.Int).Value = maintOther.IntraoperativeAnalgesia.Id;
                if (maintOther.IVFluidType.Id == -1)
                    cmd.Parameters.Add("@IVFluidTypeId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@IVFluidTypeId", SqlDbType.Decimal).Value = maintOther.IVFluidType.Id;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int UpdateMonitoring(Monitoring monitor)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Monitoring_To_Patient SET
                            EquipmentId = @EquipmentId, OtherEquipment = @OtherEquipment
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = monitor.PatientId;
                if (monitor.Equipment.Id == -1)
                    cmd.Parameters.Add("@EquipmentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@EquipmentId", SqlDbType.NVarChar).Value = monitor.Equipment.Id;

                if (monitor.OtherEquipment == null)
                    cmd.Parameters.Add("@OtherEquipment", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@OtherEquipment", SqlDbType.NVarChar).Value = monitor.OtherEquipment;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

//        public int UpdateOtherAnestheticDrug(OtherAnestheticDrug otherDrugs)
//        {
//            int returnNum = 0;
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"UPDATE dbo.Other_Anesthetic_Drugs_To_Patient SET
//                            DrugId = @DrugId
//                            WHERE
//                            PatientId = @PatientId";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = otherDrugs.Id;
//                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = otherDrugs.Drug.Id;
//                try
//                {
//                    conn.Open();
//                    returnNum = cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//            return returnNum;
//        }

        public int UpdatePatient(Patient pat)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Patient SET
                            StudentId = @StudentId, ClinicianId = @ClinicianId, FormCompleted = @FormCompleted, TemperamentId = @TemperamentId, 
                            DateSeenOn = @DateSeenOn, CageOrStallNumber = @CageOrStallNumber, BodyWeight = @BodyWeight,
                            AgeInYears = @AgeInYears, AgeInMonths = @AgeInMonths, PreOpPainAssessmentId = @PreOpPainAssessmentId, 
                            PostOpPainAssessmentId = @PostOpPainAssessmentId, ProcedureDate = @ProcedureDate, ProcedureId = @ProcedureId,
                            ProcedureOther = @ProcedureOther
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = pat.PatientId;
                cmd.Parameters.Add("@StudentId", SqlDbType.NVarChar).Value = pat.PatientInfo.Student.Username;
                cmd.Parameters.Add("@ClinicianId", SqlDbType.NVarChar).Value = pat.PatientInfo.Clinician.Username;
                cmd.Parameters.Add("@FormCompleted", SqlDbType.Char).Value = pat.PatientInfo.FormCompleted;

                if (pat.PatientInfo.Temperament.Id == -1)
                    cmd.Parameters.Add("@TemperamentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@TemperamentId", SqlDbType.Int).Value = pat.PatientInfo.Temperament.Id;

                cmd.Parameters.Add("@DateSeenOn", SqlDbType.DateTime).Value = DateTime.Now;

                if (pat.PatientInfo.ProcedureDate == DateTime.MinValue)
                    cmd.Parameters.Add("@ProcedureDate", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ProcedureDate", SqlDbType.DateTime).Value = pat.PatientInfo.ProcedureDate;

                if (pat.PatientInfo.CageOrStallNumber == -1)
                    cmd.Parameters.Add("@CageOrStallNumber", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@CageOrStallNumber", SqlDbType.Int).Value = pat.PatientInfo.CageOrStallNumber;

                if (pat.PatientInfo.BodyWeight == -1)
                    cmd.Parameters.Add("@BodyWeight", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@BodyWeight", SqlDbType.Decimal).Value = pat.PatientInfo.BodyWeight;

                if (pat.PatientInfo.AgeInYears == -1)
                    cmd.Parameters.Add("@AgeInYears", SqlDbType.TinyInt).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AgeInYears", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInYears;

                if (pat.PatientInfo.AgeInMonths == -1)
                    cmd.Parameters.Add("@AgeInMonths", SqlDbType.TinyInt).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@AgeInMonths", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInMonths;

                if (pat.PatientInfo.PreOperationPainAssessment.Id == -1)
                    cmd.Parameters.Add("@PreOpPainAssessmentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PreOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PreOperationPainAssessment.Id;

                if (pat.PatientInfo.PostOperationPainAssessment.Id == -1)
                    cmd.Parameters.Add("@PostOpPainAssessmentId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@PostOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PostOperationPainAssessment.Id;

                if (pat.PatientInfo.Procedure.Id == -1)
                    cmd.Parameters.Add("@ProcedureId", SqlDbType.Int).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ProcedureId", SqlDbType.Int).Value = pat.PatientInfo.Procedure.Id;

                if (pat.PatientInfo.ProcedureOther == null)
                    cmd.Parameters.Add("@ProcedureOther", SqlDbType.NVarChar).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@ProcedureOther", SqlDbType.NVarChar).Value = pat.PatientInfo.ProcedureOther;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int UpdatePriorAnesthesia(PriorAnesthesia priorAnes)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Prior_Anesthesia_To_Patient SET
                            DateOfProblem = @DateOfProblem, Problem = @Problem
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = priorAnes.PatientId;
                if (priorAnes.DateOfProblem != DateTime.MinValue)
                    cmd.Parameters.Add("@DateOfProblem", SqlDbType.DateTime).Value = priorAnes.DateOfProblem;
                else
                    cmd.Parameters.Add("@DateOfProblem", SqlDbType.DateTime).Value = DBNull.Value;
                cmd.Parameters.Add("@Problem", SqlDbType.NVarChar).Value = priorAnes.Problem;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

//        public int UpdateProcedure(Procedure proc)
//        {
//            int returnNum = 0;
//            using (SqlConnection conn = new SqlConnection(connString))
//            {
//                string sql = @"UPDATE dbo.Procedure_To_Patient SET
//                            ProcedureId = @ProcedureId
//                            WHERE
//                            Id = @Id";

//                SqlCommand cmd = new SqlCommand(sql, conn);
//                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = proc.Id;
//                cmd.Parameters.Add("@ProcedureId", SqlDbType.DateTime).Value = proc.ProcedureInformation.Id;
//                try
//                {
//                    conn.Open();
//                    returnNum = cmd.ExecuteNonQuery();
//                }
//                catch (Exception e)
//                {
//                    throw e;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//            return returnNum;
//        }

        public int Promote(ASFUser user)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.ASF_User SET
                            IsAdmin = @IsAdmin
                            WHERE
                            Username = @Username";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                cmd.Parameters.Add("@IsAdmin", SqlDbType.Bit).Value = 1;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        public int Demote(ASFUser user)
        {
            int returnNum = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.ASF_User SET
                            IsAdmin = @IsAdmin
                            WHERE
                            Username = @Username";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                cmd.Parameters.Add("@IsAdmin", SqlDbType.Bit).Value = 0;
                try
                {
                    conn.Open();
                    returnNum = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
            return returnNum;
        }

        #endregion

        #region DELETE
        public void DeleteAdministrationSet(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Administration_Set_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteAnesthesiaConcern(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthesia_Concerns_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteAnestheticPlanInhalant(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthetic_Plan_Inhalant
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteAnestheticPlanInjection(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthetic_Plan_Injection
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteAnestheticPlanPremedication(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthetic_Plan_Premed
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteASFUser(ASFUser user)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.ASF_User
                            WHERE
                            Username = @Username";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteASPNetMembership(MembershipInfo member)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.aspnet_Membership
                            WHERE
                            Username = @Username";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = member.Username;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteBloodwork(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Bloodwork_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteClinicalFinding(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Clinical_Findings
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteCurrentMedication(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Current_Medications_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteDropdownCategory(DropdownCategory cat)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Dropdown_Categories
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = cat.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteDropdownType(DropdownValue val)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Dropdown_Types
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = val.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteDrugInformation(DrugInformation drug)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Drug_Information
                            WHERE
                            DrugId = @DrugId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = drug.Drug.Id;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteIntraoperativeAnalgesia(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Intraoperative_Analgesia_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteIVFluidType(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.IV_Fluid_Type_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteMaintenanceInhalantDrug(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Maintenance_Inhalant_Drugs_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteMaintenanceInjectionDrug(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Maintenance_Injection_Drugs_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void DeleteMaintenanceOther(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Maintenance_Other_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteMonitoring(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Monitoring_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteOtherAnestheticDrug(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Other_Anesthetic_Drugs_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeletePatient(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Patient
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeletePriorAnesthesia(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Prior_Anesthesia_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void DeleteProcedure(int patientId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Procedure_To_Patient
                            WHERE
                            PatientId = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        #endregion

        #region SQL_STATEMENTs

        private string BuildAdministrationSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.MiniDripFlag as 'a.MiniDripFlag', a.MaxiDripFlag as 'a.MaxiDripFlag' ";
        }

        private string BuildAnesthesiaConcernSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.ConcernId as 'a.ConcernId' ";
        }

        private string BuildAnestheticPlanInjectionSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', a.RouteId as 'a.RouteId', a.Dosage as 'a.Dosage' ";
        }

        private string BuildAnestheticPlanInhalantSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', a.Percentage as 'a.Percentage', a.FlowRate as 'a.FlowRate' ";
        }

        private string BuildAnestheticPlanPremedicationSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.RouteId as 'a.RouteId', a.SedativeDosage as 'a.SedativeDosage', a.SedativeDrugId as 'a.SedativeDrugId',
                a.OpioidDosage as 'a.OpioidDosage', a.OpioidDrugId as 'a.OpioidDrugId', a.AnticholinergicDosage as 'a.AnticholinergicDosage', a.AnticholinergicDrugId as 'a.AnticholinergicDrugId',
                a.KetamineDosage as 'a.KetamineDosage'";
        }

        private string BuildBloodworkSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.BloodworkName as 'a.BloodworkName', a.Value as 'a.Value' ";
        }

        private string BuildClinicalFindingsSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.Temperature as 'a.Temperature', a.PulseRate as 'a.PulseRate',
                    a.RespiratoryRate as 'a.RespiratoryRate', a.CardiacAuscultationId as 'a.CardiacAuscultationId', 
                    a.PulseQualityId as 'a.PulseQualityId', a.MucousMembraneColorId as 'a.MucousMembraneColorId',
                    a.CapillaryRefillTimeId as 'a.CapillaryRefillTimeId', a.RespiratoryAuscultationId as 'a.RespiratoryAuscultationId',
                    a.PhysicalStatusClassId as 'a.PhysicalStatusClassId', a.ReasonForClassification as 'a.ReasonForClassification',
                    a.CurrentMedications as 'a.CurrentMedications', a.OtherAnestheticConcerns as 'a.OtherAnestheticConcerns' ";
        }

        private string BuildCurrentMedicationsSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.MedicationId as 'a.MedicationId' ";
        }

        private string BuildDropdownCategorySQL()
        {
            return @"SELECT a.Id as 'a.Id', a.ShortName as 'a.ShortName', a.LongName as 'a.LongName' ";
        }

        private string BuildDropdownValueSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.CategoryId as 'a.CategoryId', a.Label as 'a.Label', a.OtherFlag as 'a.OtherFlag', 
                    a.Description as 'a.Description', a.Concentration as 'a.Concentration', a.MaxDosage as 'a.MaxDosage' ";
        }

        private string BuildMaintenanceInhalantDrugSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', 
                    a.InductionDose as 'a.InductionDose', a.InductionOxygenFlowRate as 'a.InductionOxygenFlowRate', 
                    a.MaintenanceDose as 'a.MaintenanceDose', 
                    a.MaintenanceOxygenFlowRate as 'a.MaintenanceOxygenFlowRate', 
                    a.BreathingSystemId as 'a.BreathingSystemId', a.BreathingBagSizeId as 'a.BreathingBagSizeId', a.OtherAnestheticDrugs as 'a.OtherAnestheticDrugs', 
                    a.IntraoperativeAnalgesiaId as 'a.IntraoperativeAnalgesiaId', a.IVFluidTypeId as 'a.IVFluidTypeId' ";
        }

        private string BuildMaintenanceInjectionDrugSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', 
                    a.RouteOfAdministrationId as 'a.RouteOfAdministrationId', a.Dosage as 'a.Dosage', a.OtherAnestheticDrugs as 'a.OtherAnestheticDrugs',
                    a.IntraoperativeAnalgesiaId as 'a.IntraoperativeAnalgesiaId', a.IVFluidTypeId as 'a.IVFluidTypeId' ";
        }

        private string BuildMaintenanceOtherSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.OtherAnestheticDrugs as 'a.OtherAnestheticDrugs',
                    a.IntraoperativeAnalgesiaId as 'a.IntraoperativeAnalgesiaId', a.IVFluidTypeId as 'a.IVFluidTypeId' ";
        }

        private string BuildMonitoringSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId 'a.PatientId', a.EquipmentId as 'a.EquipmentId', a.OtherEquipment as 'a.OtherEquipment' ";
        }

        private string BuildOtherAnestheticDrugSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId ";
        }

        private string BuildPatientInformationSQL()
        {
            return @"SELECT a.PatientId as 'a.PatientId', a.StudentId as 'a.StudentId', a.ClinicianId as 'a.ClinicianId', a.FormCompleted as 'a.FormCompleted', 
                    a.TemperamentId as 'a.TemperamentId', a.DateSeenOn as 'a.DateSeenOn', a.CageOrStallNumber as 'a.CageOrStallNumber', 
                    a.BodyWeight as 'a.BodyWeight', a.AgeInYears as 'a.AgeInYears', a.AgeInMonths as 'a.AgeInMonths', 
                    a.PreOpPainAssessmentId as 'a.PreOpPainAssessmentId', a.PostOpPainAssessmentId as 'a.PostOpPainAssessmentId', 
                    a.ProcedureDate as 'a.ProcedureDate', a.ProcedureId as 'a.ProcedureId', a.ProcedureOther as 'a.ProcedureOther' ";
        }

        private string BuildPriorAnesthesiaSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DateOfProblem as 'a.DateOfProblem', a.Problem as 'a.Problem' ";
        }

        private string BuildProcedureSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.ProcedureId as 'a.ProcedureId' ";
        }

        private string BuildASFUserSQL()
        {
            return @"SELECT a.Username as 'a.Username', a.Fullname as 'a.Fullname', a.Email as 'a.Email', a.IsAdmin as 'a.IsAdmin' ";
        }

        #endregion
    }
}