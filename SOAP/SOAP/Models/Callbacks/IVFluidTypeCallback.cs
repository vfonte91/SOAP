using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class IVFluidTypeCallback
    {
        public IVFluidType ProcessRow(SqlDataReader read, params IVFluidType.LazyComponents[] lazyComponents)
        {
            IVFluidType ivFluidType = new IVFluidType();
            ivFluidType.Id = Convert.ToInt32(read["a.Id"]);
            ivFluidType.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            ivFluidType.FluidType.Id = Convert.ToInt32(read["a.FluidTypeId"].ToString());
            ivFluidType.Dose = Convert.ToDecimal(read["a.Dose"].ToString());

            foreach (IVFluidType.LazyComponents a in lazyComponents)
            {
                if (a == IVFluidType.LazyComponents.LOAD_FLUID_TYPE_WITH_DETAILS)
                {
                    ivFluidType.FluidType.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    ivFluidType.FluidType.Label = read["b.Label"].ToString();
                    ivFluidType.FluidType.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    ivFluidType.FluidType.Description = read["b.Description"].ToString();
                }
            }

            return ivFluidType;
        }
    }
}