

namespace TimeTracker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Content;
    using Android.Locations;
    using Android.OS;
    using Android.Util;
    using Android.Widget;
    using TimeTracker.Adapters;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;
    using TimeTracker.ViewModel;

    [Activity(Label = "Select Location")]
    public class SelectLocationActivity : Activity, ILocationListener
    {
        private ISelectLocationViewModel viewModel;
        private ListView listView;
        private ICoreApplicationContext coreApplicationContext;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SelectLocation);

            var locationManager = (LocationManager)this.GetSystemService(LocationService);
            var geoCoder = new Geocoder(this);
            this.listView = this.FindViewById<ListView>(Resource.Id.listViewSelectLocations);

            try
            {
                this.coreApplicationContext = CentralStation.Instance.Ainject.ResolveType<ICoreApplicationContext>();
                locationManager.RegisterLocationManager(this, this.coreApplicationContext);

                this.viewModel = CentralStation.Instance.Ainject.ResolveType<ISelectLocationViewModel>();
                IList<TrackLocation> currentLocations = this.viewModel.ResolveCurrentLocations(geoCoder);

                this.listView.Adapter = new TrackLocationListAdapter(this, currentLocations);
                this.listView.TextFilterEnabled = true;

                this.listView.ItemClick += (sender, e) =>
                {
                    var backToMain = new Intent(this, typeof(MainActivity));
                    //backToMain.PutExtra("TaskID", this._tasks[e.Position].ID);
                    var item = currentLocations[e.Position];

                    CentralStation.Instance.Ainject.ResolveType<ITimeTrackerWorkspace>().SaveTrackLocation(item);

                    this.StartActivity(backToMain);
                };
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().Name, ex.StackTrace);
            }
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