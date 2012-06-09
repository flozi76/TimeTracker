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

namespace TimeTracker.Core.Ioc
{
    using Android.Locations;
    using TimeTracker.Core.Geo;

    public class Ainject : IAinject
    {
        private readonly Dictionary<Type, object> objectCatalog;

        public Ainject()
        {
            this.objectCatalog = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The func.</param>
        public void RegisterType<T>(Func<T> func) where T : class
        {
            this.objectCatalog.Add(typeof(T), func);
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>the object of type t</returns>
        public T ResolveType<T>()
        {
            if (this.objectCatalog.ContainsKey(typeof(T)))
            {
                var function = (Func<T>)this.objectCatalog[typeof(T)];
                return function.Invoke();
            }

            throw new KeyNotFoundException(string.Format("Key not registered in Ainject catalog {0}", typeof(T).FullName));
        }
    }
}