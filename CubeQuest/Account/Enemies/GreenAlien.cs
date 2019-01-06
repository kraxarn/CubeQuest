using CubeQuest.Account.Interface;
using CubeQuest.Handler;

namespace CubeQuest.Account.Enemies
{
	public class GreenAlien : IEnemy
	{
		public ImageHandler.ImageName Image => ImageHandler.ImageName.ALIEN_GREEN;

		public string Icon => "alien_green";

		public string Name => "Green Alien";

		public string Info => "An alien that's green";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}