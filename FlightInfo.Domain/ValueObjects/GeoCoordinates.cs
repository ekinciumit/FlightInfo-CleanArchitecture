namespace FlightInfo.Domain.ValueObjects
{
    /// <summary>
    /// Geographic coordinates value object
    /// </summary>
    public class GeoCoordinates
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinates(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90 degrees", nameof(latitude));

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180 degrees", nameof(longitude));

            Latitude = latitude;
            Longitude = longitude;
        }

        public double DistanceTo(GeoCoordinates other)
        {
            const double earthRadius = 6371; // Earth's radius in kilometers

            var dLat = ToRadians(other.Latitude - Latitude);
            var dLon = ToRadians(other.Longitude - Longitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";

        public override bool Equals(object? obj)
        {
            if (obj is GeoCoordinates other)
                return Math.Abs(Latitude - other.Latitude) < 0.000001 &&
                       Math.Abs(Longitude - other.Longitude) < 0.000001;
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    }
}


