using System;

namespace TimeTracker.Core.Geo
{
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
    }
}