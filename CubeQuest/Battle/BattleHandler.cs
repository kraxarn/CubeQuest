using System;
using System.Linq;
using Android.Views.Animations;
using Android.Widget;
using CubeQuest.Account;

namespace CubeQuest.Battle
{
    public  class BattleHandler
    {
        private  ImageButton[] enemies;
        private  ProgressBar[] enemyHealthBars;
        private  ProgressBar playerHealthBar;

        

        public enum EActionType { Attack, Spare }

        public void StartAction(int index, EActionType action)
        {
            //PlayerAttack(AccountManager.CurrentUser.Attack, index);

            EnemyAttack(10);

        }
        

        private bool EnemiesAreDead
        {
            get
            {
                return !enemyHealthBars.Any(enemy => enemy.Progress > 0);
            }
        }

        public delegate void BattleEndEvent(bool playerWon);

        public event BattleEndEvent BattleEnd;

        public delegate void PlayerAttackAnimationEvent();

        public event PlayerAttackAnimationEvent PlayerAttackAnimation;

        public delegate void EnemyAttackAnimationEvent();

        public event EnemyAttackAnimationEvent EnemyAttackAnimation;

        public BattleHandler(ImageButton[] enemies, ProgressBar[] enemyHealthBars, ProgressBar playerHealthBar)
        {
            this.enemies = enemies;
            this.enemyHealthBars = enemyHealthBars;
            this.playerHealthBar = playerHealthBar;
        }

        private void PlayerAttack(int damage, int index)
        {
            enemyHealthBars[index].Progress -= damage;

            PlayerAttackAnimation?.Invoke();

            if (enemyHealthBars[index].Progress <= 0)
                KillEnemy(index);

            if (EnemiesAreDead)
                BattleEnd?.Invoke(true);
        }

        private void KillEnemy(int index)
        {
            enemies[index].Drawable.SetAlpha(127);
            enemies[index].Enabled = false;
        }

        private void EnemyAttack(int damage)
        {
            AccountManager.CurrentUser.Health -= damage;

            var health = AccountManager.CurrentUser.Health;

            EnemyAttackAnimation?.Invoke();

            playerHealthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

            var healthProcent = AccountManager.CurrentUser.HealthPercentage;

            if (AccountManager.CurrentUser.Health <= 0)
                BattleEnd?.Invoke(false);
        }

        private void AnimateAttackPlayer(ImageView player, Animation animation)
        {
            player.StartAnimation(animation);
        }

        private void AnimateAttackEnemy(ImageButton enemy, Animation animation)
        {
            enemy.StartAnimation(animation);
        }

        private static void AnimateTakingDamagePlayer(ImageView player, Animation animation)
        {
            player.StartAnimation(animation);
        }

        private static void AnimateTakingDamageEnemy(ImageButton enemy, Animation animation)
        {
            enemy.StartAnimation(animation);
        }

    }

}