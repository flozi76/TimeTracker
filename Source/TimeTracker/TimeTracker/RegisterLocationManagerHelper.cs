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

namespace TimeTracker
{
    using Android.Locations;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Geo;

    public static class RegisterLocationManagerHelper
    {
        public static void RegisterLocationManager(this  LocationManager locationManager, ILocationListener locationListener, ICoreApplicationContext coreApplicationContext)
        {
            var criteria = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Low,
            };
            var bestProvider = locationManager.GetBestProvider(criteria, true);
            var lastKnownLocation = locationManager.GetLastKnownLocation(bestProvider);
            coreApplicationContext.CurrentLocation = lastKnownLocation.ToCoordinate();
            locationManager.RequestLocationUpdates(bestProvider, 1000, 1, locationListener);
        }
    }
}