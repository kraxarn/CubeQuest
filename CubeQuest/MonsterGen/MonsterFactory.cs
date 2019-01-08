using CubeQuest.Account.Enemies;
using CubeQuest.Account.Interface;
using Java.Lang;

namespace CubeQuest.MonsterGen
{
	static class MonsterFactory
    {
        public static IEnemy CreateMonster(double val)
        {
            IEnemy enemy;

            // Get number between 0-10 and reduce 5
            var level = Math.Abs(val.GetHashCode() % 11) - 5;

            if (val <= 0.85)
            {
                enemy = new PinkAlien();
                // Pink alien is always higher/equal
                level += 5;
            }
            else if (val <= 0.9)
	            enemy = new YellowAlien();
            else if (val <= 0.95)
	            enemy = new GreenAlien();
            else if (val <= 0.98)
	            enemy = new BlueAlien();
            else
	            enemy = new BeigeAlien();

            enemy.Level = level;

            return enemy;
        }
    }
}