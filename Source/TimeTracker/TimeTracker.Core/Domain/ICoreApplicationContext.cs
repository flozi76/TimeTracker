namespace TimeTracker.Core.Domain
{
    using System.Collections.Generic;
    using Android.Locations;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public interface ICoreApplicationContext
    {
        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        /// <value>The current location.</value>
        Coordinate CurrentLocation { get; set; }

        /// <summary>
        /// Gets the last track location.
        /// </summary>
        /// <value>The last track location.</value>
        TrackLocation CurrentTrackLocation { get; }

        /// <summary>
        /// Gets the list of current track locations to add.
        /// </summary>
        /// <param name="geocoder">The geocoder.</param>
        /// <returns></returns>
        IList<TrackLocation> GetListOfCurrentTrackLocationsToAdd(Geocoder geocoder);

        /// <summary>
        /// Sets the track location.
        /// </summary>
        /// <param name="trackLocation">The track location.</param>
        void SetTrackLocation(TrackLocation trackLocation);
    }
}