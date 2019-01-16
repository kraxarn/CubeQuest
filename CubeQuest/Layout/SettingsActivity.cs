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
using System.Text;
using AlertDialog = Android.Support.V7.App.AlertDialog;
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

			FindViewById<FloatingActionButton>(Resource.Id.fab_settings_save).Click += (sender, args) =>
			{
				settings.Preferences.Save();
				Finish();
			};
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

			// TODO: Set correct preference summaries here!

			FindPreference("credits").PreferenceClick += (sender, args) => 
				OpenCredits();

			FindPreference("licenses").PreferenceClick += (sender, args) =>
				OpenLicenses();

			FindPreference("save_progress").PreferenceClick += (sender, args) => 
                AccountManager.SaveUserProgress();

			FindPreference("view_progress").PreferenceClick += (sender, args) => 
				StartActivityForResult(AccountManager.SelectSaveIntent, 9003);

            FindPreference("load_progress").PreferenceClick += async (sender, args) =>
            {
                var bytes = await AccountManager.GetUserProgress();
                var str = Encoding.UTF8.GetString(bytes);

                new AlertDialog.Builder(context, Resource.Style.AlertDialogStyle)
                    .SetTitle("Progress")
                    .SetMessage(str)
                    .SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null)
                    .Show();
            };

            FindPreference("reset_progress").PreferenceClick += (sender, args) =>
            {
                new AlertDialog.Builder(context, Resource.Style.AlertDialogStyle)
                    .SetTitle("Are you sure?")
                    .SetMessage("All your progress and companions collected will be lost!")
                    .SetPositiveButton("Yes", (s, a) =>
                    {
                        AccountManager.ResetUserProgress();

                        new AlertDialog.Builder(context, Resource.Style.AlertDialogStyle)
                            .SetTitle("Progress reset")
                            .SetMessage("It's recommended to restart the app to avoid issues")
                            .SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null)
                            .Show();
                    })
                    .SetNegativeButton("No", (IDialogInterfaceOnClickListener) null)
                    .Show();
            };

            FindPreference("debug_preferences").PreferenceClick += (sender, args) =>
            {
	            new AlertDialog.Builder(context, Resource.Style.AlertDialogStyle)
		            .SetTitle("Preferences")
		            .SetMessage(Preferences.ToString())
		            .SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null)
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
				new UserEntry("developer/dennizlund.webp", "dennizlund", "Designer"),
				new UserEntry("developer/kraxarn.webp",    "kraxarn",    "Developer and designer"),
				new UserEntry("developer/tacmotor.webp",   "tacmotor",   "Inventory designer"),
				new UserEntry("developer/timnnyman.webp",  "TimNNyman",  "Enemy placement and battle system")
			});
			
			adapter.OnItemClick += itemView =>
				StartActivity(new Intent(Intent.ActionView)
					.SetData(Uri.Parse($"https://github.com/{itemView.Title.Text}")));

			new AlertDialog.Builder(Context, Resource.Style.AlertDialogStyle)
				.SetView(view)
				.SetTitle("Credits")
				.SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null)
				.Show();
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
				new UserEntry(null,  "fastJSON", "By mgholam (CPOL license)"),
				new UserEntry(null,  "Xamarin",  "By Xamarin Inc. (MIT license)")
			});

			adapter.OnItemClick += itemView =>
				StartActivity(new Intent(Intent.ActionView)
					.SetData(Uri.Parse($"https://github.com/{(itemView.Title.Text == "fastJSON" ? "mgholam/fastJSON" : "xamarin/xamarin-android")}")));

			new AlertDialog.Builder(Context, Resource.Style.AlertDialogStyle)
				.SetView(view)
				.SetTitle("Open Source Licenses")
				.SetPositiveButton("OK", (IDialogInterfaceOnClickListener)null)
				.Show();
		}
    }
}