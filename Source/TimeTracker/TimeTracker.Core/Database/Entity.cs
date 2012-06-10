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

namespace TimeTracker.Core.Domain.Entities
{
    using TimeTracker.Core.Database.SQLite;

    public class Entity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public Entity()
        {
        }
    }
}