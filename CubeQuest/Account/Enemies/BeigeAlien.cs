using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class BeigeAlien : IEnemy
	{
		public string Icon => "alien_beige";

		public string Name => "Beige Alien";

		public string Info => "An alien that's beige";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}