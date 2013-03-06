using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class ASFUser
    {
        private Guid _userId;
        private MembershipInfo _membershipInfo;
        private string _username;
        private string _fullName;
        private string _emailAddress;
        private int _isAdmin;

        public enum LazyComponents
        {
            LOAD_MEMBERSHIP_INFO
        };

        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public MembershipInfo MembershipInfo
        {
            get { return _membershipInfo; }
            set { _membershipInfo = value; }
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

        public int IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }

        public ASFUser()
        {
        }

        public bool ValidateASFUser()
        {
            if (_username == null || _fullName == null)
                return false;
            else
                return true;
        }
    }
}