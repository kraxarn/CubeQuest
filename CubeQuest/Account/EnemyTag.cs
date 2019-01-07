using CubeQuest.Account.Interface;
using System;

namespace CubeQuest.Account
{
	public class EnemyTag : Java.Lang.Object
	{
		private readonly Type enemyType;

		private readonly int level;

		public IEnemy Enemy => 
			Activator.CreateInstance(enemyType) as IEnemy;

		public EnemyTag(Type enemyType, int level)
		{
			this.enemyType = enemyType;
			this.level     = level;
		}
	}
}