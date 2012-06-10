using System.Collections.Generic;

namespace TimeTracker.Core.Database
{
    using System.Linq;
    using TimeTracker.Core.Database.SQLite;
    using TimeTracker.Core.Domain.Entities;

    /// <summary>
    /// TrackLocationsDatabase builds on SQLite.Net and represents a specific database, in our case, the TrackLocation DB.
    /// It contains methods for retreival and persistance as well as db creation, all based on the 
    /// underlying ORM.
    /// </summary>
    public class TrackLocationsDatabase : SQLiteConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackLocationy.DL.TrackLocationDatabase"/> TrackLocationsDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        public TrackLocationsDatabase(string path)
            : base(path)
        {
            // create the tables
            CreateTable<TrackLocation>();
        }

        //TODO: make these methods generic, Add<T>(item), etc.

        public IEnumerable<TrackLocation> GetLocations()
        {
            return (from i in this.Table<TrackLocation>() select i);
        }

        public TrackLocation GetLocation(int id)
        {
            return (from i in Table<TrackLocation>()
                    where i.ID == id
                    select i).FirstOrDefault();
        }

        public int SaveLocation(TrackLocation item)
        {
            if (item.ID != 0)
            {
                base.Update(item);
                return item.ID;
            }
            else
            {
                return base.Insert(item);
            }
        }

        public int DeleteLocation(int id)
        {
            return base.Delete<TrackLocation>(new TrackLocation() { ID = id });
        }
    }
}

