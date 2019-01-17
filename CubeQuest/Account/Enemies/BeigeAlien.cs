using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class BeigeAlien : IEnemy
	{
        public int Level { get; set; }

        public string Icon => "alien_beige";

		public string Name => "Beige Alien";

		public string Info => "An alien that's beige";

		public int Health => 5 * Level;

		public int Armor => 1;

		public int Attack => 1 + (Level / 6);
	}
}