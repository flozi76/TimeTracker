namespace TimeTracker.Core.Database
{
    using System;

    public class CollationAttribute : Attribute
    {
        public string Value { get; private set; }

        public CollationAttribute(string collation)
        {
            this.Value = collation;
        }
    }
}