using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class MaintenanceOtherCallback
    {
        public MaintenanceOther ProcessRow(SqlDataReader read, params MaintenanceOther.LazyComponents[] lazyComponents)
        {
            MaintenanceOther maintOther = new MaintenanceOther();
            maintOther.Id = Convert.ToInt32(read["a.Id"]);
            maintOther.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            maintOther.OtherAnestheticDrug = read["a.OtherAnestheticDrugs"].ToString();
            if (read["a.IntraoperativeAnalgesiaId"].ToString() != "")
                maintOther.IntraoperativeAnalgesia.Id = Convert.ToInt32(read["a.IntraoperativeAnalgesiaId"].ToString());
            if (read["a.IVFluidTypeId"].ToString() != "")
                maintOther.IVFluidType.Id = Convert.ToInt32(read["a.IVFluidTypeId"].ToString());

            foreach (MaintenanceOther.LazyComponents a in lazyComponents)
            {
                if (a == MaintenanceOther.LazyComponents.LOAD_INTRAOP_WITH_DETAILS && maintOther.IntraoperativeAnalgesia.Id != -1)
                {
                    if (read["e.CategoryId"].ToString() != "")
                        maintOther.IntraoperativeAnalgesia.Category.Id = Convert.ToInt32(read["e.CategoryId"].ToString());
                    maintOther.IntraoperativeAnalgesia.Label = read["e.Label"].ToString();
                    maintOther.IntraoperativeAnalgesia.OtherFlag = Convert.ToChar(read["e.OtherFlag"].ToString());
                    maintOther.IntraoperativeAnalgesia.Description = read["e.Description"].ToString();
                    if (read["e.Concentration"].ToString() != "")
                        maintOther.IntraoperativeAnalgesia.Concentration = Convert.ToDecimal(read["e.Concentration"].ToString());
                }
                else if (a == MaintenanceOther.LazyComponents.LOAD_IV_WITH_DETAILS && maintOther.IVFluidType.Id != -1)
                {
                    if (read["f.CategoryId"].ToString() != "")
                        maintOther.IVFluidType.Category.Id = Convert.ToInt32(read["f.CategoryId"].ToString());
                    maintOther.IVFluidType.Label = read["f.Label"].ToString();
                    maintOther.IVFluidType.OtherFlag = Convert.ToChar(read["f.OtherFlag"].ToString());
                    maintOther.IVFluidType.Description = read["f.Description"].ToString();
                    if (read["f.Concentration"].ToString() != "")
                        maintOther.IVFluidType.Concentration = Convert.ToDecimal(read["f.Concentration"].ToString());
                }
            }

            return maintOther;
        }
    }
}
