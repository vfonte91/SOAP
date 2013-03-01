using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class ASPNETMembership
    {
        private Guid _id;
        private string password;
        private string _passwordFormat;
        private string _passwordSalt;
        private bool _isLockedOut;
        private DateTime _lastLoginDate;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string PasswordFormat
        {
            get { return _passwordFormat; }
            set { _passwordFormat = value; }
        }

        public string PasswordSalt
        {
            get { return _passwordSalt; }
            set { _passwordSalt = value; }
        }

        public bool IsLockedOut
        {
            get { return _isLockedOut; }
            set { _isLockedOut = value; }
        }

        public DateTime LastLoginDate
        {
            get { return _lastLoginDate; }
            set { _lastLoginDate = value; }
        }

    }
}