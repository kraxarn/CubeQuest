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
            googleMap = map;

            googleMap.UiSettings.ScrollGesturesEnabled = false;

            var icon = BitmapDescriptorFactory.FromAsset("enemy/snake.png");
            var spookyNoodleIcon = BitmapDescriptorFactory.FromAsset("enemy/snake2.png");
            
            var location = userLocation == null ? new LatLng(0, 0) : LocationManager.ToLatLng(userLocation);
            
            var testPosition = new LatLng(location.Latitude + 0.005, location.Longitude + 0.005);

            SetUpMarker(location, AccountManager.Name, icon);

            SetUpMarker(testPosition, "Spooky Noodle", spookyNoodleIcon);

            var position = CameraPosition.InvokeBuilder()
                .Target(location)
                .Zoom(32f)
                .Build();

            googleMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(position));
        }
        
        private void SetUpMarker(LatLng latLng, string title, BitmapDescriptor icon) => 
            markers.Add(googleMap.AddMarker(new MarkerOptions()
                .SetPosition(latLng)
                .SetTitle(title)
                .SetIcon(icon)));

        protected override void OnStart()
        {
            base.OnStart();

            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                RequestPermissions(new[]
                {
                    Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation
                }, 0);
        }
    }
}