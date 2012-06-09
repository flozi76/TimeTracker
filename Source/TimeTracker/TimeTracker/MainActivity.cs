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
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Geo;

    [Activity(Label = "TimeTracker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button addCurrentLocationButton;
        private TextView listLocationsText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            this.listLocationsText = FindViewById<TextView>(Resource.Id.ListLocations);
            this.addCurrentLocationButton = FindViewById<Button>(Resource.Id.btnTrackcCurrentLocation);

            var trackLocations = CentralStation.Instance.Ainject.ResolveType<ITrackLocationManager>().GetTrackLocations();
            var stringBuilderList = new StringBuilder();
            foreach (var trackLocation in trackLocations)
            {
                stringBuilderList.AppendLine(string.Format("{0} {1} {2} {3} {4}", trackLocation.ID, trackLocation.PostalCode, trackLocation.City, trackLocation.Street, trackLocation.HouseNumber));
                stringBuilderList.AppendLine(string.Format("Lat: {0} Lon: {1}", trackLocation.Latitude, trackLocation.Longitude));
            }

            this.listLocationsText.Text = stringBuilderList.ToString();

            this.addCurrentLocationButton.Click += delegate
                                                       {
                                                           var intent = new Intent(this, typeof(SelectLocationActivity));
                                                           StartActivity(intent);
                                                       };

            //this.stringBuilder = new StringBuilder();
            //var geocoder = new Geocoder(this);
            //this.locationManager = (LocationManager)GetSystemService(LocationService);


            //var criteria = new Criteria
            //                   {
            //                       Accuracy = Accuracy.Coarse,
            //                       PowerRequirement = Power.Low,
            //                   };
            //string bestProvider = this.locationManager.GetBestProvider(criteria, true);

            //Location lastKnownLocation = this.locationManager.GetLastKnownLocation(bestProvider);

            //if (lastKnownLocation != null)
            //{
            //    this.currentLocation = new TrackLocation { Latitude = lastKnownLocation.Latitude, Longitude = lastKnownLocation.Longitude };
            //    this.stringBuilder.AppendLine(string.Format("Last known location, lat: {0}, long: {1}", lastKnownLocation.Latitude, lastKnownLocation.Longitude));
            //    this.UpdateAddress();
            //    this.locationText.Text = this.stringBuilder.ToString();
            //}

            //this.locationManager.RequestLocationUpdates(bestProvider, 1000, 1, this);
        }

        //public void OnLocationChanged(Location location)
        //{
        //    this.stringBuilder.AppendLine(string.Format("Location updated, lat: {0}, long: {1}", location.Latitude, location.Longitude));

        //    if (this.currentLocation != null)
        //    {
        //        var newCoordinate = new Coordinate { Latitude = location.Latitude, Longitude = location.Longitude };
        //        var lastCoordinate = new Coordinate { Latitude = this.currentLocation.Latitude, Longitude = this.currentLocation.Longitude };
        //        var distance = this.distanceCalculator.Distance(lastCoordinate, newCoordinate, UnitsOfLength.Meter);
        //        this.stringBuilder.AppendLine(string.Format("Distance to last coordinate: {0}", distance));
        //    }

        //    this.currentLocation = new TrackLocation
        //    {
        //        Longitude = location.Longitude,
        //        Latitude = location.Latitude
        //    };

        //    this.UpdateAddress();

        //    this.locationText.Text = this.stringBuilder.ToString();
        //}

        //private void UpdateAddress()
        //{
        //    try
        //    {
        //        Address address = this.geocoder.GetFromLocation(this.currentLocation.Latitude, this.currentLocation.Longitude, 1).FirstOrDefault();

        //        if (address != null)
        //        {
        //            this.stringBuilder.AppendLine(string.Format("Country: {0}-{1}", address.CountryCode, address.CountryName));
        //            this.stringBuilder.AppendLine(string.Format("City: {0}-{1}", address.PostalCode, address.Locality));
        //            this.stringBuilder.AppendLine(string.Format("Street: {0}-{1}", address.Thoroughfare, address.FeatureName));

        //            this.currentLocation.City = address.Locality;
        //            this.currentLocation.PostalCode = address.PostalCode;
        //            this.currentLocation.Country = address.CountryName;
        //            this.currentLocation.HouseNumber = address.FeatureName;
        //            this.currentLocation.Street = address.Thoroughfare;
        //        }
        //    }
        //    catch (IOException io)
        //    {
        //        Android.Util.Log.Debug("LocationActivity", io.Message);
        //    }
        //}

        //public void OnProviderDisabled(string provider)
        //{
        //}

        //public void OnProviderEnabled(string provider)
        //{
        //}

        //public void OnStatusChanged(string provider, Availability status, Bundle extras)
        //{
        //}
    }
}

