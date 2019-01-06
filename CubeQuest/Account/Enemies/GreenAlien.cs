using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class GreenAlien : IEnemy
	{
		public string Icon => "alien_green";

		public string Name => "Green Alien";

		public string Info => "An alien that's green";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}