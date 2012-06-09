namespace TimeTracker.Core.Domain
{
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.Core.Geo;

    public interface ICoreApplicationContext
    {
        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        /// <value>The current location.</value>
        Coordinate CurrentLocation { get; set; }

        TrackLocation GetCurrentTrackLocation();
    }
}