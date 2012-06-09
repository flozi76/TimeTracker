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

namespace TimeTracker
{
    using Android.Locations;
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Geo;
    using TimeTracker.Core.Ioc;
    using TimeTracker.ViewModel;

    public class CentralStation
    {
        private static CentralStation centralStation;
        private Ainject ainject;
        private Context applicationContext;
        private bool catalogInitialized;

        private CentralStation()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static CentralStation Instance
        {
            get { return centralStation ?? (centralStation = new CentralStation()); }
        }

        public Ainject Ainject
        {
            get
            {
                if (this.ainject == null)
                {
                    this.ainject = new Ainject();
                    this.InitializeAinjectCatalog();
                }
                return this.ainject;
            }
            set { this.ainject = value; }
        }

        /// <summary>
        /// Initializes the ainject catalog.
        /// </summary>
        private void InitializeAinjectCatalog()
        {
            if (this.catalogInitialized == false)
            {
                var locationManager = (LocationManager)this.applicationContext.GetSystemService(Context.LocationService);
                ICoreApplicationContext coreApplicationContext = new CoreCoreApplicationContext();
                ILocationListener currentLocationListener = new CurrentLocationListener(coreApplicationContext, locationManager, this.applicationContext);

                this.ainject.RegisterType(() => coreApplicationContext);
                this.ainject.RegisterType(() => currentLocationListener);
                this.ainject.RegisterType<IDistanceCalculator>(() => new DistanceCalculator());

                // Register ViewModels
                this.ainject.RegisterType<ISelectLocationViewModel>(() => new SelectLocationViewModel(this.ainject.ResolveType<ICoreApplicationContext>()));

                this.catalogInitialized = true;
            }
        }

        public void InitializeContext(Context applicationContext)
        {
            this.applicationContext = applicationContext;
        }
    }
}