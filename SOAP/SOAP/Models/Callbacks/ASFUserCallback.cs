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
            user.UserId = Convert.ToInt32(read["UserId"]);
            user.Username = read["Username"].ToString();
            user.FullName = read["FullName"].ToString();
            user.EmailAddress = read["Email"].ToString();
            user.IsAdmin = Convert.ToBoolean(read["IsAdmin"].ToString());
            return user;
        }
    }
}