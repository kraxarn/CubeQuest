using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class GreenAlien : IEnemy
	{
        public int Level { get; set; }

        public string Icon => "alien_green";

		public string Name => "Green Alien";

		public string Info => "An alien that's green";

		public int Health => 12 * Level;

		public int Armor => Level;

		public int Attack => 5 + (Level / 4);
	}
}