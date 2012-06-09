using System;

namespace TimeTracker.Core.Geo
{
    public enum UnitsOfLength { Mile, NauticalMiles, Kilometer, Meter }
    public enum CardinalPoints { N, E, W, S, Ne, Nw, Se, Sw }


    public class DistanceCalculator : IDistanceCalculator
    {
        private const Double MilesToKilometers = 1.609344;
        private const Double MilesToNautical = 0.8684;

        /// <summary>
        /// Calculates the distance between two points of latitude and longitude.
        /// Great Link - http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="coordinate1">First coordinate.</param>
        /// <param name="coordinate2">Second coordinate.</param>
        /// <param name="unitsOfLength">Sets the return value unit of length.</param>
        public Double Distance(Coordinate coordinate1, Coordinate coordinate2, UnitsOfLength unitsOfLength)
        {

            var theta = coordinate1.Longitude - coordinate2.Longitude;
            var distance = Math.Sin(coordinate1.Latitude.ToRadian()) * Math.Sin(coordinate2.Latitude.ToRadian()) +
                           Math.Cos(coordinate1.Latitude.ToRadian()) * Math.Cos(coordinate2.Latitude.ToRadian()) *
                           Math.Cos(theta.ToRadian());

            distance = Math.Acos(distance);
            distance = distance.ToDegree();
            distance = distance * 60 * 1.1515;

            if (unitsOfLength == UnitsOfLength.Kilometer)
                distance = distance * MilesToKilometers;
            else if (unitsOfLength == UnitsOfLength.NauticalMiles)
                distance = distance * MilesToNautical;
            else if (unitsOfLength == UnitsOfLength.Meter)
                distance = distance * MilesToKilometers * 1000;

            return (distance);

        }

        /// <summary>
        /// Accepts two coordinates in degrees.
        /// </summary>
        /// <returns>A double value in degrees.  From 0 to 360.</returns>
        public Double Bearing(Coordinate coordinate1, Coordinate coordinate2)
        {
            var latitude1 = coordinate1.Latitude.ToRadian();
            var latitude2 = coordinate2.Latitude.ToRadian();

            var longitudeDifference = (coordinate2.Longitude - coordinate1.Longitude).ToRadian();

            var y = Math.Sin(longitudeDifference) * Math.Cos(latitude2);
            var x = Math.Cos(latitude1) * Math.Sin(latitude2) -
                    Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longitudeDifference);

            return (Math.Atan2(y, x).ToDegree() + 360) % 360;
        }
    }
}