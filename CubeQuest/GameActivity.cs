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
using CubeQuest.Account.Enemies;
using System;
using Android.Content.Res;
using Android.Graphics;
using AlertDialog = Android.App.AlertDialog;

namespace CubeQuest
{
    [Activity(Label = "GameActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class GameActivity : AppCompatActivity, IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
    {
        private Location userLocation;
        
        private GoogleMap googleMap;

        private LocationManager locationManager;

        /// <summary>
        /// Marker used to represent the player
        /// </summary>
        private Marker playerMarker;

        private View profileView;

        private View battleView;

        private View mainView;

        private const int RcAchievementUi = 9003;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);

            // Get main view
            mainView = FindViewById<CoordinatorLayout>(Resource.Id.layout_game);
            mainView.Visibility = ViewStates.Invisible;

			var health = AccountManager.CurrentUser.Health;

            this.FindViewById<ProgressBar>(Resource.Id.barHealth).Progress = health;

            // Get last known location
            locationManager = new LocationManager(this);
            userLocation    = await locationManager.GetLastKnownLocation();

            // Get map and listen when it's ready
            var mapFragment = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var textDebugLocation = FindViewById<TextView>(Resource.Id.text_debug_location);

            locationManager.OnLocationUpdate += location =>
                {
                    // TODO: Check speed etc. here
                    // TODO: Show a loading dialog until we get the first position

                    Log.Info("POSITION", $"Long={location.Longitude} Lat={location.Latitude} Speed={location.Speed}");

                    textDebugLocation.Text = $"Accuracy: {location.Accuracy}%\nSpeed:    {location.Speed} m/s";

                    // Update user location
                    userLocation = location;

                    // If map hasn't loaded yet, ignore player marker
                    if (playerMarker == null)
                        return;
                    
                    playerMarker.Position = location.ToLatLng();
                    googleMap.AnimateCamera(CameraUpdateFactory.NewLatLng(location.ToLatLng()));
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

            // Inflate battle view
            battleView = FindViewById<ViewStub>(Resource.Id.stub_battle).Inflate();
            battleView.Visibility = ViewStates.Invisible;

            // Setup debug mode
            FindViewById<Button>(Resource.Id.button_debug_enemy).Click += (sender, args) =>
                AddMarker(new LatLng(userLocation.Latitude + 0.0001, userLocation.Longitude + 0.0001), "Enemy",
                    BitmapDescriptorFactory.FromAsset("enemy/snake2.webp"));

            FindViewById<Button>(Resource.Id.button_debug_battle).Click += (sender, args) => StartBattle();
        }

        public override void OnEnterAnimationComplete()
        {
	        base.OnEnterAnimationComplete();
			StartEnterAnimation();
        }

        public void OnMapReady(GoogleMap map)
        {
            // Set local maps
            googleMap = map;

            // Disable scrolling
            googleMap.UiSettings.ScrollGesturesEnabled = false;
            googleMap.UiSettings.ZoomGesturesEnabled = false;

            // Set custom theme to map
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.map_theme_dark));
            
            // Get last known location or 0,0 if not known
            // TODO: If not known, show loading dialog
            var location = userLocation == null ? new LatLng(0, 0) : userLocation.ToLatLng();

            // Create player marker
            playerMarker = AddMarker(location, AccountManager.Name, BitmapDescriptorFactory.FromAsset("player/0.webp"));
            playerMarker.ZIndex = 10f;
            playerMarker.Tag = "player";

            // Target player with initial zoom
            var position = CameraPosition.InvokeBuilder()
                .Target(location)
                .Zoom(24f)
                .Build();

            // Move camera to player
            googleMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(position));

            googleMap.SetOnMarkerClickListener(this);
        }

        public bool OnMarkerClick(Marker marker)
        {
            // Ignore player
            if (marker.Tag?.ToString() == "player")
                return true;

            // TODO: Temporary message
            var distance = (int) (LocationManager.GetDistance(userLocation.ToLatLng(), marker.Position) + 0.5);
            
            // TODO: Sometimes it works and sometimes not?
            var text = $"Level 5 Danger Noodle ({distance} {(distance == 1 ? "meter" : "meters")} away)";

            Snackbar.Make(mainView, text, Snackbar.LengthLong)
                .SetActionTextColor(ColorStateList.ValueOf(Color.ParseColor("#e53935")))
                .SetAction("Fight", view => StartBattle())
                .Show();

            return true;
        }

        /// <summary>
        /// Creates a marker at the specified position
        /// </summary>
        private Marker AddMarker(LatLng latLng, string title, BitmapDescriptor icon) =>
            googleMap.AddMarker(new MarkerOptions()
                .SetPosition(latLng)
                .SetTitle(title)
                .Anchor(0.5f, 0.5f)
                .SetIcon(icon));

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

        private void StartEnterAnimation()
        {
	        var centerX = mainView.Width / 2;
	        var centerY = mainView.Height / 2;

	        var radius = (float) Math.Sqrt(centerX * centerX + centerY * centerY);

	        var animator = ViewAnimationUtils.CreateCircularReveal(mainView, centerX, centerY, 0f, radius);

	        mainView.Visibility = ViewStates.Visible;
	        animator.Start();
		}

        private void StartBattle()
        {
            var fabUser = FindViewById<FloatingActionButton>(Resource.Id.fabUser);
            fabUser.Hide();

            // Sets the health on the progressbar 
            mainView.FindViewById<ProgressBar>(Resource.Id.progress_battle_health).Progress =
                AccountManager.CurrentUser.Health;
;
            var battle = new Battle(this, battleView, Assets, new EnemySnake());

            var centerX = mainView.Width  / 2;
            var centerY = mainView.Height / 2;

            var radius = (float) Math.Sqrt(centerX * centerX + centerY * centerY);

            var animator = ViewAnimationUtils.CreateCircularReveal(battleView, centerX, centerY, 0f, radius);

            battle.End += () =>
            {
                var animator2 = ViewAnimationUtils.CreateCircularReveal(battleView, centerX, centerY, radius, 0f);
                animator2.AnimationEnd += (o, eventArgs) => battleView.Visibility = ViewStates.Invisible;
                animator2.Start();
                fabUser.Show();
            };

            battleView.Visibility = ViewStates.Visible;
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