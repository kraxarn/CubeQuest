using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class PinkAlien : IEnemy
	{
		public string Icon => "alien_pink";

		public string Name => "Pink Alien";

		public string Info => "An alien that's pink";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}