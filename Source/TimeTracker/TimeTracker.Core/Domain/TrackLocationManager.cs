namespace TimeTracker.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.Core.Database;

    public class TrackLocationManager : ITrackLocationManager
    {
        private readonly TrackLocationsDatabase database;
        private readonly string databaseLocation;

        public TrackLocationManager()
        {
            // set the db location
            //databaseLocation = Path.Combine (NSBundle.MainBundle.BundlePath, "Library/TrackLocationDB.db3");
            this.databaseLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "LocationsDb.db3");

            // instantiate the database	
            this.database = new TrackLocationsDatabase(this.databaseLocation);
        }

        public TrackLocation GetLocation(int id)
        {
            return this.database.GetLocation(id);
        }

        public IEnumerable<TrackLocation> GetTrackLocations()
        {
            return this.database.GetLocations();
        }

        public int SaveCurrentLocation(TrackLocation item)
        {
            return this.database.SaveLocation(item);
        }

        public int DeleteTrackLocation(int id)
        {
            return this.database.DeleteLocation(id);
        }
    }
}

