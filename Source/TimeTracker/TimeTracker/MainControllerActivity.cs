
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Widget;
using Java.IO;

namespace TimeTracker
{
    using Android.Locations;

    [Activity(Label = "TimeTracker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainControllerActivity : Activity, ILocationListener
    {
        private TextView _locationText;
        private LocationManager _locationManager;
        private StringBuilder _builder;
        private Geocoder _geocoder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _builder = new StringBuilder();
            _geocoder = new Geocoder(this);
            _locationText = FindViewById<TextView>(Resource.Id.TextLocation);
            _locationManager = (LocationManager)GetSystemService(LocationService);

            var criteria = new Criteria
                               {
                                   Accuracy = Accuracy.Coarse,
                                   PowerRequirement = Power.Low,
                               };
            string bestProvider = _locationManager.GetBestProvider(criteria, true);

            Location lastKnownLocation = _locationManager.GetLastKnownLocation(bestProvider);

            if (lastKnownLocation != null)
            {
                _locationText.Text = string.Format("Last known location, lat: {0}, long: {1}",
                                                   lastKnownLocation.Latitude, lastKnownLocation.Longitude);
            }

            _locationManager.RequestLocationUpdates(bestProvider, 1000, 30, this);
        }

        public void OnLocationChanged(Location location)
        {
            _builder.AppendLine(string.Format("Location updated, lat: {0}, long: {1}", location.Latitude, location.Longitude)
            );

            try
            {
                Address address = _geocoder.GetFromLocation(location.Latitude, location.Longitude, 1).FirstOrDefault();

                if (address != null)
                {
                    _builder.AppendLine(string.Format("Country: {0}-{1}", address.CountryCode, address.CountryName));
                    _builder.AppendLine(string.Format("City: {0}-{1}", address.PostalCode, address.Locality));
                    _builder.AppendLine(string.Format("Street: {0}-{1}", address.Thoroughfare, address.FeatureName));

                }
            }
            catch (IOException io)
            {
                Android.Util.Log.Debug("LocationActivity", io.Message);
            }

            _locationText.Text = _builder.ToString();
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

