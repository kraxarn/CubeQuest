using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class YellowAlien : IEnemy
	{
        public int Level { get; set; }

        public string Icon => "alien_yellow";

		public string Name => "Yellow Alien";

		public string Info => "An alien that's yellow";

		public int Health => 17 * Level;

		public int Armor => 3 * Level;

		public int Attack => 7 + (Level / 2);
	}
}