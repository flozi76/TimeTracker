

namespace TimeTracker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Android.App;
    using Android.Widget;
    using TimeTracker.Adapters;
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.ViewModel;

    [Activity(Label = "Select Location")]
    public class SelectLocationActivity : Activity
    {
        private ISelectLocationViewModel viewModel;
        private ListView listView;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SelectLocation);
            this.listView = this.FindViewById<ListView>(Resource.Id.listViewSelectLocations);

            try
            {
                this.viewModel = CentralStation.Instance.Ainject.ResolveType<ISelectLocationViewModel>();
                IList<TrackLocation> currentLocations = this.viewModel.ResolveCurrentLocations();
                this.listView.Adapter = new TrackLocationListAdapter(this, currentLocations);
                this.listView.TextFilterEnabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }
    }
}