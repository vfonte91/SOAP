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
            aSet.Id = Convert.ToInt32(read["Id"]);
            aSet.PatientId = Convert.ToInt32(read["PatientId"].ToString());
            aSet.MiniDripFlag = Convert.ToInt16(read["MiniDripFlag"].ToString());
            aSet.MaxiDripFlag = Convert.ToInt16(read["MaxiDripFlag"].ToString());
            return aSet;
        }
    }
}