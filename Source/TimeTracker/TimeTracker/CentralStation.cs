namespace TimeTracker
{
    using TimeTracker.Core.Domain;
    using TimeTracker.Core.Geo;
    using TimeTracker.Core.Ioc;
    using TimeTracker.ViewModel;

    public class CentralStation
    {
        private static CentralStation centralStation;
        private Ainject ainject;
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
                this.ainject.RegisterType<ICoordinateGeocoder>(() => new CoordinateGeocoder());

                ICoreApplicationContext coreApplicationContext = new CoreCoreApplicationContext(this.ainject.ResolveType<ICoordinateGeocoder>());

                this.ainject.RegisterType(() => coreApplicationContext);
                this.ainject.RegisterType<IDistanceCalculator>(() => new DistanceCalculator());
                this.ainject.RegisterType<ITrackLocationManager>(() => new TrackLocationManager());

                // Register ViewModels
                this.ainject.RegisterType<ISelectLocationViewModel>(() => new SelectLocationViewModel(this.ainject.ResolveType<ICoreApplicationContext>()));

                this.catalogInitialized = true;
            }
        }
    }
}