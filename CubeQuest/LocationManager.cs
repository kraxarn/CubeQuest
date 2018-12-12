using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Threading.Tasks;

namespace CubeQuest
{
    public class LocationManager
    {
        public delegate void LocationUpdateEvent(Location location);

        /// <summary>
        /// Location was updated
        /// </summary>
        public event LocationUpdateEvent OnLocationUpdate;

        /// <summary>
        /// Universal location provider
        /// </summary>
        private readonly FusedLocationProviderClient client;

        private readonly Android.Locations.LocationManager locationManager;

        public LocationManager(Context context)
        {
            // Create client from context
            client = LocationServices.GetFusedLocationProviderClient(context);

            // Create Android location manager
            locationManager = context.GetSystemService(Context.LocationService) as Android.Locations.LocationManager;

            // Create location request and set some options
            var locationRequest = new LocationRequest();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(1000);

            // Receives all location updates
            var locationCallback = new LocationCallback();
            locationCallback.LocationResult += (sender, args) =>
            {
                if (args.Result == null)
                    return;

                foreach (var location in args.Result.Locations)
                    OnLocationUpdate?.Invoke(location);
            };

            // Start receiving location updates
            client.RequestLocationUpdatesAsync(locationRequest, locationCallback);
        }

        /// <summary>
        /// Gets the user's last known location
        /// (very likely to return null at first)
        /// </summary>
        public async Task<Location> GetLastKnownLocation() => 
            await client.GetLastLocationAsync();

        /// <summary>
        /// Converts a <see cref="Location"/> to a <see cref="LatLng"/>
        /// </summary>
        public static LatLng ToLatLng(Location location) => 
            new LatLng(location.Latitude, location.Longitude);

        public bool IsLocationServicesEnabled => 
            locationManager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider);
    }
}