using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class IntraoperativeAnalgesiaCallback
    {
        public IntraoperativeAnalgesia ProcessRow(SqlDataReader read, params IntraoperativeAnalgesia.LazyComponents[] lazyComponents)
        {
            IntraoperativeAnalgesia analgesia = new IntraoperativeAnalgesia();
            analgesia.Id = Convert.ToInt32(read["a.Id"]);
            analgesia.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            if (read["a.AnalgesiaId"].ToString() != "")
                analgesia.Analgesia.Id = Convert.ToInt32(read["a.AnalgesiaId"].ToString());

            foreach (IntraoperativeAnalgesia.LazyComponents a in lazyComponents)
            {
                if (a == IntraoperativeAnalgesia.LazyComponents.LOAD_ANALGESIA_WITH_DETAILS && analgesia.Analgesia.Id != -1)
                {
                    analgesia.Analgesia.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    analgesia.Analgesia.Label = read["b.Label"].ToString();
                    analgesia.Analgesia.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    analgesia.Analgesia.Description = read["b.Description"].ToString();
                    if (read["b.Concentration"].ToString() != "")
                        analgesia.Analgesia.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                }
            }

            return analgesia;
        }
    }
}