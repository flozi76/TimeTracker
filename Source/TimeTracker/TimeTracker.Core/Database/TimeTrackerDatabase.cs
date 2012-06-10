using System.Collections.Generic;

namespace TimeTracker.Core.Database
{
    using System.Linq;
    using TimeTracker.Core.Database.SQLite;
    using TimeTracker.Core.Domain.Entities;

    /// <summary>
    /// TimeTrackerDatabase builds on SQLite.Net and represents a specific database, in our case, the TrackLocation DB.
    /// It contains methods for retreival and persistance as well as db creation, all based on the 
    /// underlying ORM.
    /// </summary>
    public class TimeTrackerDatabase : SQLiteConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackLocationy.DL.TrackLocationDatabase"/> TimeTrackerDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        public TimeTrackerDatabase(string path)
            : base(path)
        {
            // create the tables
            CreateTable<TrackLocation>();
            CreateTable<TrackLocationLogEntry>();
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>All entries in the Database</returns>
        public IEnumerable<T> GetAll<T>() where T : new()
        {
            return (from i in this.Table<T>() select i);
        }

        public T GetEntry<T>(int id) where T : class, new()
        {
            foreach (T unknown in (this.Table<T>().Where(i => (i as Entity != null) && (i as Entity).ID == id)))
                return unknown;
            return default(T);
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

