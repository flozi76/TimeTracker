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
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.Core.Geo;

    public class CoordinateGeocoder : ICoordinateGeocoder
    {
        public IList<TrackLocation> GenerateListOfTrackLocations(Coordinate currentLocation, Geocoder geocoder)
        {
            var listAddresses = geocoder.GetFromLocation(currentLocation.Latitude, currentLocation.Longitude, 30);
            IList<TrackLocation> returnList = new List<TrackLocation>();

            returnList.Add(new TrackLocation
                               {
                                   Latitude = currentLocation.Latitude,
                                   Longitude = currentLocation.Longitude
                               });

            foreach (var address in listAddresses)
            {
                returnList.Add(new TrackLocation
                                   {
                                       City = address.Locality,
                                       Country = address.CountryName,
                                       PostalCode = address.PostalCode,
                                       Street = address.Thoroughfare,
                                       HouseNumber = address.FeatureName,
                                       Latitude = address.Latitude,
                                       Longitude = address.Longitude
                                   });
            }

            return returnList;
        }
    }
}