namespace TimeTracker.Core.Database.SQLite
{
    using System;

    public class MaxLengthAttribute : Attribute
    {
        public int Value { get; private set; }

        public MaxLengthAttribute(int length)
        {
            this.Value = length;
        }
    }
}