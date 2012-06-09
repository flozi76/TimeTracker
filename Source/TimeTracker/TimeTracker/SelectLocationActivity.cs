

namespace TimeTracker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Content;
    using Android.Locations;
    using Android.OS;
    using Android.Widget;
    using TimeTracker.Adapters;
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Geo;
    using TimeTracker.ViewModel;
    using Debug = System.Diagnostics.Debug;

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
                this.RegisterLocationManager(locationManager);

                this.viewModel = CentralStation.Instance.Ainject.ResolveType<ISelectLocationViewModel>();
                IList<TrackLocation> currentLocations = this.viewModel.ResolveCurrentLocations(geoCoder);

                this.listView.Adapter = new TrackLocationListAdapter(this, currentLocations);
                this.listView.TextFilterEnabled = true;

                this.listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
                {
                    var backToMain = new Intent(this, typeof(MainActivity));
                    //backToMain.PutExtra("TaskID", this._tasks[e.Position].ID);
                    var item = currentLocations[e.Position];

                    CentralStation.Instance.Ainject.ResolveType<ITrackLocationManager>().SaveCurrentLocation(item);

                    this.StartActivity(backToMain);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void RegisterLocationManager(LocationManager locationManager)
        {
            var criteria = new Criteria
                               {
                                   Accuracy = Accuracy.Coarse,
                                   PowerRequirement = Power.Low,
                               };
            string bestProvider = locationManager.GetBestProvider(criteria, true);
            Location lastKnownLocation = locationManager.GetLastKnownLocation(bestProvider);
            this.coreApplicationContext.CurrentLocation = lastKnownLocation.ToCoordinate();
            locationManager.RequestLocationUpdates(bestProvider, 1000, 1, this);
        }

        public void OnLocationChanged(Location location)
        {
            this.coreApplicationContext.CurrentLocation = location.ToCoordinate();
        }

        //protected override void OnResume()
        //{
        //    base.OnResume();
        //    this.listView = this.FindViewById<ListView>(Resource.Id.listViewSelectLocations);
        //    var centralStationInstance = new CentralStation(this.ApplicationContext);
        //    try
        //    {
        //        this.viewModel = centralStationInstance.Ainject.ResolveType<ISelectLocationViewModel>();
        //        IList<TrackLocation> currentLocations = this.viewModel.ResolveCurrentLocations();
        //        this.listView.Adapter = new TrackLocationListAdapter(this, currentLocations);
        //        this.listView.TextFilterEnabled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //}

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