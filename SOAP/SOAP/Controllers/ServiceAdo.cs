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

        #endregion

        #region READ

        public bool CheckUserForForgotPassword(ASFUser user)
        {
            bool valToReturn = false;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = BuildASFUserSQL();
                string from = @"FROM dbo.ASF_User as a ";
                string where = @"WHERE a.Username = @Username AND a.Email = @Email ";

                sql = sql + from + where;
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.Username;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = user.EmailAddress;
                try
                {
                    conn.Open();
                    
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        valToReturn = true;
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
            return valToReturn;
        }

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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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
                        sql += @", b.Id as 'b.Id', b.DoseMinRange as 'b.DoseMinRange', b.DoseMaxRange as 'b.DoseMaxRange', b.DoseMax as 'b.DoseMax', 
                              b.DoseUnits as 'b.DoseUnits', b.Route as 'b.Route', b.Concentration as 'b.Concentration', b.ConcentrationUnits as 'b.ConcentrationUnits', 
                                   d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description'";
                        from += @" LEFT OUTER JOIN dbo.Drug_Information as b ON a.DrugId = b.DrugId 
                                   LEFT OUTER JOIN dbo.Dropdown_Types d on d.DrugId = b.DrugId";
                    }

                    else if (a == AnestheticPlanInjection.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description'";
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
                        sql += @", b.Id as 'b.Id', b.DoseMinRange as 'b.DoseMinRange', b.DoseMaxRange as 'b.DoseMaxRange', b.DoseMax as 'b.DoseMax', 
                              b.DoseUnits as 'b.DoseUnits', b.Route as 'b.Route', b.Concentration as 'b.Concentration', b.ConcentrationUnits as 'b.ConcentrationUnits', 
                                   d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description'";
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
                        from += @" INNER JOIN dbo.Dropdown_Types as b ON a.DrugId = b.Id ";
                    }

                    else if (a == AnestheticPlanPremedication.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description'";
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.CardiacAuscultationId = b.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_PHYSICAL_STATUS_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description'";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.PhysicalStatusClassId = c.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_PULSE_QUALITY_WITH_DETAILS)
                    {
                        sql += @", d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description'";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.PulseQualityId = d.Id ";
                    }
                    else if (a == ClinicalFindings.LazyComponents.LOAD_RESPIRATORY_AUSCULTATION_WITH_DETAILS)
                    {
                        sql += @", e.CategoryId as 'e.CategoryId', e.Label as 'e.Label', e.OtherFlag as 'e.OtherFlag', e.Description as 'e.Description'";
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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

                sql = sql + from + where + " ORDER BY a.Id ";

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

                string where = @" WHERE a.CategoryId = @CategoryId AND a.OtherFlag = 'N'";

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

                string sql = @"SELECT PatientId, DateSeenOn FROM dbo.Patient WHERE StudentId = @Username";

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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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
                catch (Exception e)
                {
                    throw e;
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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
                catch (Exception e)
                {
                    throw e;
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as b ON a.DrugId = b.Id ";
                    }
                    else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_BAG_SIZE_WITH_SETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description'";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as c ON a.BreathingSystemId = c.Id ";
                    }
                    else if (a == MaintenanceInhalantDrug.LazyComponents.LOAD_BREATHING_SYSTEM_WITH_DETAILS)
                    {
                        sql += @", d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description'";
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
                        sql += @", b.Id as 'b.Id', b.DoseMinRange as 'b.DoseMinRange', b.DoseMaxRange as 'b.DoseMaxRange', b.DoseMax as 'b.DoseMax', 
                              b.DoseUnits as 'b.DoseUnits', b.Route as 'b.Route', b.Concentration as 'b.Concentration', b.ConcentrationUnits as 'b.ConcentrationUnits', 
                                   d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description'";
                        from += @" LEFT OUTER JOIN dbo.Drug_Information as b ON a.DrugId = b.DrugId 
                                   LEFT OUTER JOIN dbo.Dropdown_Types d on d.DrugId = b.DrugId";
                    }
                    else if (a == MaintenanceInjectionDrug.LazyComponents.LOAD_ROUTE_WITH_DETAILS)
                    {
                        sql += @", c.CategoryId as 'c.CategoryId', c.Label as 'c.Label', c.OtherFlag as 'c.OtherFlag', c.Description as 'c.Description'";
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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
                        sql += @", d.CategoryId as 'd.CategoryId', d.Label as 'd.Label', d.OtherFlag as 'd.OtherFlag', d.Description as 'd.Description'";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as d ON a.PostOpPainAssessmentId = d.Id ";
                    }
                    else if (a == PatientInformation.LazyComponents.LOAD_PREOP_PAIN_DETAIL)
                    {
                        sql += @", e.CategoryId as 'e.CategoryId', e.Label as 'e.Label', e.OtherFlag as 'e.OtherFlag', e.Description as 'e.Description'";
                        from += @" LEFT OUTER JOIN dbo.Dropdown_Types as e ON a.PreOpPainAssessmentId = e.Id ";
                    }

                    else if (a == PatientInformation.LazyComponents.LOAD_TEMPERAMENT_DETAIL)
                    {
                        sql += @", f.CategoryId as 'f.CategoryId', f.Label as 'f.Label', f.OtherFlag as 'f.OtherFlag', f.Description as 'f.Description'";
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
                        sql += @", b.CategoryId as 'b.CategoryId', b.Label as 'b.Label', b.OtherFlag as 'b.OtherFlag', b.Description as 'b.Description'";
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
                cmd.Parameters.Add("@MiniDripFlag", SqlDbType.Int).Value = adminSet.MiniDripFlag;
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
                            PatientId, DrugId, Dose, FlowRate
                            ) VALUES (
                            @PatientId, @DrugId, @Dose, @FlowRate
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = inhalant.PatientId;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = inhalant.Drug.Id;
                cmd.Parameters.Add("@Dose", SqlDbType.Decimal).Value = inhalant.Dose;
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
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = injection.Drug.Id;
                cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = injection.Route.Id;
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
                            PatientId, DrugId, RouteId, Dosage
                            ) VALUES (
                            @PatientId, @DrugId, @RouteId, @Dosage
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = premed.PatientId;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = premed.Drug.Id;
                cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = premed.Route.Id;
                cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = premed.Dosage;
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
                cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = user.FullName;
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

        public void CreateBloodwork(Bloodwork blood)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Bloodwork_To_Patient (
                            PatientId, BloodworkId, Value
                            ) VALUES (
                            @PatientId, @BloodworkId, @Value
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = blood.PatientId;
                cmd.Parameters.Add("@BloodworkId", SqlDbType.Int).Value = blood.BloodworkInfo.Id;
                cmd.Parameters.Add("@Value", SqlDbType.Decimal).Value = blood.Value;
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
                            PatientId, Temperature, PulseRate, RespiratoryRate, CardiacAuscultationId, PulseQualityId, MucousMembraneColor, 
                            CapillaryRefillTime, RespiratoryAuscultationId, PhysicalStatusClassId, ReasonForClassification
                            ) VALUES (
                            @PatientId, @Temperature, @PulseRate, @RespiratoryRate, @CardiacAuscultationId, @PulseQualityId, @MucousMembraneColor,
                            @CapillaryRefillTime, @RespiratoryAuscultationId, @PhysicalStatusClassId, @ReasonForClassification
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = cFind.PatientId;
                cmd.Parameters.Add("@Temperature", SqlDbType.Decimal).Value = cFind.Temperature;
                cmd.Parameters.Add("@PulseRate", SqlDbType.Decimal).Value = cFind.PulseRate;
                cmd.Parameters.Add("@RespiratoryRate", SqlDbType.Decimal).Value = cFind.RespiratoryRate;
                cmd.Parameters.Add("@CardiacAuscultationId", SqlDbType.Int).Value = cFind.CardiacAuscultation.Id;
                cmd.Parameters.Add("@PulseQualityId", SqlDbType.Int).Value = cFind.PulseQuality.Id;
                cmd.Parameters.Add("@MucousMembraneColor", SqlDbType.NVarChar).Value = cFind.MucousMembraneColor;
                cmd.Parameters.Add("@CapillaryRefillTime", SqlDbType.Decimal).Value = cFind.CapillaryRefillTime;
                cmd.Parameters.Add("@RespiratoryAuscultationId", SqlDbType.Int).Value = cFind.RespiratoryAuscultation.Id;
                cmd.Parameters.Add("@PhysicalStatusClassId", SqlDbType.Int).Value = cFind.PhysicalStatusClassification.Id;
                cmd.Parameters.Add("@ReasonForClassification", SqlDbType.NVarChar).Value = cFind.ReasonForClassification;
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
                cmd.Parameters.Add("@BloodworkId", SqlDbType.Int).Value = meds.Medication.Id;
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
                            CategoryId, Label, OtherFlag, Description
                            ) VALUES (
                            @CategoryId, @Label, @OtherFlag, @Description
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = val.Category.Id;
                cmd.Parameters.Add("@Label", SqlDbType.NVarChar).Value = val.Label;
                cmd.Parameters.Add("@OtherFlag", SqlDbType.Char).Value = val.OtherFlag;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = val.Description;
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

        public void CreateDrugInformation(DrugInformation drug)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Drug_Information (
                            DrugId, DoseMinRange, DoseMaxRange, DoseMax, DoseUnits, Route, Concentration, ConcentrationUnits
                            ) VALUES (
                            @DrugId, @DoseMinRange, @DoseMaxRange, @DoseMax, @DoseUnits, @Route, @Concentration, @ConcentrationUnits
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = drug.Drug.Id;
                cmd.Parameters.Add("@DoseMinRange", SqlDbType.Float).Value = drug.DoseMinRange;
                cmd.Parameters.Add("@DoseMaxRange", SqlDbType.Float).Value = drug.DoseMaxRange;
                cmd.Parameters.Add("@DoseMax", SqlDbType.Float).Value = drug.DoseMax;
                cmd.Parameters.Add("@DoseUnits", SqlDbType.NVarChar).Value = drug.DoseUnits;
                cmd.Parameters.Add("@Route", SqlDbType.NVarChar).Value = drug.Route;
                cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = drug.Concentration;
                cmd.Parameters.Add("@ConcentrationUnits", SqlDbType.NVarChar).Value = drug.ConcentrationUnits;
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
                string sql = @"INSERT INTO dbo.IV_Fluid_Type_To_Patient (
                            PatientId, DrugId, InductionReqFlag, InductionDose, InductionOxygenFlowRate, MaintenanceReqFlag, 
                            MaintenanceDose, MaintenanceOxygenFlowRate, EquipmentReqFlag, BreathingSystemId, BreathingBagSizeId
                            ) VALUES (
                            @PatientId, @DrugId, @InductionReqFlag, @InductionDose, @InductionOxygenFlowRate, @MaintenanceReqFlag,
                            @MaintenanceDose, @MaintenanceOxygenFlowRate, @EquipmentReqFlag, @BreathingSystemId, @BreathingBagSizeId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = maintInhalant.PatientId;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInhalant.Drug.Id;
                cmd.Parameters.Add("@InductionReqFlag", SqlDbType.Char).Value = maintInhalant.InductionReqFlag;
                cmd.Parameters.Add("@InductionDose", SqlDbType.Decimal).Value = maintInhalant.InductionDose;
                cmd.Parameters.Add("@InductionOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.InductionOxygenFlowRate;
                cmd.Parameters.Add("@MaintenanceReqFlag", SqlDbType.Char).Value = maintInhalant.MaintenanceReqFlag;
                cmd.Parameters.Add("@MaintenanceDose", SqlDbType.Decimal).Value = maintInhalant.MaintenanceDose;
                cmd.Parameters.Add("@MaintenanceOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.MaintenanceOxygenFlowRate;
                cmd.Parameters.Add("@EquipmentReqFlag", SqlDbType.Char).Value = maintInhalant.EquipmentReqFlag;
                cmd.Parameters.Add("@BreathingSystemId", SqlDbType.Int).Value = maintInhalant.BreathingSystem.Id;
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
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInject.Drug.Id;
                cmd.Parameters.Add("@RouteOfAdministrationId", SqlDbType.Int).Value = maintInject.RouteOfAdministration.Id;
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

        public void CreateMonitoring(Monitoring monitor)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Monitoring_To_Patient (
                            PatientId, EquipmentId
                            ) VALUES (
                            @PatientId, @EquipmentId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = monitor.PatientId;
                cmd.Parameters.Add("@EquipmentId", SqlDbType.Int).Value = monitor.Equipment.Id;
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

        public void CreatePatient(Patient pat)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"INSERT INTO dbo.Patient (
                            StudentId, ClinicianId, FormCompleted, TemperamentId, DateSeenOn, CageOrStallNumber, BodyWeight,
                            AgeInYears, AgeInMonths, PreOpPainAssessmentId, PostOpPainAssessmentId
                            ) VALUES (
                            @StudentId, @ClinicianId, @FormCompleted, @TemperamentId, @DateSeenOn, @CageOrStallNumber, @BodyWeight,
                            @AgeInYears, @AgeInMonths, @PreOpPainAssessmentId, @PostOpPainAssessmentId
                            )";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@StudentId", SqlDbType.NVarChar).Value = pat.PatientInfo.Student.Username;
                cmd.Parameters.Add("@ClinicianId", SqlDbType.NVarChar).Value = pat.PatientInfo.Clinician.Username;
                cmd.Parameters.Add("@FormCompleted", SqlDbType.Char).Value = pat.PatientInfo.FormCompleted;
                cmd.Parameters.Add("@TemperamentId", SqlDbType.Int).Value = pat.PatientInfo.Temperament.Id;
                cmd.Parameters.Add("@DateSeenOn", SqlDbType.DateTime).Value = pat.PatientInfo.DateSeenOn;
                cmd.Parameters.Add("@CageOrStallNumber", SqlDbType.Int).Value = pat.PatientInfo.CageOrStallNumber;
                cmd.Parameters.Add("@BodyWeight", SqlDbType.Decimal).Value = pat.PatientInfo.BodyWeight;
                cmd.Parameters.Add("@AgeInYears", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInYears;
                cmd.Parameters.Add("@AgeInMonths", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInMonths;
                cmd.Parameters.Add("@PreOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PreOperationPainAssessment.Id;
                cmd.Parameters.Add("@PostOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PostOperationPainAssessment.Id;
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
                cmd.Parameters.Add("@DateOfProblem", SqlDbType.DateTime).Value = priorAnes.DateOfProblem;
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
        public void UpdateAdministrationSet(AdministrationSet adminSet)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Administration_Set_To_Patient SET
                            MiniDripFlag = @MiniDripFlag, MaxiDripFlag = @MaxiDripFlag
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = adminSet.Id;
                cmd.Parameters.Add("@MiniDripFlag", SqlDbType.Int).Value = adminSet.MiniDripFlag;
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

        public void UpdateAnesthesiaConcern(AnesthesiaConcern aConcern)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Anesthesia_Concerns_To_Patient SET
                            ConcernId = @ConcernId
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = aConcern.Id;
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

        public void UpdateAnestheticPlanInhalant(AnestheticPlanInhalant inhalant)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Anesthetic_Plan_Inhalant SET
                            DrugId = @DrugId, Dose =  @Dose, FlowRate = @FlowRate
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = inhalant.Id;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = inhalant.Drug.Id;
                cmd.Parameters.Add("@Dose", SqlDbType.Decimal).Value = inhalant.Dose;
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

        public void UpdateAnestheticPlanInjection(AnestheticPlanInjection injection)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Anesthetic_Plan_Injection SET
                            DrugId = @DrugId, RouteId = @RouteId, Dosage = @Dosage
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = injection.Id;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = injection.Drug.Id;
                cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = injection.Route.Id;
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

        public void UpdateAnestheticPlanPremedication(AnestheticPlanPremedication premed)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UDPATE dbo.Anesthetic_Plan_Premed SET
                            DrugId = @DrugId, RouteId = @RouteId, Dosage = @Dosage
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = premed.Id;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = premed.Drug.Id;
                cmd.Parameters.Add("@RouteId", SqlDbType.Decimal).Value = premed.Route.Id;
                cmd.Parameters.Add("@Dosage", SqlDbType.Decimal).Value = premed.Dosage;
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
                cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = user.FullName;
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

        public bool UpdateMembershipPassword(MembershipInfo member, string oldpassword)
        {
            bool b = false;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Anesthetic_Plan_Premed SET
                            Password = @Password
                            WHERE
                            Username = @Username AND Password = @OldPassword";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = member.Password;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = member.Username;
                cmd.Parameters.Add("@OldPassword", SqlDbType.NVarChar).Value = oldpassword;

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

        public bool UpdateForgottenPassword(MembershipInfo member)
        {
            bool b = false;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.aspnet_Membership SET
                            Password = @Password
                            WHERE
                            Username = @Username";

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

        public void UpdateBloodwork(Bloodwork blood)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Bloodwork_To_Patient SET
                            BloodworkId = @BloodworkId, Value = @Value
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = blood.Id;
                cmd.Parameters.Add("@BloodworkId", SqlDbType.Int).Value = blood.BloodworkInfo.Id;
                cmd.Parameters.Add("@Value", SqlDbType.Decimal).Value = blood.Value;
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

        public void UpdateClinicalFinding(ClinicalFindings cFind)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Clinical_Findings SET
                            Temperature = @Temperature, PulseRate = @PulseRate, RespiratoryRate = @RespiratoryRate, CardiacAuscultationId = @CardiacAuscultationId, 
                            PulseQualityId = @PulseQualityId, MucousMembraneColor = @MucousMembraneColor, CapillaryRefillTime = @CapillaryRefillTime, 
                            RespiratoryAuscultationId = @RespiratoryAuscultationId, PhysicalStatusClassId = @PhysicalStatusClassId, 
                            ReasonForClassification = @ReasonForClassification
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = cFind.Id;
                cmd.Parameters.Add("@Temperature", SqlDbType.Decimal).Value = cFind.Temperature;
                cmd.Parameters.Add("@PulseRate", SqlDbType.Decimal).Value = cFind.PulseRate;
                cmd.Parameters.Add("@RespiratoryRate", SqlDbType.Decimal).Value = cFind.RespiratoryRate;
                cmd.Parameters.Add("@CardiacAuscultationId", SqlDbType.Int).Value = cFind.CardiacAuscultation.Id;
                cmd.Parameters.Add("@PulseQualityId", SqlDbType.Int).Value = cFind.PulseQuality.Id;
                cmd.Parameters.Add("@MucousMembraneColor", SqlDbType.NVarChar).Value = cFind.MucousMembraneColor;
                cmd.Parameters.Add("@CapillaryRefillTime", SqlDbType.Decimal).Value = cFind.CapillaryRefillTime;
                cmd.Parameters.Add("@RespiratoryAuscultationId", SqlDbType.Int).Value = cFind.RespiratoryAuscultation.Id;
                cmd.Parameters.Add("@PhysicalStatusClassId", SqlDbType.Int).Value = cFind.PhysicalStatusClassification.Id;
                cmd.Parameters.Add("@ReasonForClassification", SqlDbType.NVarChar).Value = cFind.ReasonForClassification;
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

        public void UpdateCurrentMedication(CurrentMedication meds)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Current_Medications_To_Patient SET
                            MedicationId = @MedicationId
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = meds.Id;
                cmd.Parameters.Add("@BloodworkId", SqlDbType.Int).Value = meds.Medication.Id;
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
                            Label = @Label, Description = @Description
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = val.Id;
                cmd.Parameters.Add("@Label", SqlDbType.NVarChar).Value = val.Label;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = val.Description;
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

        public void UpdateDrugInformation(DrugInformation drug)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Drug_Information SET
                            DoseMinRange = @DoseMinRange, DoseMaxRange = @DoseMaxRange, DoseMax = @DoseMax, DoseUnits = @DoseUnits, 
                            Route = @Route, Concentration = @Concentration, ConcentrationUnits = @ConcentrationUnits
                            WHERE
                            DrugId = @DrugId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = drug.Drug.Id;
                cmd.Parameters.Add("@DoseMinRange", SqlDbType.Float).Value = drug.DoseMinRange;
                cmd.Parameters.Add("@DoseMaxRange", SqlDbType.Float).Value = drug.DoseMaxRange;
                cmd.Parameters.Add("@DoseMax", SqlDbType.Float).Value = drug.DoseMax;
                cmd.Parameters.Add("@DoseUnits", SqlDbType.NVarChar).Value = drug.DoseUnits;
                cmd.Parameters.Add("@Route", SqlDbType.NVarChar).Value = drug.Route;
                cmd.Parameters.Add("@Concentration", SqlDbType.Float).Value = drug.Concentration;
                cmd.Parameters.Add("@ConcentrationUnits", SqlDbType.NVarChar).Value = drug.ConcentrationUnits;
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

        public void UpdateIntraoperativeAnalgesia(IntraoperativeAnalgesia opera)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Intraoperative_Analgesia_To_Patient SET 
                            AnalgesiaId = @AnalgesiaId
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = opera.Id;
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

        public void UpdateIVFluidType(IVFluidType iv)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.IV_Fluid_Type_To_Patient SET
                            FluidTypeId = @FluidTypeId, Dose = @Dose
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = iv.Id;
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

        public void UpdateMaintenanceInhalantDrug(MaintenanceInhalantDrug maintInhalant)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.IV_Fluid_Type_To_Patient SET
                            DrugId = @DrugId, InductionReqFlag = @InductionReqFlag, InductionDose = @InductionDose, InductionOxygenFlowRate = @InductionOxygenFlowRate
                            MaintenanceReqFlag = @MaintenanceReqFlag, MaintenanceDose = @MaintenanceDose, MaintenanceOxygenFlowRate = @MaintenanceOxygenFlowRate, 
                            EquipmentReqFlag = @EquipmentReqFlag, BreathingSystemId = @BreathingSystemId, BreathingBagSizeId = @BreathingBagSizeId
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = maintInhalant.Id;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInhalant.Drug.Id;
                cmd.Parameters.Add("@InductionReqFlag", SqlDbType.Char).Value = maintInhalant.InductionReqFlag;
                cmd.Parameters.Add("@InductionDose", SqlDbType.Decimal).Value = maintInhalant.InductionDose;
                cmd.Parameters.Add("@InductionOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.InductionOxygenFlowRate;
                cmd.Parameters.Add("@MaintenanceReqFlag", SqlDbType.Char).Value = maintInhalant.MaintenanceReqFlag;
                cmd.Parameters.Add("@MaintenanceDose", SqlDbType.Decimal).Value = maintInhalant.MaintenanceDose;
                cmd.Parameters.Add("@MaintenanceOxygenFlowRate", SqlDbType.Decimal).Value = maintInhalant.MaintenanceOxygenFlowRate;
                cmd.Parameters.Add("@EquipmentReqFlag", SqlDbType.Char).Value = maintInhalant.EquipmentReqFlag;
                cmd.Parameters.Add("@BreathingSystemId", SqlDbType.Int).Value = maintInhalant.BreathingSystem.Id;
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

        public void UpdateMaintenanceInjectionDrug(MaintenanceInjectionDrug maintInject)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Maintenance_Injection_Drugs_To_Patient SET
                            DrugId = @DrugId, RouteOfAdministrationId = @RouteOfAdministrationId, Dosage = @Dosage
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = maintInject.Id;
                cmd.Parameters.Add("@DrugId", SqlDbType.Int).Value = maintInject.Drug.Id;
                cmd.Parameters.Add("@RouteOfAdministrationId", SqlDbType.Int).Value = maintInject.RouteOfAdministration.Id;
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

        public void UpdateMonitoring(Monitoring monitor)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Monitoring_To_Patient SET
                            EquipmentId = @EquipmentId
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = monitor.Id;
                cmd.Parameters.Add("@EquipmentId", SqlDbType.Int).Value = monitor.Equipment.Id;
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

        public void UpdateOtherAnestheticDrug(OtherAnestheticDrug otherDrugs)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Other_Anesthetic_Drugs_To_Patient SET
                            DrugId = @DrugId
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = otherDrugs.Id;
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

        public void UpdatePatient(Patient pat)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Patient SET
                            StudentId = @StudentId, ClinicianId = @ClinicianId, FormCompleted = @FormCompleted, TemperamentId = @TemperamentId, 
                            DateSeenOn = @DateSeenOn, CageOrStallNumber = @CageOrStallNumber, BodyWeight = @BodyWeight,
                            AgeInYears = @AgeInYears, AgeInMonths = @AgeInMonths, PreOpPainAssessmentId = @PreOpPainAssessmentId, 
                            PostOpPainAssessmentId = @PostOpPainAssessmentId
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = pat.PatientId;
                cmd.Parameters.Add("@StudentId", SqlDbType.NVarChar).Value = pat.PatientInfo.Student.Username;
                cmd.Parameters.Add("@ClinicianId", SqlDbType.NVarChar).Value = pat.PatientInfo.Clinician.Username;
                cmd.Parameters.Add("@FormCompleted", SqlDbType.Char).Value = pat.PatientInfo.FormCompleted;
                cmd.Parameters.Add("@TemperamentId", SqlDbType.Int).Value = pat.PatientInfo.Temperament.Id;
                cmd.Parameters.Add("@DateSeenOn", SqlDbType.DateTime).Value = pat.PatientInfo.DateSeenOn;
                cmd.Parameters.Add("@CageOrStallNumber", SqlDbType.Int).Value = pat.PatientInfo.CageOrStallNumber;
                cmd.Parameters.Add("@BodyWeight", SqlDbType.Decimal).Value = pat.PatientInfo.BodyWeight;
                cmd.Parameters.Add("@AgeInYears", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInYears;
                cmd.Parameters.Add("@AgeInMonths", SqlDbType.TinyInt).Value = pat.PatientInfo.AgeInMonths;
                cmd.Parameters.Add("@PreOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PreOperationPainAssessment.Id;
                cmd.Parameters.Add("@PostOpPainAssessmentId", SqlDbType.Int).Value = pat.PatientInfo.PostOperationPainAssessment.Id;
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

        public void UpdatePriorAnesthesia(PriorAnesthesia priorAnes)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Prior_Anesthesia_To_Patient SET
                            DateOfProblem = @DateOfProblem, Problem = @Problem
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = priorAnes.Id;
                cmd.Parameters.Add("@DateOfProblem", SqlDbType.DateTime).Value = priorAnes.DateOfProblem;
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

        public void UpdateProcedure(Procedure proc)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"UPDATE dbo.Procedure_To_Patient SET
                            ProcedureId = @ProcedureId
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = proc.Id;
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

        public void Promote(ASFUser user)
        {
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

        public void Demote(ASFUser user)
        {
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

        #region DELETE
        public void DeleteAdministrationSet(AdministrationSet adminSet)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Administration_Set_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = adminSet.Id;
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

        public void DeleteAnesthesiaConcern(AnesthesiaConcern aConcern)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthesia_Concerns_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = aConcern.Id;
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

        public void DeleteAnestheticPlanInhalant(AnestheticPlanInhalant inhalant)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthetic_Plan_Inhalant
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = inhalant.Id;
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

        public void DeleteAnestheticPlanInjection(AnestheticPlanInjection injection)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthetic_Plan_Injection
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = injection.Id;
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

        public void DeleteAnestheticPlanPremedication(AnestheticPlanPremedication premed)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Anesthetic_Plan_Premed
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = premed.Id;
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

        public void DeleteBloodwork(Bloodwork blood)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Bloodwork_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = blood.Id;
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

        public void DeleteClinicalFinding(ClinicalFindings cFind)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Clinical_Findings
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = cFind.Id;
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

        public void DeleteCurrentMedication(CurrentMedication meds)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Current_Medications_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = meds.Id;
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

        public void DeleteIntraoperativeAnalgesia(IntraoperativeAnalgesia opera)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Intraoperative_Analgesia_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = opera.Id;
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

        public void DeleteIVFluidType(IVFluidType iv)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.IV_Fluid_Type_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = iv.Id;
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

        public void DeleteMaintenanceInhalantDrug(MaintenanceInhalantDrug maintInhalant)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.IV_Fluid_Type_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = maintInhalant.Id;
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

        public void DeleteMaintenanceInjectionDrug(MaintenanceInjectionDrug maintInject)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Maintenance_Injection_Drugs_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = maintInject.Id;
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

        public void DeleteMonitoring(Monitoring monitor)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Monitoring_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = monitor.Id;
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

        public void DeleteOtherAnestheticDrug(OtherAnestheticDrug otherDrugs)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Other_Anesthetic_Drugs_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = otherDrugs.Id;
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

        public void DeletePatient(Patient pat)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Patient
                            WHERE
                            PatientId = @PatientId";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@PatientId", SqlDbType.Int).Value = pat.PatientId;
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

        public void DeletePriorAnesthesia(PriorAnesthesia priorAnes)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Prior_Anesthesia_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = priorAnes.Id;
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

        public void DeleteProcedure(Procedure proc)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string sql = @"DELETE FROM dbo.Procedure_To_Patient
                            WHERE
                            Id = @Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = proc.Id;
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
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', a.Dose as 'a.Dose', a.FlowRate as 'a.FlowRate' ";
        }

        private string BuildAnestheticPlanPremedicationSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', a.RouteId as 'a.RouteId', a.Dosage as 'a.Dosage' ";
        }

        private string BuildBloodworkSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.BloodworkId as 'a.BloodworkId', a.Value as 'a.Value' ";
        }

        private string BuildClinicalFindingsSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.PreOpPainAssessmentId as 'a.PreOpPainAssessmentId', 
                    a.PostOpPainAssessmentId as 'a.PostOpPainAssessmentId', a.Temperature as 'a.Temperature' ";
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
                    a.Description as 'a.Description' ";
        }

        private string BuildIntraoperativeAnalgesiaSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.AnalgesiaId as 'a.AnalgesiaId' ";
        }

        private string BuildIVFluidTypeSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.FluidTypeId as 'a.FluidTypeId', a.Dose as 'a.Dose' ";
        }

        private string BuildMaintenanceInhalantDrugSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', a.InductionReqFlag as 'a.InductionReqFlag', 
                    a.InductionDose as 'a.InductionDose', a.InductionOxygenFlowRate as 'a.InductionOxygenFlowRate', 
                    a.MaintenanceReqFlag as 'a.MaintenanceReqFlag', a.MaintenanceDose as 'a.MaintenanceDose', 
                    a.MaintenanceOxygenFlowRate as 'a.MaintenanceOxygenFlowRate', a.EquipmentReqFlag as 'a.EquipmentReqFlag', 
                    a.BreathingSystemId as 'a.BreathingSystemId', a.BreathingBagSizeId as 'a.BreathingBagSizeId' ";
        }

        private string BuildMaintenanceInjectionDrugSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId as 'a.PatientId', a.DrugId as 'a.DrugId', 
                    a.RouteOfAdministrationId as 'a.RouteOfAdministrationId', a.Dosage as 'a.Dosage' ";
        }

        private string BuildMonitoringSQL()
        {
            return @"SELECT a.Id as 'a.Id', a.PatientId 'a.PatientId', a.EquipmentId as 'a.EquipmentId' ";
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
                    a.PreOpPainAssessmentId as 'a.PreOpPainAssessmentId', a.PostOpPainAssessmentId as 'a.PostOpPainAssessmentId' ";
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