namespace TimeTracker.Core.Domain
{
    using System.Collections.Generic;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public interface ITimeTrackerWorkspace
    {
        TrackLocation GetLocation(int id);
        IEnumerable<TrackLocation> GetTrackLocations();
        int SaveTrackLocation(TrackLocation item);
        int DeleteTrackLocation(int id);
        TrackLocation GetTrackLocationForCoordinate(Coordinate coordinate);

        /// <summary>
        /// Saves the track location log entry.
        /// </summary>
        /// <param name="trackLocationLogEntry">The track location log entry.</param>
        /// <returns></returns>
        int SaveTrackLocationLogEntry(TrackLocationLogEntry trackLocationLogEntry);
    }
}