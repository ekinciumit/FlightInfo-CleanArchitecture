namespace FlightInfo.Domain.Enums
{
    public static class UserRoleEnum
    {
        public const string User = "User";
        public const string Admin = "Admin";

        public static readonly string[] All = { User, Admin };

        public static bool IsValid(string role)
        {
            return All.Contains(role);
        }

        public static string GetDefault()
        {
            return User;
        }
    }
}


