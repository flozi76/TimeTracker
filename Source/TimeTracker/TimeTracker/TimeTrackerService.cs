using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTracker
{
    using Android.App;
    using Android.Content;
    using Android.Locations;
    using Android.OS;
    using Android.Util;
    using Android.Widget;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Geo;

    [Service]
    public class TimeTrackerService : Service, ILocationListener
    {
        public override IBinder OnBind(Intent intent)
        {
            Log.Warn("SimpleService", "On bind");
            throw new NotImplementedException();
        }

        System.Threading.Timer timer;
        private ICoreApplicationContext coreApplicationContext;
        private IPerimeterWatchDog perimeterWatchDog;

        public override void OnStart(Android.Content.Intent intent, int startId)
        {
            base.OnStart(intent, startId);

            try
            {
                var locationManager = (LocationManager)this.GetSystemService(LocationService);
                //var geoCoder = new Geocoder(this);
                this.coreApplicationContext = CentralStation.Instance.Ainject.ResolveType<ICoreApplicationContext>();
                this.perimeterWatchDog = CentralStation.Instance.Ainject.ResolveType<IPerimeterWatchDog>();

                locationManager.RegisterLocationManager(this, this.coreApplicationContext);

                this.StartPerimeterWatchDog();

            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().Name, ex.StackTrace);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                this.timer.Dispose();
                Log.Warn("SimpleService", "SimpleService stopped");
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().Name, ex.StackTrace);
            }
        }

        private void StartPerimeterWatchDog()
        {
            this.timer = new System.Threading.Timer(o =>
                this.perimeterWatchDog.CheckPerimeter(), null, 0, 4000);
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                this.coreApplicationContext.CurrentLocation = location.ToCoordinate();
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().Name, ex.StackTrace);
            }
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
