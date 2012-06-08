namespace TimeTracker.Core.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using TimeTracker.Core.Database;

    public class TaskManager
    {
        TaskDatabase _db = null;
        protected static string _dbLocation;
        protected static TaskManager _me;

        static TaskManager()
        {
            _me = new TaskManager();
        }

        protected TaskManager()
        {
            // set the db location
            //_dbLocation = Path.Combine (NSBundle.MainBundle.BundlePath, "Library/TaskDB.db3");
            _dbLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TaskDB.db3");
            // instantiate the database	
            this._db = new TaskDatabase(_dbLocation);
        }

        public static Task GetTask(int id)
        {
            return _me._db.GetTask(id);
        }

        public static IEnumerable<Task> GetTasks()
        {
            return _me._db.GetTasks();
        }

        public static int SaveTask(Task item)
        {
            return _me._db.SaveTask(item);
        }

        public static int DeleteTask(int id)
        {
            return _me._db.DeleteTask(id);
        }

    }
}

