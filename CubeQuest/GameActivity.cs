using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CubeQuest.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using AlertDialog = Android.App.AlertDialog;

namespace CubeQuest
{
    [Activity(Label = "GameActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class GameActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private Location userLocation;

        private GoogleMap googleMap;

        private LocationManager locationManager;

        private List<Marker> markers;

        private View profileView;

        private const int RcAchievementUi = 9003;

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

                    Log.Info("POSITION", $"Long={location.Longitude} Lat={location.Latitude} Speed={location.Speed}");
                    
                    markers.First().Position = LocationManager.ToLatLng(location);
                    googleMap.AnimateCamera(CameraUpdateFactory.NewLatLng(LocationManager.ToLatLng(location)));
                };

            // Show profile when clicking on button
            FindViewById<FloatingActionButton>(Resource.Id.fabUser).Click  += (sender, args) => ToggleProfile(true);
            FindViewById<FloatingActionButton>(Resource.Id.fabGame).Click  += (sender, args) => ToggleProfile(false);

            // Set up profile view
            profileView = FindViewById<ViewStub>(Resource.Id.stub_profile).Inflate();

            // Hide it in code to be able to see it when designing in xml
            profileView.Visibility = ViewStates.Invisible;

            // Set values on profile
            profileView.FindViewById<TextView>(Resource.Id.textProfileName).Text = AccountManager.Name;

            // Set button actions
            profileView.FindViewById<ImageButton>(Resource.Id.button_achievements).Click += (sender, args) => 
                StartActivityForResult(AccountManager.AchievementsIntent, RcAchievementUi);
        }
        
        public void OnMapReady(GoogleMap map)
        {
            // Set local maps
            googleMap = map;

            // Disable scrolling
            googleMap.UiSettings.ScrollGesturesEnabled = false;
            googleMap.UiSettings.ZoomGesturesEnabled = false;

            // Set custom theme to map
            //googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.map_theme_dark));
            
            // Sample icons
            var spookyNoodleIcon = BitmapDescriptorFactory.FromAsset("enemy/snake2.png");
            
            // Get last known location or 0,0 if not known
            // TODO: If not known, show loading dialog
            var location = userLocation == null ? new LatLng(0, 0) : LocationManager.ToLatLng(userLocation);
            
            // Test position for test enemy
            var testPosition = new LatLng(location.Latitude + 0.005, location.Longitude + 0.005);

            // Create player marker
            AddMarker(location, AccountManager.Name, BitmapDescriptorFactory.FromAsset("player/0.webp"));
            markers.First().ZIndex = 10f;

            // Create test marker
            AddMarker(testPosition, "Spooky Noodle", spookyNoodleIcon);

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
        private void AddMarker(LatLng latLng, string title, BitmapDescriptor icon) => 
            markers.Add(googleMap.AddMarker(new MarkerOptions()
                .SetPosition(latLng)
                .SetTitle(title)
                .Anchor(0.5f, 0.5f)
                .SetIcon(icon)));

        protected override void OnStart()
        {
            base.OnStart();

            // Set view for game popups
            AccountManager.SetViewForPopups(FindViewById(Android.Resource.Id.Content));

            // Check if GPS is enabled
            if (!locationManager?.IsLocationServicesEnabled ?? true)
            {
                // TODO: Show fullscreen until user enabled location
                new AlertDialog.Builder(this)
                    .SetTitle("Location Required")
                    .SetMessage("GPS is required to get your location, but it's disabled")
                    .SetPositiveButton("Enable It", (sender, args) => StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings)))
                    .Show();
            }

            // Request permissions on start
            // TODO: The app will try to access the position while asking for permissions, making it crash
            if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                RequestPermissions(new[]
                {
                    Manifest.Permission.AccessFineLocation
                }, 0);
        }

        private void ToggleProfile(bool enabled)
        {
            // FABs
            var fabUser = FindViewById<FloatingActionButton>(Resource.Id.fabUser);
            var fabMap  = FindViewById<FloatingActionButton>(Resource.Id.fabGame);

            // Show or hide buttons
            if (enabled)
            {
                // Show map, hide user
                fabUser.Hide();
                fabMap.Show();
            }
            else
            {
                // Hide map, show user
                fabMap.Hide();
                fabUser.Show();
            }

            // Starting/ending point
            var centerX = fabUser.Left + fabUser.Width / 2;
            var centerY = fabUser.Top  + fabUser.Height / 2;

            // Button radius
            var radius = (float) Math.Sqrt(centerX * centerX + centerY * centerY);

            var animator = ViewAnimationUtils.CreateCircularReveal(profileView, centerX, centerY, enabled ? 0f : radius, enabled ? radius : 0f);

            // Hide or show view
            if (enabled)
                profileView.Visibility = ViewStates.Visible;
            else
                animator.AnimationEnd += (o, eventArgs) => profileView.Visibility = ViewStates.Invisible;

            animator.Start();
        }

        public override void OnBackPressed()
        {
            // If on profile, go back to map, otherwise, ignore
            if (profileView.Visibility == ViewStates.Visible)
                ToggleProfile(false);
        }
    }
}