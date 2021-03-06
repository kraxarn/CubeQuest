﻿using CubeQuest.Account.Interface;
using System;

namespace CubeQuest.Account
{
	public class EnemyTag : Java.Lang.Object
	{
		private readonly Type enemyType;

		private readonly int level;

		public IEnemy Enemy
        {
            get
            {
                var enemy = (IEnemy) Activator.CreateInstance(enemyType);
                enemy.Level = level;
                return enemy;
            }
        }

        public EnemyTag(Type enemyType, int level)
		{
			this.enemyType = enemyType;
			this.level     = level;
		}
	}
}