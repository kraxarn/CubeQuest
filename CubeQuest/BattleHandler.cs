using Android.Views.Animations;
using Android.Widget;

namespace CubeQuest
{
    public static class BattleHandler
    {

        public static Battle Battle;


        public static void PlayerAttack(int damage)
        {
            Battle.EnemyLoseLife(5);
        }

        public static void EnemyAttack(int damage)
        {
            Battle.PlayerLoseLife(damage);
        }

        public static void AnimateAttackPlayer(ImageView player, Animation animation)
        {
            player.StartAnimation(animation);
        }

        public static void AnimateAttackEnemy(ImageButton enemy, Animation animation)
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








    }

}