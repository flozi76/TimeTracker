using System.Collections.Generic;

namespace TimeTracker.Core.Domain
{
    using System;
    using Android.Locations;
    using Android.Util;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public class CoreApplicationContext : ICoreApplicationContext
    {
        private readonly ICoordinateGeocoder coordinateGeocoder;
        private readonly ITimeTrackerWorkspace timeTrackerWorkspace;
        private TrackLocation currentTrackLocation;

        public CoreApplicationContext(ICoordinateGeocoder coordinateGeocoder, ITimeTrackerWorkspace timeTrackerWorkspace)
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
            if (enterTrackLocation == null)
            {
                return;
            }

            Log.Debug(this.GetType().Name, "Enter Perimeter");
            var entry = new TrackLocationLogEntry { LogEntry = Constants.EntryPerimeter, TrackLocationId = enterTrackLocation.ID, LocationName = enterTrackLocation.LocationName };
            entry.LogDateTime = (DateTime.Now);
            int response = this.timeTrackerWorkspace.SaveTrackLocationLogEntry(entry);
            Log.Error(this.GetType().Name, "Enter Perimeter response" + response);
        }

        private void WriteExitPerimeterEvent(TrackLocation exitTrackLocation)
        {
            if (exitTrackLocation == null)
            {
                return;
            }

            Log.Debug(this.GetType().Name, "Exit Perimeter");
            var entry = new TrackLocationLogEntry { LogEntry = Constants.ExitPerimeter, TrackLocationId = exitTrackLocation.ID, LocationName = exitTrackLocation.LocationName };
            entry.LogDateTime = (DateTime.Now);
            int response = this.timeTrackerWorkspace.SaveTrackLocationLogEntry(entry);
        }

        /// <summary>
        /// Gets the list of current track locations to add.
        /// </summary>
        /// <param name="geocoder">The geocoder.</param>
        /// <returns></returns>
        public IList<TrackLocation> GetListOfCurrentTrackLocationsToAdd(Geocoder geocoder)
        {
            IList<TrackLocation> returnList = new List<TrackLocation>();

            if (this.CurrentLocation != null)
            {
                returnList = this.coordinateGeocoder.GenerateListOfTrackLocations(this.CurrentLocation, geocoder);
            }

            return returnList;
        }
    }
}