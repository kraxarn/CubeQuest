using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CubeQuest.Account;

namespace CubeQuest.Battle
{
    public class BattleHandler
    {
        private ImageButton[] enemies;
        private ProgressBar[] enemyHealthBars;
        private ProgressBar playerHealthBar;
        private BattleQueue battleQueue;
        
        public enum EActionType { Attack, Spare }
        public enum EAnimationTarget { Player, Enemy }
        public enum EAnimationType { Attack, Damage }

        private bool EnemiesAreDead
        {
            get
            {
                return !enemyHealthBars.Any(enemy => enemy.Progress > 0);
            }
        }

        public delegate void BattleEndEvent(bool playerWon);

        public event BattleEndEvent BattleEnd;

        public delegate void OnAnimationEvent(EAnimationTarget target, EAnimationType type);

        public event OnAnimationEvent OnAnimation;


        public BattleHandler(ImageButton[] enemies, ProgressBar[] enemyHealthBars, ProgressBar playerHealthBar)
        {
            this.enemies = enemies;
            this.enemyHealthBars = enemyHealthBars;
            this.playerHealthBar = playerHealthBar;

            battleQueue = new BattleQueue();
            

            battleQueue.OnQueueEnd += () =>
            {
                foreach (var enemy in enemies)
                {
                    enemy.Enabled = true;
                    
                }
            };
        }

        

        

        public void StartAction(int index, EActionType action)
        {
            var action1 = new QueueAction(() =>
            {
                PlayerAttack(AccountManager.CurrentUser.Attack, index);
                Thread.Sleep(1000);
            }, true);

            battleQueue.Add(action1);

            battleQueue.Add(new QueueAction(() =>
            {
                EnemyAttack(10);
                Thread.Sleep(600);
            }, true));

            battleQueue.Execute();
           
        }

        private void PlayerAttack(int damage, int index)
        {
            enemyHealthBars[index].Progress -= damage;

            OnAnimation?.Invoke(EAnimationTarget.Player, EAnimationType.Attack);

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
            OnAnimation?.Invoke(EAnimationTarget.Enemy, EAnimationType.Attack);

            //if (!AccountManager.CurrentUser.ShouldHit)
            //    return;

            damage = AccountManager.CurrentUser.GetDamage(damage) + 50;

            AccountManager.CurrentUser.Health -= damage;

            if (AccountManager.CurrentUser.Health <= 0)
            {
                BattleEnd?.Invoke(false);
            }

        }


    }

}