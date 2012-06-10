namespace TimeTracker.Core.Domain
{
    using System.Collections.Generic;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public interface ITrackLocationManager
    {
        TrackLocation GetLocation(int id);
        IEnumerable<TrackLocation> GetTrackLocations();
        int SaveCurrentLocation(TrackLocation item);
        int DeleteTrackLocation(int id);
        TrackLocation GetTrackLocationForCoordinate(Coordinate coordinate);
    }
}