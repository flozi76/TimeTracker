namespace TimeTracker.Core.Database.SQLite
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