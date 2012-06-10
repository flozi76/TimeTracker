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

                this.ainject.RegisterType<IDistanceCalculator>(() => new DistanceCalculator());
                this.ainject.RegisterType<ITimeTrackerWorkspace>(() => new TimeTrackerWorkspace(this.ainject.ResolveType<IDistanceCalculator>()));
                ICoreApplicationContext coreApplicationContext = new CoreCoreApplicationContext(this.ainject.ResolveType<ICoordinateGeocoder>(), this.ainject.ResolveType<ITimeTrackerWorkspace>());

                this.ainject.RegisterType(() => coreApplicationContext);
                this.ainject.RegisterType<IPerimeterWatchDog>(() => new PerimeterWatchDog(coreApplicationContext, this.ainject.ResolveType<IDistanceCalculator>(), this.ainject.ResolveType<ITimeTrackerWorkspace>()));

                // Register ViewModels
                this.ainject.RegisterType<ISelectLocationViewModel>(() => new SelectLocationViewModel(this.ainject.ResolveType<ICoreApplicationContext>()));

                this.catalogInitialized = true;
            }
        }
    }
}