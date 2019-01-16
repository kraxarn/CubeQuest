using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps.Model;
using Android.Locations;
using System;
using System.Threading.Tasks;

namespace CubeQuest.Handler
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

        private readonly LocationRequest locationRequest;

        private readonly LocationCallback locationCallback;

        public LocationManager(Context context)
        {
            // Create client from context
            client = LocationServices.GetFusedLocationProviderClient(context);

            // Create Android location manager
            locationManager = context.GetSystemService(Context.LocationService) as Android.Locations.LocationManager;

            // Create location request and set some options
            locationRequest = new LocationRequest();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(1000);

            // Receives all location updates
            locationCallback = new LocationCallback();
            locationCallback.LocationResult += (sender, args) =>
            {
                if (args.Result == null)
                    return;

                foreach (var location in args.Result.Locations)
                    OnLocationUpdate?.Invoke(location);
            };

            // Start receiving location updates
			Start();
        }

        public async void Start() => 
	        await client.RequestLocationUpdatesAsync(locationRequest, locationCallback);

        public async void Stop() => 
	        await client.RemoveLocationUpdatesAsync(locationCallback);

        /// <summary>
		/// Accuracy for location (100/highest - 105/lowest)
		/// </summary>
        public int LocationPriority
        {
	        get => locationRequest.Priority;
	        set => locationRequest.SetPriority(value);
        }

        /// <summary>
        /// Gets the user's last known location
        /// (very likely to return null at first)
        /// </summary>
        public async Task<Location> GetLastKnownLocationAsync() => 
            await client.GetLastLocationAsync();

        public bool IsLocationServicesEnabled => 
            locationManager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider);

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
        
        /// <summary>
        /// Get the distance between two points in meter
        /// </summary>
        /// <returns>Distance in meters</returns>
        public static double GetDistance(LatLng c1, LatLng c2)
        {
            /*
			 * This uses the 'haversine' formula to calculate the distance between two points.
			 * More info: http://www.movable-type.co.uk/scripts/latlong.html
			 * I have no clue how it works tbh, but it does, I guess
			 */

            var distLat = DegreesToRadians(c2.Latitude - c1.Latitude);
            var distLng = DegreesToRadians(c2.Longitude - c1.Longitude);

            var lat1 = DegreesToRadians(c1.Latitude);
            var lat2 = DegreesToRadians(c2.Latitude);

            // This does things, no clue what tbh
            var a = Math.Sin(distLat / 2) * Math.Sin(distLat / 2) + Math.Sin(distLng / 2) * Math.Sin(distLng / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Earth's radius is 6371 km, unless it's flat
            return 6371e3 * c;
        }
    }

    public static class LocationManagerExtensions
    {
        /// <summary>
        /// Converts a <see cref="Location"/> to a <see cref="LatLng"/>
        /// </summary>
        public static LatLng ToLatLng(this Location location) =>
            new LatLng(location.Latitude, location.Longitude);
    }
}