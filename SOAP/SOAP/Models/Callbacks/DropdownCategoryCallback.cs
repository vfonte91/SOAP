using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class DropdownCategoryCallback
    {
        public DropdownCategory ProcessRow(SqlDataReader read, params DropdownCategory.LazyComponents[] lazyComponents)
        {
            DropdownCategory category = new DropdownCategory();
            category.Id = Convert.ToInt32(read["Id"]);
            category.ShortName = read["ShortName"].ToString();
            category.LongName = read["LongName"].ToString();

            return category;
        }
    }
}