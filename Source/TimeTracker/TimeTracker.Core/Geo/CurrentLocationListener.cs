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

namespace TimeTracker.Core.Geo
{
    using Android.Locations;
    using TimeTracker.Core.Domain;

    public class CurrentLocationListener : ILocationListener
    {
        private readonly ICoreApplicationContext coreApplicationContext;
        private readonly LocationManager locationManager;
        private readonly Context applicationContext;

        public CurrentLocationListener(ICoreApplicationContext coreApplicationContext, LocationManager locationManager, Context applicationContext)
        {
            this.coreApplicationContext = coreApplicationContext;
            this.locationManager = locationManager;
            this.applicationContext = applicationContext;

            this.InitializeLocationListener();
        }

        private void InitializeLocationListener()
        {
            var criteria = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Low,
            };

            string bestProvider = this.locationManager.GetBestProvider(criteria, true);
            Location lastKnownLocation = this.locationManager.GetLastKnownLocation(bestProvider);
            this.coreApplicationContext.CurrentLocation = lastKnownLocation.ToCoordinate();
            this.locationManager.RequestLocationUpdates(bestProvider, 1000, 1, this);
        }

        public void Dispose()
        {
        }

        public IntPtr Handle
        {
            get { return this.applicationContext.Handle; }
        }

        public void OnLocationChanged(Location location)
        {
            this.coreApplicationContext.CurrentLocation = location.ToCoordinate();
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }
    }
}