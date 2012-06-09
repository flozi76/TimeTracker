namespace TimeTracker.ViewModel
{
    using System.Collections.Generic;
    using Android.Locations;
    using TimeTracker.Core.BusinessLayer;

    public interface ISelectLocationViewModel
    {
        IList<TrackLocation> ResolveCurrentLocations(Geocoder geoCoder);
    }
}