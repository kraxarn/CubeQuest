using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CubeQuest.Account;
using CubeQuest.Account.Enemies;
using CubeQuest.Account.Interface;
using CubeQuest.Battle;
using CubeQuest.Handler;
using CubeQuest.ListView.Item;
using CubeQuest.WorldGen;
using System;
using System.Collections.Generic;
using AlertDialog = Android.App.AlertDialog;

namespace CubeQuest.Layout
{
	[Activity(Label = "GameActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class GameActivity : AppCompatActivity, IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
    {
		/// <summary>
		/// Current location of the user
		/// </summary>
        private Location userLocation;
        
		/// <summary>
		/// The main map
		/// </summary>
        private GoogleMap googleMap;

		/// <summary>
		/// Manages and updates our location
		/// </summary>
        private Handler.LocationManager locationManager;

        /// <summary>
        /// Marker used to represent the player
        /// </summary>
        private Marker playerMarker;

		/// <summary>
		/// Main view for the profile
		/// </summary>
        private View profileView;

		/// <summary>
		/// Main view for the battle ui
		/// </summary>
        private View battleView;

		/// <summary>
		/// Main view with map
		/// </summary>
        private View mainView;

		/// <summary>
		/// All markers except the player
		/// </summary>
        private Dictionary<LatLng, Marker> markers;
        
        private ChunkHandler chunkHandler;

        private AlertDialog itemPopupDialog;

        private View itemPopupView;

        private RecyclerView popupRecycler;

        private BottomSheetBehavior battleInfo;

		/// <summary>
		/// First time starting the activity
		/// </summary>
        private bool firstTime;

        /// <summary>
        /// Value returned from the achievements intent
        /// </summary>
        private const int RcAchievementUi = 9003;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);

            firstTime = true;

			markers = new Dictionary<LatLng, Marker>();
            chunkHandler = new ChunkHandler();

            // Get main view
            mainView = FindViewById<CoordinatorLayout>(Resource.Id.layout_game);
            mainView.Visibility = ViewStates.Invisible;

			var health = AccountManager.CurrentUser.Health;

            FindViewById<ProgressBar>(Resource.Id.barHealth).Progress = health;

            // Get last known location
            locationManager = new Handler.LocationManager(this);
            userLocation    = await locationManager.GetLastKnownLocationAsync();

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

            profileView.FindViewById<ImageButton>(Resource.Id.button_settings).Click += (sender, args) =>
	            StartActivity(new Intent(this, typeof(SettingsActivity)));

            // Inflate battle view
            battleView = FindViewById<ViewStub>(Resource.Id.stub_battle).Inflate();
            battleView.Visibility = ViewStates.Invisible;

            // Setup debug mode
            FindViewById<Button>(Resource.Id.button_debug_enemy).Click += (sender, args) =>
                AddMarker(new LatLng(userLocation.Latitude + 0.0001, userLocation.Longitude + 0.0001), "Enemy",
                    BitmapDescriptorFactory.FromAsset("enemy/snake2.webp"));

            FindViewById<Button>(Resource.Id.button_debug_battle).Click += (sender, args) => StartBattle();

            var battleInfoView = FindViewById<LinearLayout>(Resource.Id.layout_battle_info);
			battleInfo         = BottomSheetBehavior.From(battleInfoView);
            battleInfo.State   = BottomSheetBehavior.StateHidden;

            battleInfoView.FindViewById<Button>(Resource.Id.button_battle_info_fight).Click +=
	            (sender, args) => StartBattle();

            FindViewById<Button>(Resource.Id.button_debug_battle_info).Click += (sender, args) =>
            {
	            battleInfoView.FindViewById<ImageView>(Resource.Id.image_battle_info).SetImageBitmap(BitmapFactory.DecodeStream(Assets.Open("enemy/snake.webp")));

	            battleInfo.State = BottomSheetBehavior.StateCollapsed;
            };

            //Set up itemPopupView, set up briefcase button 
            //and link itemPopupView to the briefcase button
            itemPopupView = LayoutInflater.Inflate(Resource.Layout.view_popup_layout, null);
            popupRecycler = (RecyclerView)itemPopupView.FindViewById(Resource.Id.popup_recyclerview);

            var itemPopup = new LinearLayoutManager(itemPopupView.Context);
            popupRecycler.SetLayoutManager(itemPopup);

            //Create a list of test items.
            var items = new List<IItem>();

            var adapter = new ItemViewAdapter(items);

            popupRecycler.SetAdapter(adapter);

            var briefcaseButton = FindViewById<ImageButton>(Resource.Id.button_briefcase);

            briefcaseButton.Click += (sender, e) =>
            {
                if (itemPopupDialog == null)
                {
                    itemPopupDialog = new AlertDialog.Builder(this)
	                    .SetView(itemPopupView)
	                    .SetPositiveButton("Apply", (o, ee) =>
	                    {
							/*
							 Insert code that makes the users choice of item from the
							 list become their selected equipment
							*/
	                    })
	                    .SetNegativeButton("Cancel", (o, ee) =>
	                    {
		                    //Insert code for closing dialog without any updates to chosen equipment
		                })
	                    .Create();
                }

                itemPopupDialog.Show();
            };
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
            MapHandler.Map = map;
            MapHandler.Visited = new List<LatLng>();

            //map.CameraChange += Map_CameraChange;

            // Disable scrolling
            if (!MainActivity.DebugMode)
            {
	            googleMap.UiSettings.ScrollGesturesEnabled = false;
	            googleMap.UiSettings.ZoomGesturesEnabled   = false;
            }

            // Set custom theme to map
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.map_theme_dark));
            
            // Get last known location or 0,0 if not known
            // TODO: If not known, show loading dialog
            var location = userLocation == null ? new LatLng(0, 0) : userLocation.ToLatLng();
            chunkHandler.UpdateCoord(location.Latitude, location.Longitude);

            // Create player marker
            AddPlayer(location, 0);

            // Target player with initial zoom
            var position = CameraPosition.InvokeBuilder()
                .Target(location)
                .Zoom(17f)
                .Build();

            // Move camera to player
            googleMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(position));

            googleMap.SetOnMarkerClickListener(this);
        }

        private void Map_CameraChange(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            if (e?.Position?.Target != null)
            {
                chunkHandler.UpdateCoord(e.Position.Target.Latitude, e.Position.Target.Longitude);
            }
        }

        public bool OnMarkerClick(Marker marker)
        {
            // Ignore player
            if (marker.Tag?.ToString() == "player")
                return true;

            /*
             TODO: Check distance
             To get distance in meters, use:
             (int) (LocationManager.GetDistance(userLocation.ToLatLng(), marker.Position) + 0.5)
            */

            battleInfo.State = BottomSheetBehavior.StateCollapsed;
            

            if (!PlayerWithinRange(marker.Position))
            {
                var battleInfoView = FindViewById<LinearLayout>(Resource.Id.layout_battle_info);
                battleInfoView.FindViewById<Button>(Resource.Id.button_battle_info_fight).Enabled = false;
            }



            return true;
        }

        private bool PlayerWithinRange(LatLng target)
        {
            double playerLat = playerMarker.Position.Latitude;
            double playerLon = playerMarker.Position.Longitude;
            double targetLat = target.Latitude;
            double targetLon = target.Longitude;
            float[] results = new float[3];
            Location.DistanceBetween(playerLat, playerLon, targetLat, targetLon, results);
            return results[0] < 100; //100 Meters
        }

        /// <summary>
        /// Creates a marker at the specified position
        /// </summary>
        private void AddMarker(LatLng latLng, string title, BitmapDescriptor icon)
        {
			// Check if marker was already added
			// TODO: Don't know how slow this will be
	        if (markers.ContainsKey(latLng))
		        return;

	        markers.Add(latLng, googleMap.AddMarker(new MarkerOptions()
		        .SetPosition(latLng)
		        .SetTitle(title)
		        .Anchor(0.5f, 0.5f)
		        .SetIcon(icon)));
        }

        /// <summary>
		/// Sets up the player and adds it to <see cref="playerMarker"/>
		/// </summary>
        private void AddPlayer(LatLng position, int playerIcon)
        {
			var marker = googleMap.AddMarker(new MarkerOptions()
				.SetPosition(position)
				.SetTitle(AccountManager.Name)
				.SetIcon(BitmapDescriptorFactory.FromAsset($"player/{playerIcon}.webp")));

			marker.ZIndex = 10f;
			marker.Tag = "player";

			playerMarker = marker;
        }

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
	        if (!firstTime)
		        return;

	        var centerX = mainView.Width  / 2;
	        var centerY = mainView.Height / 2;

	        var radius = (float) Math.Sqrt(centerX * centerX + centerY * centerY);

			// TODO: If app is put in background, this crashes it
	        var animator = ViewAnimationUtils.CreateCircularReveal(mainView, centerX, centerY, 0f, radius);

	        mainView.Visibility = ViewStates.Visible;
	        animator.Start();

	        firstTime = false;
        }

        private void StartBattle()
        {
			// Hide battle info
			battleInfo.State = BottomSheetBehavior.StateHidden;

            var fabUser = FindViewById<FloatingActionButton>(Resource.Id.fabUser);
            fabUser.Hide();

            // Sets the health on the progressbar 
            mainView.FindViewById<ProgressBar>(Resource.Id.progress_battle_health).Progress =
                AccountManager.CurrentUser.Health;

            var battle = new BattleCore(this, battleView, Assets, new EnemySnake());

            var centerX = mainView.Width  / 2;
            var centerY = mainView.Height / 2;

            var radius = (float) Math.Sqrt(centerX * centerX + centerY * centerY);

            var animator = ViewAnimationUtils.CreateCircularReveal(battleView, centerX, centerY, 0f, radius);

            battle.End += () =>
            {
                MusicManager.Play(MusicManager.EMusicTrack.Map);

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

        protected override void OnPause()
        {
	        base.OnPause();

			// Balanced power accuracy wi-fi and cell information to determine location and very rarely gps
			if (locationManager != null)
				locationManager.LocationPriority = LocationRequest.PriorityBalancedPowerAccuracy;
		}

        protected override void OnResume()
        {
	        base.OnResume();

	        if (locationManager != null)
				locationManager.LocationPriority = LocationRequest.PriorityHighAccuracy;
		}
    }
}