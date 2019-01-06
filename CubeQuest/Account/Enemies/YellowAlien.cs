using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class YellowAlien : IEnemy
	{
		public string Icon => "alien_yellow";

		public string Name => "Yellow Alien";

		public string Info => "An alien that's yellow";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}