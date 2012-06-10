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
    using TimeTracker.Adapters;
    using TimeTracker.Core.Domain;

    [Activity(Label = "My Activity")]
    public class LogEntriesListActivity : Activity
    {
        private ListView listViewLogEntries;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LogEntriesList);

            this.listViewLogEntries = this.FindViewById<ListView>(Resource.Id.listViewLogEntries);

            try
            {
                var trackLocations = CentralStation.Instance.Ainject.ResolveType<ITimeTrackerWorkspace>().GetTrackLocationLogEntries().ToList();
                this.listViewLogEntries.Adapter = new TrackLocationLogEntryListAdapter(this, trackLocations);
                this.listViewLogEntries.TextFilterEnabled = true;

                this.listViewLogEntries.ItemClick += (sender, e) =>
                {
                    var backToMain = new Intent(this, typeof(MainActivity));
                    this.StartActivity(backToMain);
                };
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().Name, ex.StackTrace);
            }
        }
    }
}