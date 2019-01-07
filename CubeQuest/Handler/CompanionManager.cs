using System;
using CubeQuest.Account.Companions;
using CubeQuest.Account.Interface;

namespace CubeQuest.Handler
{
	public static class CompanionManager
	{
		private static readonly Type[] Companions =
		{
			typeof(Bear),
			typeof(Buffalo),
			typeof(Chick),
			typeof(Horse),
			typeof(Owl),
			typeof(Parrot),
			typeof(Penguin),
			typeof(Rabbit),
			typeof(Sloth),
			typeof(Snake),
			typeof(Walrus),
			typeof(Wolf)
		};

		/// <summary>
		/// Get a random companion
		/// </summary>
		public static ICompanion GetRandom() => 
			Activator.CreateInstance(Companions[new Random().Next(Companions.Length)]) as ICompanion;
	}
}