﻿using CubeQuest.Account.Interface;
using CubeQuest.Handler;

namespace CubeQuest.Account.Enemies
{
	public class PinkAlien : IEnemy
	{
		public ImageHandler.ImageName Image => ImageHandler.ImageName.ALIEN_PINK;

		public string Icon => "alien_pink";

		public string Name => "Pink Alien";

		public string Info => "An alien that's pink";

		public int Health => 1;

		public int Armor => 1;

		public int Attack => 1;
	}
}