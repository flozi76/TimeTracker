using System;

namespace TimeTracker.Core.Domain.Entities
{
    public class TrackLocationLogEntry : Entity
    {
        /// <summary>
        /// Gets or sets the track location id.
        /// </summary>
        /// <value>The track location id.</value>
        public int TrackLocationId { get; set; }

        /// <summary>
        /// Gets or sets the enter time.
        /// </summary>
        /// <value>The enter time.</value>
        public DateTime LogDateTime { get; set; }

        /// <summary>
        /// Gets or sets the log entry.
        /// </summary>
        /// <value>The log entry.</value>
        public string LogEntry { get; set; }

        public string LocationName { get; set; }

        ///// <summary>
        ///// Sets the log date time.
        ///// </summary>
        ///// <param name="logDateTime">The log date time.</param>
        //public void SetLogDateTime(DateTime logDateTime)
        //{
        //    this.LogDateTime = logDateTime.ToLongDateString();
        //}

        ///// <summary>
        ///// Gets the date time.
        ///// </summary>
        ///// <returns></returns>
        //public DateTime GetDateTime()
        //{

        //    return DateTime.Parse(this.LogDateTime);
        //}
    }
}