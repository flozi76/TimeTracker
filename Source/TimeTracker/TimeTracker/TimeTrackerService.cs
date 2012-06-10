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

    [Service]
    public class TimeTrackerService : Service, ILocationListener
    {
        public override IBinder OnBind(Intent intent)
        {
            Log.Warn("SimpleService", "On bind");
            throw new NotImplementedException();
        }

        System.Threading.Timer timer;

        public override void OnStart(Android.Content.Intent intent, int startId)
        {

            base.OnStart(intent, startId);
            Log.Warn("SimpleService", "SimpleService started");
            DoStuff();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.timer.Dispose();
            Log.Warn("SimpleService", "SimpleService stopped");
        }

        public void DoStuff()
        {
            this.timer = new System.Threading.Timer((o) =>
                this.PeriodCheck(), null, 0, 4000);
        }

        private void PeriodCheck()
        {
            Log.Debug("TimeTrackerService", "hello from simple service");
        }

        public void OnLocationChanged(Location location)
        {
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
