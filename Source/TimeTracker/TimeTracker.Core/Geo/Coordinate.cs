namespace TimeTracker.Core.Geo
{
    using System;

    public class Coordinate
    {
        private double latitude, longitude;

        /// <summary>
        /// Latitude in degrees. -90 to 90
        /// </summary>
        public Double Latitude
        {
            get { return this.latitude; }
            set
            {
                if (value > 90) throw new ArgumentOutOfRangeException("value", "Latitude value cannot be greater than 90.");
                if (value < -90) throw new ArgumentOutOfRangeException("value", "Latitude value cannot be less than -90.");
                this.latitude = value;
            }
        }

        /// <summary>
        /// Longitude in degree. -180 to 180
        /// </summary>
        public Double Longitude
        {
            get { return this.longitude; }
            set
            {
                if (value > 180) throw new ArgumentOutOfRangeException("value", "Longitude value cannot be greater than 180.");
                if (value < -180) throw new ArgumentOutOfRangeException("value", "Longitude value cannot be less than -180.");
                this.longitude = value;
            }
        }
    }
}