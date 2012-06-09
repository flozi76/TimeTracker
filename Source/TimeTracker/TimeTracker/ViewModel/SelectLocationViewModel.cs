using System.Collections.Generic;

namespace TimeTracker.ViewModel
{
    using Android.Locations;
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.Core.Domain;

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

            //IList<TrackLocation> locations = new List<TrackLocation>();

            //locations.Add(this.coreApplicationContext.GetCurrentTrackLocation());

            //for (int i = 0; i < 10; i++)
            //{
            //    locations.Add(new TrackLocation
            //                      {
            //                          Name = string.Format("location: {0}", i)
            //                      });
            //}

            return locations;
        }
    }
}