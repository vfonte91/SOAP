using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class BloodworkCallback
    {
        public Bloodwork ProcessRow(SqlDataReader read)
        {
            Bloodwork bloodwork = new Bloodwork();
            bloodwork.Id = Convert.ToInt32(read["a.Id"]);
            bloodwork.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            string bloodworkType = read["a.BloodworkName"].ToString();
            if (read["a.Value"].ToString() != "")
            {
                decimal val = Convert.ToDecimal(read["a.Value"].ToString());
                switch (bloodworkType)
                {
                    case "PCV":
                        bloodwork.PCV = val;
                        break;
                    case "TP":
                        bloodwork.TP = val;
                        break;
                    case "Albumin":
                        bloodwork.Albumin = val;
                        break;
                    case "Globulin":
                        bloodwork.Globulin = val;
                        break;
                    case "WBC":
                        bloodwork.WBC = val;
                        break;
                    case "NA":
                        bloodwork.NA = val;
                        break;
                    case "K":
                        bloodwork.K = val;
                        break;
                    case "Cl":
                        bloodwork.Cl = val;
                        break;
                    case "Ca":
                        bloodwork.Ca = val;
                        break;
                    case "iCa":
                        bloodwork.iCa = val;
                        break;
                    case "Glucose":
                        bloodwork.Glucose = val;
                        break;
                    case "ALT":
                        bloodwork.ALT = val;
                        break;
                    case "ALP":
                        bloodwork.ALP = val;
                        break;
                    case "BUN":
                        bloodwork.BUN = val;
                        break;
                    case "CREAT":
                        bloodwork.CREAT = val;
                        break;
                    case "USG":
                        bloodwork.USG = val;
                        break;
                    default:
                        bloodwork.OtherType = bloodworkType;
                        bloodwork.OtherValue = val;
                        break;
                }
            }

            return bloodwork;
        }
    }
}