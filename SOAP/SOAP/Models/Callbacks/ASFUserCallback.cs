using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class ASFUserCallback
    {
        public ASFUser ProcessRow(SqlDataReader read)
        {
            ASFUser user = new ASFUser();
            user.Username = read["a.Username"].ToString();
            user.FullName = read["a.FullName"].ToString();
            user.EmailAddress = read["a.Email"].ToString();
            user.IsAdmin = Convert.ToBoolean(read["a.IsAdmin"].ToString());
            return user;
        }
    }
}