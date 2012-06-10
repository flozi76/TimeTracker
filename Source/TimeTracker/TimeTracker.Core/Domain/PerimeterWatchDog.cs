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
        private readonly ITrackLocationManager trackLocationManager;

        public PerimeterWatchDog(ICoreApplicationContext coreApplicationContext, IDistanceCalculator distanceCalculator, ITrackLocationManager trackLocationManager)
        {
            this.coreApplicationContext = coreApplicationContext;
            this.distanceCalculator = distanceCalculator;
            this.trackLocationManager = trackLocationManager;
        }

        public void CheckPerimeter()
        {
            if (this.coreApplicationContext.LastTrackLocation != null && this.coreApplicationContext.CurrentLocation != null)
            {
                var distance = distanceCalculator.Distance(this.coreApplicationContext.CurrentLocation, this.coreApplicationContext.LastTrackLocation.ToCoordinate(), UnitsOfLength.Meter);

            }
        }

        /// <summary>
        /// Finds the track location for coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns></returns>
        private TrackLocation FindTrackLocationForCoordinate(Coordinate coordinate)
        {
            return null;
        }
    }
}