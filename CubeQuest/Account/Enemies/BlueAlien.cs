using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class BlueAlien : IEnemy
	{
        public int Level { get; set; }

        public string Icon => "alien_blue";

		public string Name => "Blue Alien";

		public string Info => "An alien that's blue";

		public int Health => 1;
        
        public int Armor => 1;

		public int Attack => 1 + (Level / 5);
	}
}