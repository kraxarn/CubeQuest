using Android.Gms.Maps.Model;
using Android.Util;
using CubeQuest.Account.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeQuest.Account
{
	/// <summary>
	/// Minimal version of <see cref="User"/> containing all important information for saving
	/// </summary>
	public class SaveData
	{
		/*
		 * String data:
		 * 0:	Health
		 * 1:	Experience
		 * 2:	Comma separated EquippedCompanions
		 * 3:	Comma separated Companions
		 *
		 * 4:	LastSeed
		 * 5:	Space separated coordinates (Lat_Lng)
		 */

		public int Health { get; set; }

		public float Experience { get; set; }

		public List<Type> EquippedCompanions { get; set; }

		public List<Type> Companions { get; set; }

		public int LastSeed { get; set; }

		public List<LatLng> DefeatedEnemies { get; set; }

		public SaveData()
		{
		}

		public SaveData(string text)
		{
			var data = text.Split('\n');

			if (data.Length < 4)
			{
				var error = $"Text length is {data.Length}, expected 4";
				Log.Error("SAVE_DATA", error);
				throw new InvalidOperationException(error);
			}

			// Health
			if (int.TryParse(data[0], out var health))
				Health = health;

			// Experience
			if (float.TryParse(data[1], out var experience))
				Experience = experience;

			// Equipped companions
			EquippedCompanions = data[2].FormatCompanions().ToList();

			// All companions
			// This value can be empty
			Companions = string.IsNullOrEmpty(data[3]) ? new List<Type>() : data[3].FormatCompanions().ToList();

			// Version 2 with defeated enemies (6 lines)
			/*
			if (data.Length > 5)
			{
				if (int.TryParse(data[4], out var lastSeed))
					LastSeed = lastSeed;

				DefeatedEnemies = data[5].FormatCoordinates().ToList();
			}
			else
			{
				LastSeed = 0;
				DefeatedEnemies = new List<LatLng>();
			}
			*/
		}

		public override string ToString() => 
			$"{Health}\n{Experience}\n{string.Join(',', EquippedCompanions)}\n{string.Join(',', Companions)}";
	}

	public static class SaveDataExtensions
	{
		// -- Saving -- //

		public static List<Type> ToTypeList(this IEnumerable<ICompanion> companions) =>
			new List<Type>(companions.Select(companion => companion.GetType()));

		public static string ToSaveString(this IEnumerable<LatLng> coordinates) => 
			string.Join(' ', coordinates.Select(c => c.ToSaveString()));

		// -- Loading -- //

		public static List<ICompanion> ToCompanions(this IEnumerable<Type> types) =>
			new List<ICompanion>(types.Select(t => Activator.CreateInstance(t) as ICompanion));

		public static IEnumerable<Type> FormatCompanions(this string text) =>
			text.Split(',').Select(c => c.ToType());

		public static IEnumerable<LatLng> FormatCoordinates(this string text) =>
			text.Split(' ').Select(l => l.ToLatLng());

		// -- Private helpers -- //

		private static Type ToType(this string value) =>
			Type.GetType(value);
		
		private static LatLng ToLatLng(this string value)
		{
			var v = value.Split('_');
			return new LatLng(double.Parse(v[0]), double.Parse(v[1]));
		}

		private static string ToSaveString(this LatLng coordinate) => 
			$"{coordinate.Latitude}_{coordinate.Longitude}";
	}
}