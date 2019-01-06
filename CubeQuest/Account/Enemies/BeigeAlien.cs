using CubeQuest.Account.Interface;
using CubeQuest.Handler;

namespace CubeQuest.Account.Enemies
{
	public class BeigeAlien : IEnemy
	{
		public ImageHandler.ImageName Image => ImageHandler.ImageName.ALIEN_BEIGE;

		public string Icon => "alien_beige";

		public string Name => "Beige Alien";

		public string Info => "An alien that's beige";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}