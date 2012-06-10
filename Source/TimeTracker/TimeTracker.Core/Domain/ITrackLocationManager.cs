namespace TimeTracker.Core.Domain
{
    using System.Collections.Generic;
    using TimeTracker.Core.Domain.Entities;

    public interface ITrackLocationManager
    {
        TrackLocation GetLocation(int id);
        IEnumerable<TrackLocation> GetTrackLocations();
        int SaveCurrentLocation(TrackLocation item);
        int DeleteTrackLocation(int id);
    }
}