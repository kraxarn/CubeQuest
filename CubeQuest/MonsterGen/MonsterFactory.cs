using CubeQuest.Account.Enemies;
using CubeQuest.Account.Interface;

namespace CubeQuest.MonsterGen
{
	static class MonsterFactory
    {
        public static IEnemy CreateMonster(double val)
        {
            IEnemy enemy = null;

            if (val <= 0.85)
	            enemy = new PinkAlien();
            else if (val <= 0.9)
	            enemy = new YellowAlien();
            else if (val <= 0.95)
	            enemy = new GreenAlien();
            else if (val <= 0.98)
	            enemy = new BlueAlien();
            else if (val <= 1)
	            enemy = new BeigeAlien();

            return enemy;
        }
    }
}