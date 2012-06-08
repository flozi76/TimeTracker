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
    }
}

