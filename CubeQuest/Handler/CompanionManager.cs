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
		public static ICompanion Random => 
			Activator.CreateInstance(CompanionTypes[new Random().Next(CompanionTypes.Length)]) as ICompanion;

        public static IEnumerable<ICompanion> StartingCompanions
        {
            get
            {
                Random r = new Random();
                int n = CompanionTypes.Length;
                int[] indexes = new int[n];
                for (int i = 0; i < indexes.Length; i++)
                    indexes[i] = i;

                while (n > 1)
                {
                    int k = r.Next(n--);
                    int temp = indexes[n];
                    indexes[n] = indexes[k];
                    indexes[k] = temp;
                }

                var list = new ICompanion[3];

                for (var i = 0; i < 3; i++)
                    list[i] = Activator.CreateInstance(CompanionTypes[(indexes[i]) % CompanionTypes.Length]) as ICompanion;

                return list;
            }
        }
	}
}