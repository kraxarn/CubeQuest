using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CubeQuest.Account;
using CubeQuest.Battle;
using CubeQuest.Handler;
using CubeQuest.ListView.Companions;
using CubeQuest.WorldGen;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Graphics;

namespace CubeQuest.Layout
{
	[Activity(Label = "GameActivity", Theme = "@style/AppTheme.NoActionBar", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
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
        private LocationHandler locationHandler;

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

		/// <summary>
		/// Handles loading chunks for the map
		/// </summary>
        private ChunkHandler chunkHandler;

		/// <summary>
		/// TODO
		/// </summary>
        private View companionInsertView;

		/// <summary>
		/// TODO
		/// </summary>
        private RecyclerView companionRecycler;

		/// <summary>
		/// TODO
		/// </summary>
        private CompanionViewAdapter companionAdapter;

		/// <summary>
		/// Battle info after pressing on en enemy
		/// </summary>
        private BottomSheetBehavior battleInfo;

		/// <summary>
		/// Main view in <see cref="battleInfo"/>
		/// </summary>
        private LinearLayout battleInfoView;

		/// <summary>
		/// Last clicked marker
		/// </summary>
        private Marker selectedMarker;

		/// <summary>
		/// Custom <see cref="ISharedPreferences"/>
		/// </summary>
        private AppPreferences preferences;

        /// <summary>
        /// <see cref="FloatingActionButton"/> for opening the inventory
        /// </summary>
        private FloatingActionButton fabUser;

        /// <summary>
        /// <see cref="FloatingActionButton"/> for returning to the map
        /// </summary>
        private FloatingActionButton fabMap;

        /// <summary>
        /// First time starting the activity
        /// </summary>
        private bool firstTime;

        /// <summary>
        /// If the camera should automatically focus on the player on location updates
        /// </summary>
        private bool autoCamera;

        /// <summary>
        /// Value returned from the achievements intent
        /// </summary>
        private const int RcAchievementUi = 9003;

		/// <summary>
		/// Dictionary with text views for our profile stats
		/// </summary>
        private Dictionary<string, TextView> profileStats;

        private ImageButton[] equippedCubes;

        /// <summary>
        /// If it's day (8-17)
        /// </summary>
        private bool IsDay
        {
	        get
	        {
		        var now = DateTime.Now;
		        return now.Hour < 17 && now.Hour > 8;
			}
        }

		/// <summary>
		/// Selected map theme from time and preferences
		/// </summary>
        private int MapTheme
        {
	        get
	        {
		        switch (preferences.MapTheme)
		        {
					case AppPreferences.EMapTheme.Auto:
						return IsDay ? Resource.Raw.map_theme_day : Resource.Raw.map_theme_night;

					case AppPreferences.EMapTheme.Night:
						return Resource.Raw.map_theme_night;

					case AppPreferences.EMapTheme.Day:
						return Resource.Raw.map_theme_day;

					case AppPreferences.EMapTheme.Midnight:
						return Resource.Raw.map_theme_midnight;

					default:
						throw new ArgumentOutOfRangeException();
		        }
	        }
        }

		/// <summary>
		/// If the map is currently using the day theme
		/// </summary>
		private bool IsMapDay
		{
			get
			{
				switch (preferences.MapTheme)
				{
					case AppPreferences.EMapTheme.Auto:
						return IsDay;

					case AppPreferences.EMapTheme.Day:
						return true;

					case AppPreferences.EMapTheme.Night:
					case AppPreferences.EMapTheme.Midnight:
						return false;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		
		protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);

			firstTime = true;

            // By default, enable auto camera
            autoCamera = true;

            markers = new Dictionary<LatLng, Marker>();
            
			preferences = new AppPreferences(this);
            
            // Get main view
            mainView = FindViewById<CoordinatorLayout>(Resource.Id.layout_game);
            mainView.Visibility = ViewStates.Invisible;
            
            // Get last known location
            locationHandler = new LocationHandler(this);
            userLocation = await locationHandler.GetLastKnownLocationAsync();

            // Get map and listen when it's ready
            var mapFragment = (SupportMapFragment) SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var textDebugLocation = FindViewById<TextView>(Resource.Id.text_debug_location);

            locationHandler.OnLocationUpdate += location =>
                {
                    // TODO: Check speed etc. here
                    // TODO: Show a loading dialog until we get the first position

                    Log.Info("POSITION", $"Long={location.Longitude} Lat={location.Latitude} Speed={location.Speed}");

                    textDebugLocation.Text = $"Accuracy: {location.Accuracy}%\nSpeed:    {location.Speed} m/s";

                    // Update user location
                    userLocation = location;

                    if (location.Speed > 0 && battleView.Visibility != ViewStates.Visible)
                        AccountManager.CurrentUser.Health += 1;

                    // If map hasn't loaded yet, ignore player marker
                    if (playerMarker == null)
                        return;

                    playerMarker.Position = location.ToLatLng();

					if (autoCamera)
						googleMap.AnimateCamera(CameraUpdateFactory.NewLatLng(location.ToLatLng()));
                };

            // Show profile when clicking on button
            fabUser = FindViewById<FloatingActionButton>(Resource.Id.fabUser);
            fabMap  = FindViewById<FloatingActionButton>(Resource.Id.fabGame);

            fabUser.Click += (sender, args) => ToggleProfile(true);
            fabMap.Click  += (sender, args) => ToggleProfile(false);

            // Set up profile view
            profileView = FindViewById<ViewStub>(Resource.Id.stub_profile).Inflate();

            // Hide it in code to be able to see it when designing in xml
            profileView.Visibility = ViewStates.Invisible;

            // Set values on profile
            profileView.FindViewById<ImageButton>(Resource.Id.button_settings).Click += (sender, args) => 
	            StartActivity(new Intent(this, typeof(SettingsActivity)));

            equippedCubes = new ImageButton[3];

            equippedCubes[0] = profileView.FindViewById<ImageButton>(Resource.Id.inventory_companion_1);
            equippedCubes[1] = profileView.FindViewById<ImageButton>(Resource.Id.inventory_companion_2);
            equippedCubes[2] = profileView.FindViewById<ImageButton>(Resource.Id.inventory_companion_3);
			
            // Avoid clicking through profile view
            profileView.Touch += (sender, args) =>
                args.Handled = true;

            // Inflate battle view
            battleView = FindViewById<ViewStub>(Resource.Id.stub_battle).Inflate();
            battleView.Visibility = ViewStates.Invisible;

            // Avoid clicking through battle view
            battleView.Touch += (sender, args) =>
                args.Handled = true;
			
			// Check if using fullscreen HUD
			if (preferences.Fullscreen)
			{
				void HudUpdate()
				{
					var batteryManager = (BatteryManager) GetSystemService(BatteryService);
					var hud = mainView.FindViewById<TextView>(Resource.Id.text_game_hud);
					hud.SetTextColor(IsMapDay ? Color.Black : Color.White);

					Task.Run(() =>
					{
						while (true)
						{
							var time = DateTime.Now.ToString("HH:mm");
							var battery = batteryManager.GetIntProperty((int) BatteryProperty.Capacity);
							hud.Text = $"{time} | {battery}%";

							Thread.Sleep(1000);
						}
					});
				}

				// Wrap it in a function to avoid an async warning
				HudUpdate();
			}

            // Setup debug mode
            FindViewById<Button>(Resource.Id.button_debug_focus_player).Click += (sender, args) => 
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLng(userLocation.ToLatLng()));

            FindViewById<Button>(Resource.Id.button_debug_respawn).Click += (sender, args) => 
	            AccountManager.CurrentUser.HealthPercentage = 100;

            battleInfoView = FindViewById<LinearLayout>(Resource.Id.layout_battle_info);
            battleInfo = BottomSheetBehavior.From(battleInfoView);
            battleInfo.State = BottomSheetBehavior.StateHidden;

            battleInfoView.FindViewById<Button>(Resource.Id.button_battle_info_fight).Click +=
                (sender, args) => StartBattle();

			// Add auto camera toggle
            FindViewById<Switch>(Resource.Id.switch_debug_auto_camera).CheckedChange += (sender, args) => 
	            autoCamera = args.IsChecked;

            if (!MainActivity.DebugMode)
	            FindViewById<LinearLayout>(Resource.Id.layout_debug_tools).Visibility = ViewStates.Gone;

			SetUser();
        }

		/// <summary>
		/// Set everything up that has to do with the user
		/// </summary>
		private async void SetUser()
		{
			// Try to get our user
			await AccountManager.LoadCurrentUser();

			// Get health bar and heart
			var healthBar = FindViewById<ProgressBar>(Resource.Id.barHealth);
			var healthBarHeart = FindViewById<ImageView>(Resource.Id.barHeart);

			// Initial health bar progress
			healthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

			// Update health bar when changing health
			AccountManager.CurrentUser.OnHealthChange += health =>
				healthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

			// When you die the health bar and heart is set.
			AccountManager.CurrentUser.OnDeadChange += isAlive => healthBar.Alpha = healthBarHeart.Alpha = isAlive ? 1f : 0.5f;

			// Set name on profile
			profileView.FindViewById<TextView>(Resource.Id.textProfileName).Text = AccountManager.Name;

			// Set equipped companion icons
			equippedCubes[0].SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[0]));
			equippedCubes[1].SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[1]));
			equippedCubes[2].SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[2]));

			// Create recyclerView for companions
			companionRecycler = FindViewById<RecyclerView>(Resource.Id.companion_list);
			companionAdapter = new CompanionViewAdapter(AccountManager.CurrentUser.Companions, this);
			var companionLayoutManager = new LinearLayoutManager(this);

			companionRecycler.SetAdapter(companionAdapter);
			companionRecycler.SetLayoutManager(companionLayoutManager);

			companionAdapter.EquippedCompanionChanged += a_companionChanged;
			
			/*
			 * Load chunk loader
			 * (This is needed here since the monsters are based on the user level)
			 */
			chunkHandler = new ChunkHandler();

			if (userLocation != null)
				chunkHandler.UpdateCoord(userLocation.Latitude, userLocation.Longitude);
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
            MapHandler.Init(map, DateTime.Now.ToString("dd/MM/yyyy").GetHashCode());

            map.CameraChange += Map_CameraChange;

            // Disable scrolling
            if (!MainActivity.DebugMode)
            {
                googleMap.UiSettings.ScrollGesturesEnabled = false;
                googleMap.UiSettings.ZoomGesturesEnabled = false;
            }

            // Set custom theme to map
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, MapTheme));

            // Get last known location or 0,0 if not known
            // TODO: If not known, show loading dialog
            var location = userLocation == null ? new LatLng(0, 0) : userLocation.ToLatLng();
            
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
                chunkHandler?.UpdateCoord(e.Position.Target.Latitude, e.Position.Target.Longitude);
        }

        public void SetText(Dictionary<TextView, string> views)
        {
	        foreach (var (key, value) in views)
		        key.Text = value;
        }

        public bool OnMarkerClick(Marker marker)
        {
            // Ignore player
            if (marker.Tag?.ToString() == "player")
                return true;

            if (marker.Tag is EnemyTag tag)
            {
	            var enemy = tag.Enemy;

				// Set icon
	            battleInfoView.FindViewById<ImageView>(Resource.Id.image_battle_info)
		            .SetImageBitmap(AssetLoader.GetEnemyBitmap(enemy));

				// Set title
	            battleInfoView.FindViewById<TextView>(Resource.Id.text_battle_info_title).Text =
		            $"Level {enemy.Level} {enemy.Name}";

				// Set description
	            battleInfoView.FindViewById<TextView>(Resource.Id.text_battle_info_description).Text = enemy.Info;

				// Set stats
				battleInfoView.FindViewById<TextView>(Resource.Id.text_battle_info_health).Text = $"{enemy.Health}";
				battleInfoView.FindViewById<TextView>(Resource.Id.text_battle_info_armor).Text  = $"{enemy.Armor}";
				battleInfoView.FindViewById<TextView>(Resource.Id.text_battle_info_attack).Text = $"{enemy.Attack}";
			}

            selectedMarker = marker;

            battleInfo.State = BottomSheetBehavior.StateCollapsed;

            int range = 5000; //in meters
            bool isWithinRange = LocationHandler.GetDistance(userLocation.ToLatLng(), marker.Position) < range;
			
            battleInfoView.FindViewById<Button>(Resource.Id.button_battle_info_fight).Enabled = isWithinRange && AccountManager.CurrentUser.IsAlive;
            battleInfoView.FindViewById<Button>(Resource.Id.button_battle_info_fight).Alpha = isWithinRange && AccountManager.CurrentUser.IsAlive ? 1 : 0.5f;
            
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
                .SetIcon(AssetLoader.GetPlayerBitmap(playerIcon).ToBitmapDescriptor()));
            
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
            if (!locationHandler?.IsLocationServicesEnabled ?? true)
            {
                // TODO: Show fullscreen until user enabled location
                Alert.Build(this)
                    .SetTitle("Location Required")
                    .SetMessage("GPS is required to get your location, but it's disabled")
                    .SetPositiveButton("Enable It", (sender, args) => StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings)))
                    .Show();
            }
        }

        private void ToggleProfile(bool enabled)
        {
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
            var centerY = fabUser.Top + fabUser.Height / 2;

            // Button radius
            var radius = (float)Math.Sqrt(centerX * centerX + centerY * centerY);

            var animator = ViewAnimationUtils.CreateCircularReveal(profileView, centerX, centerY, enabled ? 0f : radius, enabled ? radius : 0f);

            // Hide or show view
            if (enabled)
                profileView.Visibility = ViewStates.Visible;
            else
                animator.AnimationEnd += (o, eventArgs) => profileView.Visibility = ViewStates.Invisible;

			// Update values on opening profile
			if (enabled)
			{
				if (profileStats == null)
				{
					TextView FindTextView(int id) => 
						mainView.FindViewById<TextView>(id);

					profileStats = new Dictionary<string, TextView>
					{
						{"hp",      FindTextView(Resource.Id.text_profile_hp)},
						{"attack",  FindTextView(Resource.Id.text_profile_attack)},
						{"evasion", FindTextView(Resource.Id.text_profile_evasion)},
						{"level",   FindTextView(Resource.Id.text_profile_level_info)},
						{"armor",   FindTextView(Resource.Id.text_profile_armor)}
					};
				}

				var user = AccountManager.CurrentUser;

				profileStats["hp"].Text      = $"{user.HealthPercentage}%";
				profileStats["attack"].Text  = $"{user.Attack}";
				profileStats["evasion"].Text = $"{user.Evasion * 100}%";
				profileStats["level"].Text   = $"Level {user.Level} ({user.ExperienceToNextLevel} xp to next level)";
				profileStats["armor"].Text   = $"{user.Armor}";
			}

            animator.Start();
        }

        private void StartEnterAnimation()
        {
            if (!firstTime)
                return;

            var centerX = mainView.Width / 2;
            var centerY = mainView.Height / 2;

            var radius = (float)Math.Sqrt(centerX * centerX + centerY * centerY);

            // TODO: If app is put in background, this crashes it (IllegalStateException)
            var animator = ViewAnimationUtils.CreateCircularReveal(mainView, centerX, centerY, 0f, radius);

            mainView.Visibility = ViewStates.Visible;
            animator.Start();

            firstTime = false;
        }

        private void StartBattle()
        {
            // Hide battle info
            battleInfo.State = BottomSheetBehavior.StateHidden;

            // Hide profile button
            fabUser.Hide();

            // Sets the health on the progressbar 
            mainView.FindViewById<ProgressBar>(Resource.Id.progress_battle_health).Progress =
                AccountManager.CurrentUser.HealthPercentage;

            var battle = new BattleCore(this, battleView, (selectedMarker.Tag as EnemyTag)?.Enemy);

            var centerX = mainView.Width / 2;
            var centerY = mainView.Height / 2;

            var radius = (float)Math.Sqrt(centerX * centerX + centerY * centerY);

            var animator = ViewAnimationUtils.CreateCircularReveal(battleView, centerX, centerY, 0f, radius);

            battle.End += type =>
            {
                MusicManager.Play(MusicManager.EMusicTrack.Map);

                new Thread(() =>
                {
                    RunOnUiThread(() =>
                        {
                            switch (type)
                            {
                                case BattleCore.EBattleEndType.Won:
                                    if (selectedMarker != null)
                                    {
                                        // TODO: Two different locations and get the same hash code
                                        MapHandler.Visited.Add(selectedMarker.Position.GetHashCode(), selectedMarker.Position);
                                        selectedMarker.Remove();
                                    }

                                    var dialogView = LayoutInflater.Inflate(Resource.Layout.view_dialog_loot, null);

                                    string GetStringPrefix(string value)
                                    {
	                                    switch (value[0])
	                                    {
											case 'e':
											case 'o':
												return "an";

											default:
												return "a";
	                                    }
                                    }

                                    var companion = CompanionManager.Random;
                                    dialogView.FindViewById<TextView>(Resource.Id.text_loot_title).Text = 
	                                    $"You have found {GetStringPrefix(companion.Name)} {companion.Name}!";
                                    dialogView.FindViewById<ImageView>(Resource.Id.image_loot).SetImageBitmap(AssetLoader.GetCompanionBitmap(companion));
                                    AccountManager.CurrentUser.AddCompanion(companion);

                                    companionAdapter.NotifyDataSetChanged();

									Alert.ShowSimple(this, $"{companion.Name}!", dialogView);

                                    AccountManager.CurrentUser.AddExperience(10);
                                    break;

                                case BattleCore.EBattleEndType.Lost:
									Alert.ShowSimple(this, 
										"You died!",
										"You won't be able to attack enemies until you get at least 25% health by walking");
                                    break;

                                case BattleCore.EBattleEndType.Ran:
									Alert.ShowSimple(this, 
										"You ran away!", 
										"You successfully ran away from the alien, but took some damage");
                                    break;
                            }
                        }
                    );
                }).Start();

                var animator2 = ViewAnimationUtils.CreateCircularReveal(battleView, centerX, centerY, radius, 0f);
                animator2.AnimationEnd += (o, eventArgs) =>
                {
                    battleView.Visibility = ViewStates.Invisible;
                    fabUser.Show();
                };
                animator2.Start();
            };

            battleView.Visibility = ViewStates.Visible;
            animator.Start();
        }

        public override void OnBackPressed()
        {
            // Hide battle info
            battleInfo.State = BottomSheetBehavior.StateHidden;

            // If on profile, go back to map, otherwise, ignore
            if (profileView.Visibility == ViewStates.Visible)
                ToggleProfile(false);
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Balanced power accuracy wi-fi and cell information to determine location and very rarely gps
            if (locationHandler != null)
            {
	            locationHandler.LocationPriority = preferences.GpsEnabled
		            ? LocationRequest.PriorityBalancedPowerAccuracy
		            : LocationRequest.PriorityHighAccuracy;

				// If background updating is disabled, stop listening for location updates
				if (!preferences.BackgroundUpdates)
					locationHandler.Stop();
			}
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (locationHandler != null)
            {
	            locationHandler.LocationPriority = LocationRequest.PriorityHighAccuracy;

				// If background updating is disabled, start listening for locations again
				if (!preferences.BackgroundUpdates)
					locationHandler.Start();
            }

            googleMap?.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, MapTheme));

			// Enable or disable fullscreen
			mainView.SystemUiVisibility = (StatusBarVisibility) (preferences.Fullscreen ? SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky | SystemUiFlags.Fullscreen : 0);
        }

        protected override void OnStop()
        {
	        base.OnStop();
			AccountManager.SaveUserProgress();
        }

        protected void a_companionChanged(object sender, EventArgs e)
        {
            equippedCubes[0].SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[0]));
            equippedCubes[1].SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[1]));
            equippedCubes[2].SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[2]));

        }
    }
}