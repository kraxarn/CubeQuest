using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class BlueAlien : IEnemy
	{
		public string Icon => "alien_blue";

		public string Name => "Blue Alien";

		public string Info => "An alien that's blue";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}