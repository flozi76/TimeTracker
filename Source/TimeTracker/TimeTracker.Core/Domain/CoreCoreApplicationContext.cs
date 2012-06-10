using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TimeTracker.Core.Domain
{
    using Android.Locations;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public class CoreCoreApplicationContext : ICoreApplicationContext
    {
        private readonly ICoordinateGeocoder coordinateGeocoder;

        public CoreCoreApplicationContext(ICoordinateGeocoder coordinateGeocoder)
        {
            this.coordinateGeocoder = coordinateGeocoder;
        }

        public Coordinate CurrentLocation { get; set; }

        public TrackLocation LastTrackLocation { get; set; }

        public IList<TrackLocation> GetListOfCurrentTrackLocationsToAdd(Geocoder geocoder)
        {
            return this.coordinateGeocoder.GenerateListOfTrackLocations(this.CurrentLocation, geocoder);
        }
    }
}