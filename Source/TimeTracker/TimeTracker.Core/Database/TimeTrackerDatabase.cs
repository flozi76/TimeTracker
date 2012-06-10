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

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The id.</param>
        /// <returns>the entry with the given id</returns>
        public T GetEntry<T>(int id) where T : class, new()
        {
            return (this.Table<T>().Where(i => (i as Entity != null) && (i as Entity).ID == id)).FirstOrDefault();
        }

        /// <summary>
        /// Sates the entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int SateEntity<T>(T item)
        {
            var entity = item as Entity;
            if (entity != null && entity.ID != 0)
            {
                this.Update(entity);
                return entity.ID;
            }

            return this.Insert(item);
        }

        /// <summary>
        /// Deletes the location.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public int DeleteLocation(int id)
        {
            return this.Delete(new TrackLocation() { ID = id });
        }
    }
}

