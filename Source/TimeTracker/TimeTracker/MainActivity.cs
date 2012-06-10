using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Widget;
using Java.IO;

namespace TimeTracker
{
    using Android.Content;
    using Android.Locations;
    using Android.Util;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Geo;

    [Activity(Label = "TimeTracker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button addCurrentLocationButton;
        private TextView listLocationsText;
        private Button startListenServiceButton;
        private Button stopListenServiceButton;
        private Button showLogEntries;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            this.BindElements();

            var trackLocations = CentralStation.Instance.Ainject.ResolveType<ITimeTrackerWorkspace>().GetTrackLocations();
            var stringBuilderList = new StringBuilder();
            foreach (var trackLocation in trackLocations)
            {
                stringBuilderList.AppendLine(string.Format("{0} {1} {2} {3} {4}", trackLocation.ID, trackLocation.PostalCode, trackLocation.City, trackLocation.Street, trackLocation.HouseNumber));
                stringBuilderList.AppendLine(string.Format("Lat: {0} Lon: {1}", trackLocation.Latitude, trackLocation.Longitude));
            }

            this.listLocationsText.Text = stringBuilderList.ToString();

            this.RegisterActions();

        }

        private void RegisterActions()
        {
            this.addCurrentLocationButton.Click += delegate
                                                       {
                                                           var intent = new Intent(this, typeof(SelectLocationActivity));
                                                           StartActivity(intent);
                                                       };

            this.showLogEntries.Click += delegate
                                             {
                                                 var intent = new Intent(this, typeof(LogEntriesListActivity));
                                                 StartActivity(intent);
                                             };

            this.startListenServiceButton.Click += delegate
                                                       {
                                                           var intent = new Intent(this, typeof(TimeTrackerService));
                                                           this.StartService(intent);
                                                       };

            this.stopListenServiceButton.Click += delegate
            {
                var intent = new Intent(this, typeof(TimeTrackerService));
                this.StopService(intent);
            };
        }

        private void BindElements()
        {
            this.listLocationsText = FindViewById<TextView>(Resource.Id.ListLocations);
            this.addCurrentLocationButton = FindViewById<Button>(Resource.Id.btnTrackcCurrentLocation);
            this.startListenServiceButton = FindViewById<Button>(Resource.Id.btnStartLocationListener);
            this.stopListenServiceButton = FindViewById<Button>(Resource.Id.btnStopLocationListener);
            this.showLogEntries = FindViewById<Button>(Resource.Id.btnShowLogEntries);
        }
    }
}

