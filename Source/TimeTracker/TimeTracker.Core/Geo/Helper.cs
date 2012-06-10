using System;

namespace TimeTracker.Core.Geo
{
    using System.Collections.Generic;
    using Android.Locations;
    using TimeTracker.Core.Domain.Entities;

    public static class Helper
    {
        /// <summary>
        /// Converts degrees to Radians.
        /// </summary>
        /// <returns>Returns a radian from degrees.</returns>
        public static Double ToRadian(this Double degree) { return (degree * Math.PI / 180.0); }
        /// <summary>
        /// To degress from a radian value.
        /// </summary>
        /// <returns>Returns degrees from radians.</returns>
        public static Double ToDegree(this Double radian) { return (radian / Math.PI * 180.0); }

        // The directional names are also routinely and very conveniently associated with 
        // the degrees of rotation in the unit circle, a necessary step for navigational 
        // calculations (derived from trigonometry) and/or for use with Global 
        // Positioning Satellite (GPS) Receivers. The four cardinal directions 
        // correspond to the following degrees of a compass:
        //
        // North (N): 0° = 360° 
        // East (E): 90° 
        // South (S): 180° 
        // West (W): 270° 
        // An ordinal, or intercardinal, direction is one of the four intermediate 
        // compass directions located halfway between the cardinal directions.
        //
        // Northeast (NE), 45°, halfway between north and east, is the opposite of southwest. 
        // Southeast (SE), 135°, halfway between south and east, is the opposite of northwest. 
        // Southwest (SW), 225°, halfway between south and west, is the opposite of northeast. 
        // Northwest (NW), 315°, halfway between north and west, is the opposite of southeast. 
        // These 8 words have been further compounded, resulting in a total of 32 named 
        // (and numbered) points evenly spaced around the compass. It is noteworthy that 
        // there are languages which do not use compound words to name the points, 
        // instead assigning unique words, colors, and/or associations with phenomena of the natural world.

        /// <summary>
        /// Method extension for Doubles. Converts a degree to a cardinal point enumeration.
        /// </summary>
        /// <returns>Returns a cardinal point enumeration representing a compass direction.</returns>
        public static CardinalPoints ToCardinalMark(this Double degree)
        {

            var cardinalRanges = new List<CardinalRanges>
                       {
                         new CardinalRanges {CardinalPoint = CardinalPoints.N, LowRange = 0, HighRange = 22.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.Ne, LowRange = 22.5, HighRange = 67.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.E, LowRange = 67.5, HighRange = 112.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.Se, LowRange = 112.5, HighRange = 157.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.S, LowRange = 157.5, HighRange = 202.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.Sw, LowRange = 202.5, HighRange = 247.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.W, LowRange = 247.5, HighRange = 292.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.Nw, LowRange = 292.5, HighRange = 337.5},
                         new CardinalRanges {CardinalPoint = CardinalPoints.N, LowRange = 337.5, HighRange = 360.1}
                       };


            if (!(degree >= 0 && degree <= 360))
                throw new ArgumentOutOfRangeException("degree",
                                                      "Degree value must be greater than or equal to 0 and less than or equal to 360.");


            return cardinalRanges.Find(p => (degree >= p.LowRange && degree < p.HighRange)).CardinalPoint;
        }

        /// <summary>
        /// Toes the coordinate.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Coordinates for the location</returns>
        public static Coordinate ToCoordinate(this Location location)
        {
            return new Coordinate { Longitude = location.Longitude, Latitude = location.Latitude };
        }

        /// <summary>
        /// Toes the coordinate.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Coordinates for the location</returns>
        public static Coordinate ToCoordinate(this TrackLocation location)
        {
            return new Coordinate { Longitude = location.Longitude, Latitude = location.Latitude };
        }

        /// <summary>
        /// Class is used in a calculation to determin cardinal point enumeration values from degrees.
        /// </summary>
        private struct CardinalRanges
        {
            public CardinalPoints CardinalPoint;
            /// <summary>
            /// Low range value associated with the cardinal point.
            /// </summary>
            public Double LowRange;
            /// <summary>
            /// High range value associated with the cardinal point.
            /// </summary>
            public Double HighRange;
        }
    }
}