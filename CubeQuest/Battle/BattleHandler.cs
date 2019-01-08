using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly BattleQueue battleQueue;
        
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

        public delegate void BattleEndEvent(BattleCore.EBattleEndType type);

        public event BattleEndEvent BattleEnd;

        public delegate void OnAnimationEvent(EAnimationTarget target, int index);

        public event OnAnimationEvent OnAnimation;

        public delegate void OnEnemyKilledEvent(int index);

        public event OnEnemyKilledEvent OnEnemyKilled;

        public void RanAway()
        {
            this.BattleEnd?.Invoke(BattleCore.EBattleEndType.Ran);
        }


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
            if (action == EActionType.Spare)
            {
                battleQueue.Add(new QueueAction(() =>
                {
                    EnemyAttack(10, index);
                    Thread.Sleep(600);
                }, true));

                battleQueue.Execute();

                return;
            }

            var action1 = new QueueAction(() =>
            {
                PlayerAttack(AccountManager.CurrentUser.Attack, index);
                Thread.Sleep(1000);
            }, true);

            battleQueue.Add(action1);

            battleQueue.Add(new QueueAction(() =>
            {
                EnemyAttack(10, index);
                Thread.Sleep(600);
            }, true));

            battleQueue.Execute();
            
        }

        private void PlayerAttack(int damage, int index)
        {
            enemyHealthBars[index].Progress -= damage + 50;

            OnAnimation?.Invoke(EAnimationTarget.Player, index);

            if (enemyHealthBars[index].Progress <= 0)
            {
                KillEnemy(index);
                OnEnemyKilled?.Invoke(index);
            }
            
            if (EnemiesAreDead)
                BattleEnd?.Invoke(BattleCore.EBattleEndType.Won);
        }

        private void KillEnemy(int index)
        {
            enemies[index].Drawable.SetAlpha(127);
            enemies[index].Enabled = false;
        }

        private void EnemyAttack(int damage, int index)
        {
            OnAnimation?.Invoke(EAnimationTarget.Enemy, index);

            //if (!AccountManager.CurrentUser.ShouldHit)
            //    return;

            damage = AccountManager.CurrentUser.GetDamage(damage);

            AccountManager.CurrentUser.Health -= damage;

            if (AccountManager.CurrentUser.Health <= 0)
            {
                BattleEnd?.Invoke(BattleCore.EBattleEndType.Lost);
            }
        }


    }

}