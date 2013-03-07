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
            user.UserId = Convert.ToInt32(read["a.Username"]);
            user.Username = read["a.Username"].ToString();
            user.FullName = read["a.FullName"].ToString();
            user.EmailAddress = read["a.Email"].ToString();
            user.IsAdmin = Convert.ToInt16(read["a.IsAdmin"].ToString());
            return user;
        }
    }
}