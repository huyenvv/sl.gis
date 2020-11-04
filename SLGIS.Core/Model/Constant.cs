using System.Collections.Generic;

namespace SLGIS.Core
{
    public static class Constant
    {
        public static class Role
        {
            public const string SupperAdmin = "SupperAdmin";
            public const string Admin = "Admin";
            public const string Member = "Member";
            public static List<string> All
            {
                get
                {
                    return new List<string> { SupperAdmin, Admin, Member };
                }
            }
        }

        public static class User
        {
            public const string Admin = "admin";
        }
    }
}
