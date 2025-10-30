namespace FlightInfo.Domain.Enums
{
    public static class FlightStatusEnum
    {
        public const string Scheduled = "Scheduled";
        public const string OnTime = "OnTime";
        public const string Delayed = "Delayed";
        public const string Cancelled = "Cancelled";
        public const string Completed = "Completed";
        public const string InProgress = "InProgress";
        public const string Boarding = "Boarding";

        public static readonly string[] All = { Scheduled, OnTime, Delayed, Cancelled, Completed, InProgress, Boarding };

        public static bool IsValid(string status)
        {
            return All.Contains(status);
        }

        public static string GetDefault()
        {
            return Scheduled;
        }

        public static bool IsActive(string status)
        {
            return status == Scheduled || status == OnTime || status == Boarding || status == InProgress;
        }

        public static bool IsCompleted(string status)
        {
            return status == Completed || status == Cancelled;
        }
    }
}


