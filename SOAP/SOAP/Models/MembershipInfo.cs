using System;
using System.Collections.Generic;

namespace SOAP.Models
{
    public class MembershipInfo
    {
        private int _id;
        private string _username;
        private string password;
        private System.Web.Security.MembershipPasswordFormat _passwordFormat;
        private string _passwordSalt;
        private DateTime _lastLoginDate;
        private int _isApproved;
        private int _isLockedOut;
        private DateTime _createDate;
        private DateTime _lastPasswordChangedDate;
        private DateTime _lastLockoutDate;
        private int _failedPasswordAttemptCount;
        private DateTime _failedPasswordAttemptWindowStart;
        private int _failedPasswordAnswerAttemptCount;
        private DateTime _failedPasswordAnswerAttemptWindowStart;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
            set { _passwordFormat = value; }
        }

        public string PasswordSalt
        {
            get { return _passwordSalt; }
            set { _passwordSalt = value; }
        }

        public int IsLockedOut
        {
            get { return _isLockedOut; }
            set { _isLockedOut = value; }
        }

        public DateTime LastLoginDate
        {
            get { return _lastLoginDate; }
            set { _lastLoginDate = value; }
        }

        public int IsApproved
        {
            get { return _isApproved; }
            set { _isApproved = value; }
        }

        public DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        public DateTime LastPasswordChangedDate
        {
            get { return _lastPasswordChangedDate; }
            set { _lastPasswordChangedDate = value; }
        }

        public DateTime LastLockoutDate
        {
            get { return _lastLockoutDate; }
            set { _lastLockoutDate = value; }
        }

        public int FailedPasswordAttemptCount
        {
            get { return _failedPasswordAttemptCount; }
            set { _failedPasswordAttemptCount = value; }
        }

        public DateTime FailedPasswordAttemptWindowStart
        {
            get { return _failedPasswordAttemptWindowStart; }
            set { _failedPasswordAttemptWindowStart = value; }
        }

        public int FailedPasswordAnswerAttemptCount
        {
            get { return _failedPasswordAnswerAttemptCount; }
            set { _failedPasswordAnswerAttemptCount = value; }
        }

        public DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get { return _failedPasswordAnswerAttemptWindowStart; }
            set { _failedPasswordAnswerAttemptWindowStart = value; }
        }

        public MembershipInfo()
        {
            _lastLoginDate = DateTime.Now;
            _isApproved = 1;
            _isLockedOut = 0;
            _passwordFormat = System.Web.Security.MembershipPasswordFormat.Hashed;
            _passwordSalt = "General";
            _createDate = DateTime.Now;
            _lastPasswordChangedDate = DateTime.Now;
            _lastLockoutDate = DateTime.Now;
            _failedPasswordAnswerAttemptCount = 0;
            _failedPasswordAnswerAttemptWindowStart = DateTime.Now;
            _failedPasswordAttemptCount = 0;
            _failedPasswordAttemptWindowStart = DateTime.Now;
        }
    }
}