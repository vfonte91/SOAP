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
            category.Id = Convert.ToInt32(read["a.Id"]);
            category.ShortName = read["a.ShortName"].ToString();
            category.LongName = read["a.LongName"].ToString();

            return category;
        }
    }
}