namespace TimeTracker.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using TimeTracker.Core.Database;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public class TimeTrackerWorkspace : ITimeTrackerWorkspace
    {
        private readonly IDistanceCalculator distanceCalculator;
        private readonly TimeTrackerDatabase database;
        private readonly string databaseLocation;

        public TimeTrackerWorkspace(IDistanceCalculator distanceCalculator)
        {
            this.distanceCalculator = distanceCalculator;

            // set the db location
            //databaseLocation = Path.Combine (NSBundle.MainBundle.BundlePath, "Library/TrackLocationDB.db3");
            this.databaseLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "LocationsDb.db3");

            // instantiate the database	
            this.database = new TimeTrackerDatabase(this.databaseLocation);
        }

        /// <summary>
        /// Saves the track location log entry.
        /// </summary>
        /// <param name="trackLocationLogEntry">The track location log entry.</param>
        /// <returns></returns>
        public int SaveTrackLocationLogEntry(TrackLocationLogEntry trackLocationLogEntry)
        {
            return this.database.SateEntity(trackLocationLogEntry);
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public TrackLocation GetLocation(int id)
        {
            return this.database.GetEntry<TrackLocation>(id);
        }

        /// <summary>
        /// Gets the track locations.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TrackLocation> GetTrackLocations()
        {
            return this.database.GetAll<TrackLocation>();
        }

        /// <summary>
        /// Gets the track location log entries.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TrackLocationLogEntry> GetTrackLocationLogEntries()
        {
            return this.database.GetAll<TrackLocationLogEntry>();
        }

        /// <summary>
        /// Saves the track location.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int SaveTrackLocation(TrackLocation item)
        {
            return this.database.SateEntity(item);
        }

        /// <summary>
        /// Deletes the track location.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public int DeleteTrackLocation(int id)
        {
            return this.database.DeleteLocation(id);
        }

        /// <summary>
        /// Gets the track location for coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns></returns>
        public TrackLocation GetTrackLocationForCoordinate(Coordinate coordinate)
        {
            var locations = this.GetTrackLocations();

            return (from trackLocation in locations
                    let distance = this.distanceCalculator.Distance(coordinate, trackLocation.ToCoordinate(), UnitsOfLength.Meter)
                    where distance < Constants.PerimeterDistance
                    select trackLocation).FirstOrDefault();
        }
    }
}

