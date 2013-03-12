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
            value.Id = Convert.ToInt32(read["Id"]);
            value.Category.Id = Convert.ToInt32(read["CategoryId"].ToString());
            value.Label = read["Label"].ToString();
            value.OtherFlag = Convert.ToChar(read["OtherFlag"].ToString());
            value.Description = read["Description"].ToString();

            foreach (DropdownValue.LazyComponents a in lazyComponents)
            {
                if (a == DropdownValue.LazyComponents.LOAD_DROPDOWN_CATEGORY)
                {
                    value.Category.ShortName = read["ShortName"].ToString();
                    value.Category.LongName = read["LongName"].ToString();
                }
            }

            return value;
        }
    }
}