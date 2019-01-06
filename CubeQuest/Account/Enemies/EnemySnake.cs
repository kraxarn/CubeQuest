using System;
using CubeQuest.Account.Interface;

namespace CubeQuest.Account.Enemies
{
	[Obsolete]
    public class EnemySnake : IEnemy
    {
        public string Icon => "snake";

        public string Name => "Danger Noodle";

        public string Info => "A very dangerous noodle compared to regular ones";

        public int Health => 10;

        public int Armor => 0;

        public int Attack => 5;
    }
}