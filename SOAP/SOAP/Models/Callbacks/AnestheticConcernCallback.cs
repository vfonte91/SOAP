using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class AnestheticConcernCallback
    {
        public AnesthesiaConcern ProcessRow(SqlDataReader read, params AnesthesiaConcern.LazyComponents[] lazyComponents)
        {
            AnesthesiaConcern anestheticConcern = new AnesthesiaConcern();
            anestheticConcern.Id = Convert.ToInt32(read["a.Id"]);
            anestheticConcern.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            anestheticConcern.Concern.Id = Convert.ToInt16(read["a.ConcernId"].ToString());

            foreach (AnesthesiaConcern.LazyComponents a in lazyComponents)
            {
                if (a == AnesthesiaConcern.LazyComponents.LOAD_CONCERN_WITH_DETAILS)
                {
                    anestheticConcern.Concern.Category.Id = Convert.ToInt16(read["b.CategoryId"].ToString());
                    anestheticConcern.Concern.Label = read["b.Label"].ToString();
                    anestheticConcern.Concern.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    anestheticConcern.Concern.Description = read["b.Description"].ToString();
                }
            }

            return anestheticConcern;
        }
    }
}