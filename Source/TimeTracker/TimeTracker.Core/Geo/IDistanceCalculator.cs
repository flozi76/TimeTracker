namespace TimeTracker.Core.Geo
{
    using System;

    public interface IDistanceCalculator
    {
        /// <summary>
        /// Calculates the distance between two points of latitude and longitude.
        /// Great Link - http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="coordinate1">First coordinate.</param>
        /// <param name="coordinate2">Second coordinate.</param>
        /// <param name="unitsOfLength">Sets the return value unit of length.</param>
        Double Distance(Coordinate coordinate1, Coordinate coordinate2, UnitsOfLength unitsOfLength);

        /// <summary>
        /// Accepts two coordinates in degrees.
        /// </summary>
        /// <returns>A double value in degrees.  From 0 to 360.</returns>
        Double Bearing(Coordinate coordinate1, Coordinate coordinate2);
    }
}