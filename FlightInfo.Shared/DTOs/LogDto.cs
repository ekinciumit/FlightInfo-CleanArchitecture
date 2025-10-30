namespace FlightInfo.Shared.DTOs
{
    public class LogDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }
        public int? FlightId { get; set; }
        public string? UserName { get; set; }
        public string? FlightNumber { get; set; }
        public string? Details { get; set; }
        public string? Data { get; set; }
        public string? Action { get; set; }
        public string? Exception { get; set; }
    }

    public class CreateLogRequest
    {
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? FlightId { get; set; }
    }
}


