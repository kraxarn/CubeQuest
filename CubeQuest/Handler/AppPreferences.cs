using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Support.V7.Preferences;

namespace CubeQuest.Handler
{
	public class AppPreferences : PreferenceDataStore
	{
		public enum EMapTheme
		{
			// Matches map_theme_values
			Auto,
			Day,
			Night
		}

		public static IEnumerable<string> PreferenceKeys =>
			new[]
			{
				// Appearance
				"map_theme",
				"fullscreen",
				// Battery saver
				//"black_map",
				//"black_app",
				//"background_updates",
				//"location_accuracy",
				// Audio
				"music_volume",
				//"sound_volume"
			};

		// Appearance
		public EMapTheme MapTheme
		{
			get => Enum.TryParse(GetString("map_theme", "Day"), out EMapTheme theme) ? theme : EMapTheme.Auto;
			set => PutString("map_theme", value.ToString());
		}

		public bool Fullscreen
		{
			get => GetBoolean("fullscreen", false);
			set => PutBoolean("fullscreen", value);
		}

		// Battery saver
		public bool BlackMap
		{
			get => GetBoolean("black_map", false);
			set => PutBoolean("black_map", value);
		}

		public bool BlackApp
		{
			get => GetBoolean("black_app", false);
			set => PutBoolean("black_app", value);
		}

		public bool BackgroundUpdates
		{
			get => GetBoolean("background_updates", true);
			set => PutBoolean("background_updates", value);
		}

		public bool LocationAccuracy
		{
			get => GetBoolean("location_accuracy", false);
			set => PutBoolean("location_accuracy", value);
		}

		// Audio
		public int MusicVolume
		{
			get => GetInt("music_volume", 25);
			set => PutInt("music_volume", value);
		}

		public int SoundVolume
		{
			get => GetInt("sound_volume", 25);
			set => PutInt("sound_volume", value);
		}

		private readonly ISharedPreferences prefs;

		private ISharedPreferencesEditor editor;

		private ISharedPreferencesEditor Editor => 
			editor ?? (editor = prefs.Edit());

		public AppPreferences(Context context) => 
			prefs = PreferenceManager.GetDefaultSharedPreferences(context);

		public void Save() => 
			editor?.Commit();

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach (var pair in KeyValuePairs)
				builder.AppendLine($"{pair.Key}={pair.Value}");

			return builder.ToString();
		}

		public IEnumerable<KeyValuePair<string, string>> KeyValuePairs =>
			new[]
			{
				new KeyValuePair<string, string>("MapTheme",          $"{MapTheme}"),
				new KeyValuePair<string, string>("Fullscreen",        $"{Fullscreen}"),
				//new KeyValuePair<string, string>("BlackMap",          $"{BlackMap}"),
				//new KeyValuePair<string, string>("BlackApp",          $"{BlackApp}"),
				//new KeyValuePair<string, string>("BackgroundUpdates", $"{BackgroundUpdates}"),
				//new KeyValuePair<string, string>("LocationAccuracy",  $"{LocationAccuracy}"),
				new KeyValuePair<string, string>("MusicVolume",       $"{MusicVolume}"),
				//new KeyValuePair<string, string>("SoundVolume",       $"{SoundVolume}")
			};

		public override bool GetBoolean(string key, bool defValue) => 
			prefs.GetBoolean(key, defValue);

		public override int GetInt(string key, int defValue) => 
			prefs.GetInt(key, defValue);

		public override string GetString(string key, string defValue) => 
			prefs.GetString(key, defValue);

		public override void PutBoolean(string key, bool value) => 
			Editor.PutBoolean(key, value);

		public override void PutInt(string key, int value) => 
			Editor.PutInt(key, value);

		public override void PutString(string key, string value) => 
			Editor.PutString(key, value);
	}
}