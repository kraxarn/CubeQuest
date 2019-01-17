using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class PinkAlien : IEnemy
	{
        public int Level { get; set; }

        public string Icon => "alien_pink";

		public string Name => "Pink Alien";

		public string Info => "An alien that's pink";

		public int Health => 40;

		public int Armor => 1;

		public int Attack => 5 + Level;
	}
}