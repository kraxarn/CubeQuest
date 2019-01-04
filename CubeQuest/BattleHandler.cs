using System.CodeDom.Compiler;
using System.Linq;
using Android.Accounts;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AccountManager = CubeQuest.Account.AccountManager;

namespace CubeQuest
{
    public  class BattleHandler
    {
        private readonly ImageButton[] enemies;
        private readonly ProgressBar[] enemyHealthBars;
        private readonly ProgressBar playerHealthBar;

        public enum EActionType { Attack, Spare }

        private void StartAction(int index, EActionType action)
        {
            PlayerAttack(AccountManager.CurrentUser.Attack, index);
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

        public BattleHandler(ImageButton[] enemies, ProgressBar[] enemyHealthBars, ProgressBar playerHealthBar)
        {
            this.enemies = enemies;
            this.enemyHealthBars = enemyHealthBars;
            this.playerHealthBar = playerHealthBar;
        }

        private void PlayerAttack(int damage, int index)
        {
            enemyHealthBars[index].Progress -= damage;

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

            playerHealthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

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