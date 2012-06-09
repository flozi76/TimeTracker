namespace TimeTracker.ViewModel
{
    using System.Collections.Generic;
    using TimeTracker.Core.BusinessLayer;

    public interface ISelectLocationViewModel
    {
        IList<TrackLocation> ResolveCurrentLocations();
    }
}