namespace FlightInfo.Domain.Entities
{
    public class Airport
    {
        public int Id { get; set; }
        public string Code { get; set; } = ""; // E.g., IST, ESB, ADB
        public string Name { get; set; } = "";
        public string FullName { get; set; } = ""; // E.g., "Istanbul Airport"
        public int CityId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public City City { get; set; } = null!;
    }
}

