namespace TimeTracker.Core.BusinessLayer
{
    using TimeTracker.Core.Database.SQLite;

    /// <summary>
    /// Represents a TrackLocation.
    /// </summary>
    public partial class TrackLocation
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public new string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}\n Lat: '{5}' Lon: '{6}'", Name, Street, HouseNumber, PostalCode, City, Latitude, Longitude);
        }

    }
}

