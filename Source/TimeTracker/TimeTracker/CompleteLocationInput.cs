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
    using Android.Util;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Domain.Entities;

    [Activity(Label = "My Activity")]
    public class CompleteLocationInput : Activity
    {
        private TextView textViewLocationInfo;
        private TrackLocation location;
        private EditText editTextLocationName;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CompleteLocationInputForm);

            try
            {

                int locationId = Intent.GetIntExtra("LocationId", 0);
                var buttonSave = this.FindViewById<Button>(Resource.Id.buttonSaveLocation);
                this.textViewLocationInfo = this.FindViewById<TextView>(Resource.Id.textViewLocationInfo);
                this.editTextLocationName = this.FindViewById<EditText>(Resource.Id.editTextLocationName);

                this.location = CentralStation.Instance.Ainject.ResolveType<ITimeTrackerWorkspace>().GetTrackLocations().FirstOrDefault(t => t.ID == locationId);
                this.BindLocation(location);

                buttonSave.Click += delegate
                                        {
                                            var backToMain = new Intent(this, typeof(MainActivity));
                                            this.UpdateCurrentLocation();
                                            this.StartActivity(backToMain);
                                        };
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().Name, ex.StackTrace);
            }

        }

        private void UpdateCurrentLocation()
        {
            if (this.location != null)
            {
                this.location.Name = this.editTextLocationName.Text;
                CentralStation.Instance.Ainject.ResolveType<ITimeTrackerWorkspace>().SaveTrackLocation(location);

                var tmplocation = CentralStation.Instance.Ainject.ResolveType<ITimeTrackerWorkspace>().GetTrackLocations().FirstOrDefault(t => t.ID == location.ID);
            }
        }

        private void BindLocation(TrackLocation location)
        {
            if (location != null)
            {
                this.textViewLocationInfo.Text = location.ToString();
                this.editTextLocationName.Text = location.Name;
            }
        }
    }
}