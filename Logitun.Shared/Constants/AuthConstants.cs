using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitun.Shared.Constants
{
    public static class AuthConstants
    {
        public const string ROLE_USER = "ROLE_USER";
        public const string ROLE_ADMIN = "ROLE_ADMIN";
        public const string ROLE_DRIVER = "ROLE_DRIVER";

        public const string USER_TYPE_USER = "USER";
        public const string USER_TYPE_ADMIN = "ADMIN";
        public const string USER_TYPE_DRIVER = "DRIVER";

        public const string JWT_COOKIE_NAME = "jwt-auth";
    }
}
