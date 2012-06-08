
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Widget;
using Java.IO;

namespace TimeTracker
{
    using Android.Locations;
    using TimeTracker.Core.BusinessLayer;

    [Activity(Label = "TimeTracker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainControllerActivity : Activity, ILocationListener
    {
        private TextView locationText;
        private LocationManager locationManager;
        private StringBuilder stringBuilder;
        private Geocoder geocoder;
        private Button addButton;
        private TextView listLocationsText;
        private TrackLocation currentLocation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            this.locationText = FindViewById<TextView>(Resource.Id.TextLocation);
            this.listLocationsText = FindViewById<TextView>(Resource.Id.ListLocations);
            this.addButton = FindViewById<Button>(Resource.Id.buttonToDatabase);

            this.addButton.Click += delegate
                                        {
                                            var tasks = TrackLocationManager.Instance.GetTrackLocations();
                                            var stringBuilderList = new StringBuilder();
                                            foreach (var trackLocation in tasks)
                                            {
                                                stringBuilderList.AppendLine(string.Format("{0} {1} {2} {3} {4}", trackLocation.ID, trackLocation.PostalCode, trackLocation.City, trackLocation.Street, trackLocation.HouseNumber));
                                                stringBuilderList.AppendLine(string.Format("Lat: {0} Lon: {1}", trackLocation.Latitude, trackLocation.Longitude));
                                            }

                                            this.listLocationsText.Text = stringBuilderList.ToString();

                                        };

            this.stringBuilder = new StringBuilder();
            this.geocoder = new Geocoder(this);
            this.locationManager = (LocationManager)GetSystemService(LocationService);


            var criteria = new Criteria
                               {
                                   Accuracy = Accuracy.Coarse,
                                   PowerRequirement = Power.Low,
                               };
            string bestProvider = this.locationManager.GetBestProvider(criteria, true);

            Location lastKnownLocation = this.locationManager.GetLastKnownLocation(bestProvider);

            if (lastKnownLocation != null)
            {
                this.locationText.Text = string.Format("Last known location, lat: {0}, long: {1}", lastKnownLocation.Latitude, lastKnownLocation.Longitude);
            }

            this.locationManager.RequestLocationUpdates(bestProvider, 1000, 30, this);
        }

        public void OnLocationChanged(Location location)
        {
            this.stringBuilder.AppendLine(string.Format("Location updated, lat: {0}, long: {1}", location.Latitude, location.Longitude)
            );

            this.currentLocation = new TrackLocation
            {
                Longitude = location.Longitude,
                Latitude = location.Latitude
            };

            try
            {
                Address address = this.geocoder.GetFromLocation(location.Latitude, location.Longitude, 1).FirstOrDefault();

                if (address != null)
                {
                    this.stringBuilder.AppendLine(string.Format("Country: {0}-{1}", address.CountryCode, address.CountryName));
                    this.stringBuilder.AppendLine(string.Format("City: {0}-{1}", address.PostalCode, address.Locality));
                    this.stringBuilder.AppendLine(string.Format("Street: {0}-{1}", address.Thoroughfare, address.FeatureName));

                    this.currentLocation.City = address.Locality;
                    this.currentLocation.PostalCode = address.PostalCode;
                    this.currentLocation.Country = address.CountryName;
                    this.currentLocation.HouseNumber = address.FeatureName;
                    this.currentLocation.Street = address.Thoroughfare;
                }

                TrackLocationManager.Instance.SaveCurrentLocation(this.currentLocation);
            }
            catch (IOException io)
            {
                Android.Util.Log.Debug("LocationActivity", io.Message);
            }

            this.locationText.Text = this.stringBuilder.ToString();
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

