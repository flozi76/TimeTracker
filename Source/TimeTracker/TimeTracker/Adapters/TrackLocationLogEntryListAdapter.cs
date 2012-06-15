using System.Collections.Generic;
using Android.App;
using Android.Widget;

namespace TimeTracker.Adapters
{
    using TimeTracker.Core.Domain.Entities;

    public class TrackLocationLogEntryListAdapter : BaseAdapter<TrackLocationLogEntry>
    {
        private readonly Activity context;
        private readonly IList<TrackLocationLogEntry> trackLocations = new List<TrackLocationLogEntry>();

        public TrackLocationLogEntryListAdapter(Activity context, IList<TrackLocationLogEntry> trackLocations)
        {
            this.context = context;
            this.trackLocations = trackLocations;
        }

        public override TrackLocationLogEntry this[int position]
        {
            get { return this.trackLocations[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return this.trackLocations.Count; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {

            // Get our object for this position
            var item = this.trackLocations[position];

            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // This gives us some performance gains by not always inflating a new view
            // This will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
            var view = (convertView ??
                    this.context.LayoutInflater.Inflate(
                    Resource.Layout.LocationListItem,
                    parent,
                    false)) as LinearLayout;

            if (view != null)
            {
                // Find references to each subview in the list item's view
                var textItem = view.FindViewById<TextView>(Resource.Id.textViewLocationItem);
                //var textEntry = view.FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextLocationName);

                if (item != null)
                {
                    //Assign this item's values to the various subviews
                    textItem.SetText(string.Format("{0} - {1}", item.LogEntry, item.LogDateTime), TextView.BufferType.Normal);
                    //textEntry.SetText(item.LocationName, TextView.BufferType.Normal);
                }
            }

            //Finally return the view
            return view;
        }
    }
}