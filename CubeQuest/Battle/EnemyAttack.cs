using System;
using Android.Views;

namespace CubeQuest.Battle
{
    public class EnemyAttack : IQueueAction
    {
        public delegate void PlayerAttackAnimationEvent();

        public event BattleHandler.PlayerAttackAnimationEvent PlayerAttackAnimation;

        public EnemyAttack(Android.Resource.Animation animation, View view)
        {
            
        }

        public event EventHandler OnEnd;
        public void Execute()
        {
            PlayerAttackAnimation?.Invoke();
        }
    }
}