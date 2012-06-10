namespace TimeTracker.ViewModel
{
    using System.Collections.Generic;
    using Android.Locations;
    using TimeTracker.Core.Domain.Entities;

    public interface ISelectLocationViewModel
    {
        IList<TrackLocation> ResolveCurrentLocations(Geocoder geoCoder);
    }
}