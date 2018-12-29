using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;

namespace CubeQuest
{
	[Activity(Label = "SettingsActivity", Theme = "@style/AppTheme.NoActionBar")]
	public class SettingsActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SupportFragmentManager
				.BeginTransaction()
				.Replace(Android.Resource.Id.Content, new SettingsFragment())
				.Commit();
		}
	}

	public class SettingsFragment : PreferenceFragmentCompat
	{
		public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey) => 
			AddPreferencesFromResource(Resource.Xml.preferences);
	}
}