using System;
using System.Collections.Generic;
using CubeQuest.Account.Companions;
using CubeQuest.Account.Interface;

namespace CubeQuest.Handler
{
	public static class CompanionManager
	{
		private static readonly Type[] CompanionTypes =
		{
			typeof(Bear),
			typeof(Buffalo),
			typeof(Chick),
			typeof(Horse),
            typeof(Narwhal),
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
		public static ICompanion Random => 
			Activator.CreateInstance(CompanionTypes[new Random().Next(CompanionTypes.Length)]) as ICompanion;

        public static IEnumerable<ICompanion> StartingCompanions
        {
            get
            {
                var seed = new Random().Next(CompanionTypes.Length);

                var interval = new Random().Next(1, 12);

                var list = new ICompanion[3];

                for (var i = 0; i < 3; i++)
                    list[i] = Activator.CreateInstance(CompanionTypes[(seed + interval * i) % CompanionTypes.Length]) as ICompanion;

                return list;
            }
        }
	}
}