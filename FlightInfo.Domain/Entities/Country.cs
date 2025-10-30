using System.Collections.Generic;

namespace FlightInfo.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Code { get; set; } = ""; // E.g., TR, DE, US
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}

