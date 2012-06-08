namespace TimeTracker.Core.BusinessLayer
{
    using System;
    using TimeTracker.Core.Database;

    /// <summary>
    /// Represents a Task.
    /// </summary>
    public partial class Task
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

