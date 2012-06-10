namespace TimeTracker.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using TimeTracker.Core.Database;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public class TrackLocationManager : ITrackLocationManager
    {
        private readonly IDistanceCalculator distanceCalculator;
        private readonly TimeTrackerDatabase database;
        private readonly string databaseLocation;

        public TrackLocationManager(IDistanceCalculator distanceCalculator)
        {
            this.distanceCalculator = distanceCalculator;
            // set the db location
            //databaseLocation = Path.Combine (NSBundle.MainBundle.BundlePath, "Library/TrackLocationDB.db3");
            this.databaseLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "LocationsDb.db3");

            // instantiate the database	
            this.database = new TimeTrackerDatabase(this.databaseLocation);
        }

        public TrackLocation GetLocation(int id)
        {
            return this.database.GetEntry<TrackLocation>(id);
        }

        public IEnumerable<TrackLocation> GetTrackLocations()
        {
            return this.database.GetAll<TrackLocation>();
        }

        public int SaveCurrentLocation(TrackLocation item)
        {
            return this.database.SaveLocation(item);
        }

        public int DeleteTrackLocation(int id)
        {
            return this.database.DeleteLocation(id);
        }

        public TrackLocation GetTrackLocationForCoordinate(Coordinate coordinate)
        {
            var locations = this.GetTrackLocations();

            foreach (var trackLocation in locations)
            {
                var distance = distanceCalculator.Distance(coordinate, trackLocation.ToCoordinate(), UnitsOfLength.Meter);
                if (distance < Constants.PerimeterDistance)
                {
                    // All OK still in perimeter of current TrackLocation
                    return trackLocation;
                }
            }

            return null;
        }
    }
}

