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
    using TimeTracker.Core.Domain.Entities;
    using TimeTracker.Core.Geo;

    public class PerimeterWatchDog : IPerimeterWatchDog
    {
        private readonly ICoreApplicationContext coreApplicationContext;
        private readonly IDistanceCalculator distanceCalculator;
        private readonly ITimeTrackerWorkspace timeTrackerWorkspace;

        public PerimeterWatchDog(ICoreApplicationContext coreApplicationContext, IDistanceCalculator distanceCalculator, ITimeTrackerWorkspace timeTrackerWorkspace)
        {
            this.coreApplicationContext = coreApplicationContext;
            this.distanceCalculator = distanceCalculator;
            this.timeTrackerWorkspace = timeTrackerWorkspace;
        }

        public void CheckPerimeter()
        {
            if (this.coreApplicationContext.CurrentLocation != null)
            {
                if (this.coreApplicationContext.CurrentTrackLocation != null)
                {
                    var distance = distanceCalculator.Distance(this.coreApplicationContext.CurrentLocation, this.coreApplicationContext.CurrentTrackLocation.ToCoordinate(), UnitsOfLength.Meter);
                    if (distance < Constants.PerimeterDistance)
                    {
                        // All OK still in perimeter of current TrackLocation
                        return;
                    }

                    this.coreApplicationContext.SetTrackLocation(null);
                }

                var location = this.FindTrackLocationForCoordinate(this.coreApplicationContext.CurrentLocation);
                this.coreApplicationContext.SetTrackLocation(location);
            }
        }

        /// <summary>
        /// Finds the track location for coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns></returns>
        private TrackLocation FindTrackLocationForCoordinate(Coordinate coordinate)
        {
            return this.timeTrackerWorkspace.GetTrackLocationForCoordinate(coordinate);
        }
    }
}