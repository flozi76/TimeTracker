using System;

namespace TimeTracker.Core.Domain.Entities
{
    using TimeTracker.Core.Database.SQLite;

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
        public DateTime EnterTime { get; set; }

        /// <summary>
        /// Gets or sets the exit time.
        /// </summary>
        /// <value>The exit time.</value>
        public DateTime ExitTime { get; set; }
    }
}