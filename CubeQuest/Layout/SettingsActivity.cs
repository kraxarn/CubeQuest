using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Support.V7.Widget;
using Android.Util;
using CubeQuest.ListView.Users;
using System.Collections.Generic;
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
				.Replace(Android.Resource.Id.Content, new SettingsFragment())
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
		public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
		{
			AddPreferencesFromResource(Resource.Xml.preferences);

			FindPreference("credits").PreferenceClick += (sender, args) => 
				OpenCredits();
		}

		private void OpenCredits()
		{
			var developers = new List<UserEntry>
			{
				new UserEntry("developer/dennizlund.webp", "dennizlund", "Designer"),
				new UserEntry("developer/kraxarn.webp",    "kraxarn",    "Developer and designer"),
				new UserEntry("developer/tacmotor.webp",   "tacmotor",   "Inventory designer"),
				new UserEntry("developer/timnnyman.webp",  "TimNNyman",  "Enemy placement and battle system")
			};

			var view = LayoutInflater.Inflate(Resource.Layout.view_dialog_credits, null, false);

			var viewUsers = view.FindViewById<RecyclerView>(Resource.Id.view_users);
			var adapter = new UserEntriesAdapter(developers, Context.Assets);
			viewUsers.SetAdapter(adapter);
			viewUsers.SetLayoutManager(new LinearLayoutManager(Context));
			viewUsers.HasFixedSize = true;
			
			adapter.OnItemClick += itemView =>
				StartActivity(new Intent(Intent.ActionView)
					.SetData(Uri.Parse($"https://github.com/{itemView.Title.Text}")));

			new AlertDialog.Builder(Context)
				.SetView(view)
				.SetTitle("Credits")
				.SetPositiveButton("OK", (IDialogInterfaceOnClickListener) null)
				.Show();
		}
	}
}