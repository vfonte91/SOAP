using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class PriorAnesthesiaCallback
    {
        public PriorAnesthesia ProcessRow(SqlDataReader read)
        {
            PriorAnesthesia prioAnes = new PriorAnesthesia();
            prioAnes.Id = Convert.ToInt32(read["a.Id"]);
            prioAnes.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.DateOfProblem"].ToString() != "") 
                prioAnes.DateOfProblem = Convert.ToDateTime(read["a.DateOfProblem"].ToString());
            prioAnes.Problem = read["a.Problem"].ToString();
            return prioAnes;
        }
    }
}