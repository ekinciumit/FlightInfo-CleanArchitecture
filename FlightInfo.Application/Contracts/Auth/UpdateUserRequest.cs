namespace FlightInfo.Application.Contracts.Auth
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        // Email düzenlenemez - güvenlik nedeniyle
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Phone { get; set; }
    }
}


