using System;

namespace CubeQuest.Battle
{
    public class PlayerAttack : IQueueAction
    {
        public delegate void PlayerAttackAnimationEvent();

        public event BattleHandler.PlayerAttackAnimationEvent PlayerAttackAnimation;

        public PlayerAttack(Android.Resource.Animation animation)
        {
        }

        public event EventHandler OnEnd;
        public void Execute()
        {
            PlayerAttackAnimation?.Invoke();
        }
    }
}