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
            typeof(Narwhale),
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

        public static ICompanion[] GetStartingCompanions()
        {
            var seed = new Random().Next(Companions.Length);
            var interval = new Random().Next(1,12);

            var list = new ICompanion[3];

            for (var i = 0; i < 3; i++)
            {
                list[i] = Activator.CreateInstance(Companions[(seed + (interval * i)) % Companions.Length]) as ICompanion;
            }

            return list;
        }
	}
}