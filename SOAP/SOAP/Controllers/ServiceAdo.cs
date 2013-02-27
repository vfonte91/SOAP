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

        public List<AdministrationSet> GetAdministrationSets(int patientId)
        {
            List<AdministrationSet> aSets = new List<AdministrationSet>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Abstract database field names as much as possible in case of field name changes
                string sql = BuildAdministrationSQL();
                string from = @"FROM dbo.Administration_Set_To_Patient AS a";
                string where = @"WHERE a.PatientId = @PatientId ";

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
                string sql = @"SELECT a.Id, a.PatientId";

                string from = @"FROM dbo.Anesthesia_Concerns_To_Patient AS a";

                string where = @"WHERE a.PatientId = @PatientId ";

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

                string where = @"WHERE a.PatientId = @PatientId ";

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

                string where = @"WHERE a.PatientId = @PatientId ";

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

                string where = @"WHERE a.PatientId = @PatientId ";

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

                string where = @"WHERE a.PatientId = @PatientId ";

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

                string where = @"WHERE a.PatientId = @PatientId ";

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

        private string BuildAdministrationSQL()
        {
            return @"SELECT Id, PatientId, MiniDripFlag, MaxiDripFlag";
        }

        private string BuildAnestheticPlanInjectionSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.RouteId, a.Dosage";
        }

        private string BuildAnestheticPlanInhalantSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.Dose, a.FlowRate";
        }

        private string BuildAnestheticPlanPremedicationSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.DrugId, a.RouteId, a.Dosage";
        }

        private string BuildBloodworkSQL()
        {
            return @"SELECT a.Id, a.PatientId, a.BloodworkId, a.Value";
        }

        private string BuildClinicalFindingsSQL()
        {
            return @"SELECT a.Id, a.PatientId, PreOpPainAssessmentId, PostOpPainAssessmentId, Temperature";
        }
    }
}