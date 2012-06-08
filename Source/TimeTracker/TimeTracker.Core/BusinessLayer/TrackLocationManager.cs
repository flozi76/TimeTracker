namespace TimeTracker.Core.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using TimeTracker.Core.Database;

    public class TrackLocationManager
    {
        private readonly TrackLocationsDatabase database;
        private readonly string databaseLocation;
        private static TrackLocationManager trackLocationManager;

        private TrackLocationManager()
        {
            // set the db location
            //databaseLocation = Path.Combine (NSBundle.MainBundle.BundlePath, "Library/TaskDB.db3");
            this.databaseLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "LocationsDb.db3");

            // instantiate the database	
            this.database = new TrackLocationsDatabase(databaseLocation);
        }

        public static TrackLocationManager Instance
        {
            get { return trackLocationManager ?? (trackLocationManager = new TrackLocationManager()); }
        }

        public TrackLocation GetLocation(int id)
        {
            return database.GetLocation(id);
        }

        public IEnumerable<TrackLocation> GetTrackLocations()
        {
            return database.GetLocations();
        }

        public int SaveCurrentLocation(TrackLocation item)
        {
            return database.SaveLocation(item);
        }

        public int DeleteTask(int id)
        {
            return database.DeleteLocation(id);
        }
    }
}

