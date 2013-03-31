using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class DropdownValueCallback
    {
        public DropdownValue ProcessRow(SqlDataReader read, params DropdownValue.LazyComponents[] lazyComponents)
        {
            DropdownValue value = new DropdownValue();
            value.Id = Convert.ToInt32(read["a.Id"]);
            value.Category.Id = Convert.ToInt32(read["a.CategoryId"].ToString());
            value.Label = read["a.Label"].ToString();
            value.OtherFlag = Convert.ToChar(read["a.OtherFlag"].ToString());
            value.Description = read["a.Description"].ToString();
            if (read["a.Concentration"].ToString() != "")
                value.Concentration = Convert.ToDecimal(read["a.Concentration"].ToString());
            if (read["a.MaxDosage"].ToString() != "")
                value.MaxDosage = Convert.ToDecimal(read["a.MaxDosage"].ToString());


            foreach (DropdownValue.LazyComponents a in lazyComponents)
            {
                if (a == DropdownValue.LazyComponents.LOAD_DROPDOWN_CATEGORY)
                {
                    value.Category.ShortName = read["b.ShortName"].ToString();
                    value.Category.LongName = read["b.LongName"].ToString();
                }
            }

            return value;
        }
    }
}