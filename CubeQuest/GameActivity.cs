using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using CubeQuest.Account;

namespace CubeQuest
{
    [Activity(Label = "GameActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class GameActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private Location userLocation;

        private GoogleMap googleMap;

        private LocationManager locationManager;

        private List<Marker> markers;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);

            markers = new List<Marker>();

            // Get last known location
            locationManager = new LocationManager(this);
            userLocation    = await locationManager.GetLastKnownLocation();

            // Get map and listen when it's ready
            var mapFragment = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
            
            locationManager.OnLocationUpdate += location =>
                {
                    // TODO: Check speed etc. here
                    // TODO: Show a loading dialog until we get the first position

                    Log.Info("POSITION", location.ToString());

                    markers.First().Position = LocationManager.ToLatLng(location);
                    googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(LocationManager.ToLatLng(location)));
                };
        }

        public void OnMapReady(GoogleMap map)
        {
            // Set local maps
            googleMap = map;

            // Disable scrolling
            googleMap.UiSettings.ScrollGesturesEnabled = false;

            // Sample icons
            var icon = BitmapDescriptorFactory.FromAsset("enemy/snake.png");
            var spookyNoodleIcon = BitmapDescriptorFactory.FromAsset("enemy/snake2.png");
            
            // Get last known location or 0,0 if not known
            // TODO: If not known, show loading dialog
            var location = userLocation == null ? new LatLng(0, 0) : LocationManager.ToLatLng(userLocation);
            
            // Test position for test enemy
            var testPosition = new LatLng(location.Latitude + 0.005, location.Longitude + 0.005);

            // Create player marker
            SetUpMarker(location, AccountManager.Name, icon);

            // Create test marker
            SetUpMarker(testPosition, "Spooky Noodle", spookyNoodleIcon);

            // Target player with initial zoom
            var position = CameraPosition.InvokeBuilder()
                .Target(location)
                .Zoom(32f)
                .Build();

            // Move camera to player
            googleMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(position));
        }
        
        /// <summary>
        /// Creates a marker at the specified position
        /// </summary>
        private void SetUpMarker(LatLng latLng, string title, BitmapDescriptor icon) => 
            markers.Add(googleMap.AddMarker(new MarkerOptions()
                .SetPosition(latLng)
                .SetTitle(title)
                .SetIcon(icon)));

        protected override void OnStart()
        {
            base.OnStart();

            // Request permissions on start
            // TODO: The app will try to access the position while asking for permissions, making it crash
            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                RequestPermissions(new[]
                {
                    Manifest.Permission.AccessFineLocation
                }, 0);
        }
    }
}