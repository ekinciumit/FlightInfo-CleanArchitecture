namespace FlightInfo.Domain.Enums
{
    public static class ReservationStatusEnum
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Cancelled = "Cancelled";
        public const string Completed = "Completed";

        public static readonly string[] All = { Pending, Confirmed, Cancelled, Completed };

        public static bool IsValid(string status)
        {
            return All.Contains(status);
        }

        public static string GetDefault()
        {
            return Pending;
        }

        public static bool IsActive(string status)
        {
            return status == Pending || status == Confirmed;
        }

        public static bool IsCompleted(string status)
        {
            return status == Completed || status == Cancelled;
        }
    }
}


