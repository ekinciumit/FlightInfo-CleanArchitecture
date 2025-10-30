namespace FlightInfo.Shared.DTOs
{
    public class AirportDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public int CityId { get; set; }
    }
}



