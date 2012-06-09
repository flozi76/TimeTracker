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

namespace TimeTracker.Core.Domain
{
    using TimeTracker.Core.BusinessLayer;
    using TimeTracker.Core.Geo;

    public class CoreCoreApplicationContext : ICoreApplicationContext
    {
        public Coordinate CurrentLocation { get; set; }
        public TrackLocation GetCurrentTrackLocation()
        {
            if (this.CurrentLocation != null)
            {
                var trackLocation = new TrackLocation
                                        {
                                            Name =
                                                string.Format("{0} - {1}", this.CurrentLocation.Latitude,
                                                              this.CurrentLocation.Longitude)
                                        };

                return trackLocation;
            }

            return null;
        }
    }
}