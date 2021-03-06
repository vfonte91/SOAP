﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class ProcedureCallback
    {
        public Procedure ProcessRow(SqlDataReader read, params Procedure.LazyComponents[] lazyComponents)
        {
            Procedure procedure = new Procedure();
            procedure.Id = Convert.ToInt32(read["a.Id"]);
            procedure.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.ProcedureId"].ToString() != "") 
                procedure.ProcedureInformation.Id = Convert.ToInt32(read["a.ProcedureId"].ToString());

            foreach (Procedure.LazyComponents a in lazyComponents)
            {
                if (a == Procedure.LazyComponents.LOAD_PROCEDURE_WITH_DETAIL && procedure.ProcedureInformation.Id != -1)
                {
                    procedure.ProcedureInformation.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    procedure.ProcedureInformation.Label = read["b.Label"].ToString();
                    procedure.ProcedureInformation.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    procedure.ProcedureInformation.Description = read["b.Description"].ToString();
                    if (read["b.Concentration"].ToString() != "")
                        procedure.ProcedureInformation.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                }
            }

            return procedure;
        }
    }
}