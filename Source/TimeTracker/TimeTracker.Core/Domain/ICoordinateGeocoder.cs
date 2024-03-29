namespace TimeTracker.Core.Domain
{
    using System.Collections.Generic;
    using Android.Locations;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public interface ICoordinateGeocoder
    {
        IList<TrackLocation> GenerateListOfTrackLocations(Coordinate currentLocation, Geocoder geocoder);
    }
}