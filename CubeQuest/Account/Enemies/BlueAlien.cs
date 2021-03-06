﻿using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	public class BlueAlien : IEnemy
	{
        public int Level { get; set; }

        public string Icon => "alien_blue";

		public string Name => "Blue Alien";

		public string Info => "An alien that's blue";

		public int Health => 7 * Level;
        
        public int Armor => Level;

		public int Attack => 3 + (Level / 5);
	}
}