using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class ASFUser
    {
        private int _userId;
        private string _username;
        private string _fullName;
        private string _emailAddress;

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        public ASFUser()
        {
            _userId = -1;
        }

        public bool ValidateASFUser()
        {
            if (_userId == 0 || _username == null || _fullName == null)
                return false;
            else
                return true;
        }
    }
}