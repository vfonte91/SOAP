using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace SOAP.Models.Callbacks
{
    public class AdministrationSetCallback
    {
        public AdministrationSet ProcessRow(SqlDataReader read)
        {
            AdministrationSet aSet = new AdministrationSet();
            aSet.Id = Convert.ToInt32(read["a.Id"]);
            aSet.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            aSet.MiniDripFlag = Convert.ToInt32(read["a.MiniDripFlag"].ToString());
            aSet.MaxiDripFlag = Convert.ToInt32(read["a.MaxiDripFlag"].ToString());
            return aSet;
        }
    }
}