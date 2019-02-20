using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CubeQuest.Account;
using CubeQuest.Handler;
using CubeQuest.ListView.Users;
using System.Collections.Generic;
using Environment = System.Environment;
using Uri = Android.Net.Uri;

namespace CubeQuest.Layout
{
	[Activity(Label = "SettingsActivity", Theme = "@style/AppTheme.NoActionBar")]
	public class SettingsActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_settings);
			
			var settings = (SettingsFragment) SupportFragmentManager.FindFragmentById(Resource.Id.fragment_settings);
			settings.AppContext = this;
			settings.VersionInfo = VersionInfo;

			FindViewById<FloatingActionButton>(Resource.Id.fab_settings_save).Click += (sender, args) =>
			{
				settings.Preferences.Save();
				Finish();
			};
		}

		private (int, string) VersionInfo
		{
			get
			{
				var info = PackageManager.GetPackageInfo(PackageName, 0);
				return (info.VersionCode, info.VersionName);
			}
		}
	}

	public class SettingsFragment : PreferenceFragmentCompat
	{
		private Context context;

		public Context AppContext
		{
			set
			{
				context = value;
				Preferences = new AppPreferences(value);
			}
		}

		private string versionName;
		
		public (int code, string name) VersionInfo
		{
			set
			{
				FindPreference("version").Summary = $"v{value.name} ({value.code})";
				versionName = value.name;
			}
		}

		public AppPreferences Preferences { get; private set; }

		public bool Fullscreen
        {
	        get => View.SystemUiVisibility != 0;
	        set => View.SystemUiVisibility = (StatusBarVisibility) (value ? SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky | SystemUiFlags.Fullscreen : 0);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
			AddPreferencesFromResource(Resource.Xml.preferences);
			
			// Add all to preference manager
			foreach (var key in AppPreferences.PreferenceKeys)
				FindPreference(key).PreferenceDataStore = Preferences;

			FindPreference("credits").PreferenceClick += (sender, args) => 
				OpenCredits();

			FindPreference("licenses").PreferenceClick += (sender, args) =>
				OpenLicenses();

			FindPreference("version").PreferenceClick += (sender, args) =>
				OpenAbout();

			FindPreference("reset_progress").PreferenceClick += (sender, args) =>
            {
                Alert.Build(context)
                    .SetTitle(Resource.String.are_you_sure)
                    .SetMessage(Resource.String.reset_progress_notice)
                    .SetPositiveButton(Resource.String.yes, (s, a) =>
                    {
                        AccountManager.ResetUserProgress();
                        Alert.ShowSimple(context, 
	                        GetString(Resource.String.reset_progress_title),
							GetString(Resource.String.reset_progress_message));

					})
                    .SetNegativeButton("No", (IDialogInterfaceOnClickListener) null)
                    .Show();
            };

            FindPreference("sign_out").PreferenceClick += (sender, args) =>
            {
	            Alert.Build(context)
		            .SetTitle(Resource.String.are_you_sure)
		            .SetMessage("You'll need to re-authenticate again next time and your progress may be lost")
		            .SetPositiveButton("Yes", async (s, a) =>
		            {
			            var ok = await AccountManager.SignOut();

			            if (ok)
				            Environment.Exit(Environment.ExitCode);
			            else
				            Alert.ShowSimple(context,
					            "Couldn't sign out",
					            "There was an unknown error signing you out, maybe you already signed out?");
		            })
		            .SetNegativeButton("No", (IDialogInterfaceOnClickListener)null)
		            .Show();
            };
        }
		
        public override void OnStart()
        {
	        base.OnStart();
	        Fullscreen = Preferences.Fullscreen;
		}

        public override bool OnPreferenceTreeClick(Preference preference)
        {
	        switch (preference.Key)
	        {
				case "fullscreen":
					Fullscreen = (preference as SwitchPreferenceCompat)?.Checked ?? false;
					break;
	        }

			Preferences.Save();
	        return base.OnPreferenceTreeClick(preference);
        }

        /// <summary>
		/// Builds the user entries adapter with the view and entries provided
		/// </summary>
		private UserEntriesAdapter BuildUserEntriesAdapter(View view, IEnumerable<UserEntry> entries)
		{
			var viewUsers = view.FindViewById<RecyclerView>(Resource.Id.view_users);
			var adapter = new UserEntriesAdapter(entries);
			viewUsers.SetAdapter(adapter);
			viewUsers.SetLayoutManager(new LinearLayoutManager(Context));
			viewUsers.HasFixedSize = true;

			return adapter;
		}

		/// <summary>
		/// Opens the credits dialog
		/// </summary>
		private void OpenCredits()
		{
			var view = LayoutInflater.Inflate(Resource.Layout.view_dialog_credits, null, false);
			var adapter = BuildUserEntriesAdapter(view, new List<UserEntry>
			{
				new UserEntry("developer/dennizlund.webp", "dennizlund", "Designer and battle system"),
				new UserEntry("developer/kraxarn.webp",    "kraxarn",    "Developer and designer"),
				new UserEntry("developer/tacmotor.webp",   "tacmotor",   "Inventory designer"),
				new UserEntry("developer/timnnyman.webp",  "TimNNyman",  "Enemy placement")
			});
			
			adapter.ItemClick += itemView =>
				StartActivity(new Intent(Intent.ActionView)
					.SetData(Uri.Parse($"https://github.com/{itemView.Title.Text}")));

			Alert.ShowSimple(Context, "Credits", view);
		}

		/// <summary>
		/// Opens the licenses dialog
		/// </summary>
		private void OpenLicenses()
		{
			var view = LayoutInflater.Inflate(Resource.Layout.view_dialog_credits, null, false);

			view.FindViewById<TextView>(Resource.Id.text_special_thanks).Text =
				"Special thanks to Kenney (CC0) for the assets and Juhani Junkala (CC0) for the music";

			var adapter = BuildUserEntriesAdapter(view, new List<UserEntry>
			{
				new UserEntry(null,  "Xamarin",  "By Xamarin Inc. (MIT license)")
			});

			adapter.ItemClick += itemView =>
				StartActivity(new Intent(Intent.ActionView)
					.SetData(Uri.Parse("https://github.com/xamarin/xamarin-android")));
			
			Alert.ShowSimple(Context, "Open Source Licenses", view);
		}

		private void OpenAbout()
		{
			var view = LayoutInflater.Inflate(Resource.Layout.view_dialog_credits, null, false);

			view.FindViewById<TextView>(Resource.Id.text_special_thanks).Visibility = ViewStates.Gone;

			var adapter = BuildUserEntriesAdapter(view, new List<UserEntry>
			{
				new UserEntry(null,  "Open on Google Play",  "View the store entry"),
				new UserEntry(null,  "Open on GitHub",       "View the source code")
			});

			adapter.ItemClick += itemView =>
			{
				var isGooglePlay = itemView.Title.Text.Contains("Google Play");

				var intent = new Intent(Intent.ActionView)
					.SetData(Uri.Parse(isGooglePlay
						? $"https://play.google.com/store/apps/details?id={context.PackageName}"
						: "https://github.com/kraxarn/CubeQuest"))
					.SetPackage(isGooglePlay ? "com.android.vending" : null);

				StartActivity(intent);
			};

			Alert.ShowSimple(Context, $"Cube Quest {versionName}", view);
		}
    }
}