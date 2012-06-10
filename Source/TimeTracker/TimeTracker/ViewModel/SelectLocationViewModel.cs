using System.Collections.Generic;

namespace TimeTracker.ViewModel
{
    using Android.Locations;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Domain.Entities;

    public class SelectLocationViewModel : ISelectLocationViewModel
    {
        private readonly ICoreApplicationContext coreApplicationContext;

        public SelectLocationViewModel(ICoreApplicationContext coreApplicationContext)
        {
            this.coreApplicationContext = coreApplicationContext;
        }

        IList<TrackLocation> ISelectLocationViewModel.ResolveCurrentLocations(Geocoder geoCoder)
        {
            IList<TrackLocation> locations = this.coreApplicationContext.GetListOfCurrentTrackLocationsToAdd(geoCoder);

            return locations;
        }
    }
}