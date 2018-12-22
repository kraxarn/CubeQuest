using Android.App;
using Android.Content;
using Android.Content.Res;
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
using CubeQuest.Account.Weapons;
using System;
using System.Collections.Generic;
using AlertDialog = Android.App.AlertDialog;

namespace CubeQuest
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
        private LocationManager locationManager;

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

        private AlertDialog itemPopupDialog;
        private AlertDialog.Builder popupDialogBuilder;

        private View itemPopupView;

        private RecyclerView popupRecycler;

        private BottomSheetBehavior battleInfo;

        /// <summary>
        /// Value returned from the achievements intent
        /// </summary>
        private const int RcAchievementUi = 9003;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);

			markers = new Dictionary<LatLng, Marker>();

            // Get main view
            mainView = FindViewById<CoordinatorLayout>(Resource.Id.layout_game);
            mainView.Visibility = ViewStates.Invisible;

			var health = AccountManager.CurrentUser.Health;

            FindViewById<ProgressBar>(Resource.Id.barHealth).Progress = health;

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

            AccountManager.Fitness.Success += async status =>
            {
                var sets = await AccountManager.Fitness.GetNumSteps(new DateTime(2018, 12, 1), DateTime.UtcNow);
            };

            //Set up itemPopupView, set up briefcase button 
            //and link itemPopupView to the briefcase button
            itemPopupView = LayoutInflater.Inflate(Resource.Layout.view_popup_layout, null);
            popupRecycler = (RecyclerView)itemPopupView.FindViewById(Resource.Id.popup_recyclerview);

            LinearLayoutManager popupllm = new LinearLayoutManager(itemPopupView.Context);
            popupRecycler.SetLayoutManager(popupllm);

            //Create a list of test items.
            List<IItem> items = new List<IItem>();

            items.Add(new WeaponSword());
            items.Add(new WeaponSword());
            items.Add(new WeaponSword());
            items.Add(new WeaponSword());

            ItemViewAdapter adapter = new ItemViewAdapter(items);

            popupRecycler.SetAdapter(adapter);

            ImageButton briefcaseButton = this.FindViewById<ImageButton>(Resource.Id.button_briefcase);
            briefcaseButton.Click += (sender, e) => {
                if (itemPopupDialog == null)
                {
                    popupDialogBuilder = new AlertDialog.Builder(this)
                    .SetView(itemPopupView);
                    popupDialogBuilder.SetPositiveButton("Apply",
                    (object senderer, Android.Content.DialogClickEventArgs ee) =>
                    {
                        //Insert code that makes the users choice of item from the 
                        //list become their selected equipment
                    });



                    popupDialogBuilder.SetNegativeButton("Cancel",
                    (object senderer, Android.Content.DialogClickEventArgs ee) =>
                    {
                        //Insert code for closing dialog without any updates to chosen equipment
                    });


                    itemPopupDialog = popupDialogBuilder.Create();

                    itemPopupDialog.Show();

                }

                else
                {
                    itemPopupDialog.Show();
                }
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

            // Create player marker
            AddPlayer(location, 0);

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

            /*
             TODO: Check distance
             To get distance in meters, use:
             (int) (LocationManager.GetDistance(userLocation.ToLatLng(), marker.Position) + 0.5)
            */

            battleInfo.State = BottomSheetBehavior.StateCollapsed;

            return true;
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
	        var centerX = mainView.Width / 2;
	        var centerY = mainView.Height / 2;

	        var radius = (float) Math.Sqrt(centerX * centerX + centerY * centerY);

	        var animator = ViewAnimationUtils.CreateCircularReveal(mainView, centerX, centerY, 0f, radius);

	        mainView.Visibility = ViewStates.Visible;
	        animator.Start();
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
;
            var battle = new Battle(this, battleView, Assets, new EnemySnake());

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
    }
}