using CubeQuest.Account.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Util;

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
		 */

		public int Health { get; set; }

		public float Experience { get; set; }

		public List<Type> EquippedCompanions { get; set; }

		public List<Type> Companions { get; set; }

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
		}

		public override string ToString() => 
			$"{Health}\n{Experience}\n{string.Join(',', EquippedCompanions)}\n{string.Join(',', Companions)}";
	}

	public static class SaveDataExtensions
	{
		public static List<Type> ToTypeList(this IEnumerable<ICompanion> companions) => 
			new List<Type>(companions.Select(companion => companion.GetType()));

		public static List<ICompanion> ToCompanions(this IEnumerable<Type> types) => 
			new List<ICompanion>(types.Select(t => Activator.CreateInstance(t) as ICompanion));

		public static IEnumerable<Type> FormatCompanions(this string text) => 
			text.Split(',').Select(c => c.ToType());

		private static Type ToType(this string value) => 
			Type.GetType(value);
	}
}