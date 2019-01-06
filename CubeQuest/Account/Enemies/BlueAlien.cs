using CubeQuest.Account.Interface;
using CubeQuest.Handler;

namespace CubeQuest.Account.Enemies
{
	public class BlueAlien : IEnemy
	{
		public ImageHandler.ImageName Image => ImageHandler.ImageName.ALIEN_BLUE;

		public string Icon => "alien_blue";

		public string Name => "Blue Alien";

		public string Info => "An alien that's blue";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}