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

        public event LocationUpdateEvent OnLocationUpdate;

        private readonly FusedLocationProviderClient client;

        public LocationManager(Context context)
        {
            client = LocationServices.GetFusedLocationProviderClient(context);

            var locationRequest = new LocationRequest();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(1000);

            var locationCallback = new LocationCallback();

            // Function that receives all the location updates
            locationCallback.LocationResult += (sender, args) =>
            {
                if (args.Result == null)
                    return;

                foreach (var location in args.Result.Locations)
                    OnLocationUpdate?.Invoke(location);
            };

            client.RequestLocationUpdatesAsync(locationRequest, locationCallback);
        }

        /// <summary>
        /// Gets the user's last known location
        /// </summary>
        public async Task<Location> GetLastKnownLocation() => 
            await client.GetLastLocationAsync();

        /// <summary>
        /// Converts a <see cref="Location"/> to a <see cref="LatLng"/>
        /// </summary>
        public static LatLng ToLatLng(Location location) => 
            new LatLng(location.Latitude, location.Longitude);


    }
}