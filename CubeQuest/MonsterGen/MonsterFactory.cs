using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CubeQuest.Account.Enemies;
using CubeQuest.Account.Interface;

namespace CubeQuest.MonsterGen
{
    class MonsterFactory
    {
        public static IEnemy CreateMonster(double val)
        {
            IEnemy enemy;
            if(val <= 0.7)
            {
                enemy = new PinkAlien();
            }
            else if (val <= 0.8)
            {
                enemy = new YellowAlien();
            }
            else if (val <= 0.9)
            {
                enemy = new GreenAlien();
            }
            else if (val <= 0.95)
            {
                enemy = new BlueAlien();
            }
            else if (val <= 1)
            {
                enemy = new BeigeAlien();
            }
            else
            {
                enemy = new EnemySnake();
            }
            return enemy;
        }
    }
}