namespace FlightInfo.Shared.Enums
{
    /// <summary>
    /// Flight status enumeration
    /// </summary>
    public enum FlightStatus
    {
        /// <summary>
        /// Flight is scheduled
        /// </summary>
        Scheduled = 0,

        /// <summary>
        /// Flight is boarding
        /// </summary>
        Boarding = 1,

        /// <summary>
        /// Flight is departed
        /// </summary>
        Departed = 2,

        /// <summary>
        /// Flight has arrived
        /// </summary>
        Arrived = 3,

        /// <summary>
        /// Flight is delayed
        /// </summary>
        Delayed = 4,

        /// <summary>
        /// Flight is cancelled
        /// </summary>
        Cancelled = 5,

        /// <summary>
        /// Flight is completed
        /// </summary>
        Completed = 6
    }
}


