using Android.Accounts;
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

        public BattleHandler(ImageButton[] enemies, ProgressBar[] enemyHealthBars, ProgressBar playerHealthBar)
        {
            this.enemies = enemies;
            this.enemyHealthBars = enemyHealthBars;
            this.playerHealthBar = playerHealthBar;
        }

        public void PlayerAttack(int damage, int index)
        {
            enemyHealthBars[index].Progress -= damage;

            if (enemyHealthBars[index].Progress <= 0)
            {
                // TOTO STUFF THEN ENEMY DIES
            }
        }

        public void EnemyAttack(int damage)
        {
            AccountManager.CurrentUser.Health -= damage;

            playerHealthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

            if (AccountManager.CurrentUser.Health <= 0)
            {
                // TODO STUFF WHEN PLAYER DIES
            }
        }

        public void AnimateAttackPlayer(ImageView player, Animation animation)
        {
            player.StartAnimation(animation);
        }

        public void AnimateAttackEnemy(ImageButton enemy, Animation animation)
        {
            enemy.StartAnimation(animation);
        }

        public static void AnimateTakingDamagePlayer(ImageView player, Animation animation)
        {
            player.StartAnimation(animation);
        }

        public static void AnimateTakingDamageEnemy(ImageButton enemy, Animation animation)
        {
            enemy.StartAnimation(animation);
        }

        public static void AnimationController()
        {
            bool keepLooping = true;

            while (keepLooping)
            {

            }
        }








    }

}