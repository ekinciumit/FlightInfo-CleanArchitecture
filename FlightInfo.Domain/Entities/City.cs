namespace FlightInfo.Domain.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = ""; // E.g., IST, ANK, IZM
        public int CountryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Country Country { get; set; } = null!;
        public ICollection<Airport> Airports { get; set; } = new List<Airport>();
    }
}

