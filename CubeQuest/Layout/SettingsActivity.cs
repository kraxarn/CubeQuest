﻿using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Support.V7.Widget;
using Android.Util;
using CubeQuest.ListView.Users;
using System.Collections.Generic;
using System.Text;
using Android.Views;
using Android.Widget;
using CubeQuest.Account;
using CubeQuest.Handler;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace CubeQuest.Layout
{
	[Activity(Label = "SettingsActivity", Theme = "@style/AppTheme.NoActionBar")]
	public class SettingsActivity : AppCompatActivity, ISharedPreferencesOnSharedPreferenceChangeListener
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SupportFragmentManager
				.BeginTransaction()
				.Replace(Android.Resource.Id.Content, new SettingsFragment(this))
				.Commit();

			PreferenceManager.GetDefaultSharedPreferences(this)
				.RegisterOnSharedPreferenceChangeListener(this);
		}

		public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
		{
			switch (key)
			{
				default:
					Log.Info("PREFERENCES", $"Ignoring {key} as key wasn't found");
					break;
			}
		}
	}

	public class SettingsFragment : PreferenceFragmentCompat
    {
        private readonly Context context;

        private readonly AppPreferences prefs;

        public SettingsFragment(Context context)
        {
	        this.context = context;
			prefs = new AppPreferences(context);
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
		{
			AddPreferencesFromResource(Resource.Xml.preferences);
			
			// Add all to preference manager
			foreach (var key in AppPreferences.PreferenceKeys)
				FindPreference(key).PreferenceDataStore = prefs;

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
                new AlertDialog.Builder(context)
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
		            .SetMessage(prefs.ToString())
		            .SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null)
		            .Show();
            };
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

		public override void OnStop()
		{
			base.OnStop();
			prefs.Save();
		}
    }
}