using System.Collections.Generic;

namespace TimeTracker.Core.Domain
{
    using System;
    using Android.Locations;
    using Android.Util;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public class CoreCoreApplicationContext : ICoreApplicationContext
    {
        private readonly ICoordinateGeocoder coordinateGeocoder;
        private readonly ITimeTrackerWorkspace timeTrackerWorkspace;
        private TrackLocation currentTrackLocation;

        public CoreCoreApplicationContext(ICoordinateGeocoder coordinateGeocoder, ITimeTrackerWorkspace timeTrackerWorkspace)
        {
            this.coordinateGeocoder = coordinateGeocoder;
            this.timeTrackerWorkspace = timeTrackerWorkspace;
        }

        public Coordinate CurrentLocation { get; set; }

        public TrackLocation CurrentTrackLocation
        {
            get { return this.currentTrackLocation; }
        }

        /// <summary>
        /// Sets the track location.
        /// </summary>
        /// <param name="trackLocation">The track location.</param>
        public void SetTrackLocation(TrackLocation trackLocation)
        {
            if (trackLocation == this.currentTrackLocation)
            {
                return;
            }

            this.WriteExitPerimeterEvent(this.currentTrackLocation);
            this.currentTrackLocation = trackLocation;
            this.WriteEnterPerimeterEvent(this.currentTrackLocation);
        }

        private void WriteEnterPerimeterEvent(TrackLocation enterTrackLocation)
        {
            Log.Debug(this.GetType().Name, "Enter Perimeter");
            this.timeTrackerWorkspace.SaveTrackLocationLogEntry(new TrackLocationLogEntry { EnterTime = DateTime.Now, TrackLocationId = enterTrackLocation.ID });
        }

        private void WriteExitPerimeterEvent(TrackLocation exitTrackLocation)
        {
            Log.Debug(this.GetType().Name, "Exit Perimeter");
            this.timeTrackerWorkspace.SaveTrackLocationLogEntry(new TrackLocationLogEntry { ExitTime = DateTime.Now, TrackLocationId = exitTrackLocation.ID });
        }

        /// <summary>
        /// Gets the list of current track locations to add.
        /// </summary>
        /// <param name="geocoder">The geocoder.</param>
        /// <returns></returns>
        public IList<TrackLocation> GetListOfCurrentTrackLocationsToAdd(Geocoder geocoder)
        {
            return this.coordinateGeocoder.GenerateListOfTrackLocations(this.CurrentLocation, geocoder);
        }
    }
}