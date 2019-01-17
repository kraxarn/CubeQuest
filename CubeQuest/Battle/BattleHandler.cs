using Android.Widget;
using CubeQuest.Account;
using System.Linq;
using System.Threading;
using CubeQuest.Account.Interface;

namespace CubeQuest.Battle
{
    public class BattleHandler
    {
        private readonly ImageButton[] enemies;

        private readonly ProgressBar[] enemyHealthBars;

        private ProgressBar playerHealthBar;

        private readonly BattleQueue battleQueue;

        private IEnemy enemy;
        
        public enum EActionType { Attack, Spare }

        public enum EAnimationTarget { Player, Enemy }



        private bool EnemiesAreDead => 
            !enemyHealthBars.Any(enemy => enemy.Progress > 0);

        public delegate void BattleEndEvent(BattleCore.EBattleEndType type);

        public event BattleEndEvent BattleEnd;

        public delegate void OnAnimationEvent(EAnimationTarget target, int index);

        public event OnAnimationEvent OnAnimation;

        public delegate void OnEnemyKilledEvent(int index);

        public event OnEnemyKilledEvent OnEnemyKilled;

        public void RunAway() => 
            BattleEnd?.Invoke(BattleCore.EBattleEndType.Ran);

        public BattleHandler(ImageButton[] enemies, ProgressBar[] enemyHealthBars, ProgressBar playerHealthBar, IEnemy enemyInfo)
        {
            this.enemies = enemies;
            this.enemyHealthBars = enemyHealthBars;
            this.playerHealthBar = playerHealthBar;
            this.enemy = enemyInfo;

            battleQueue = new BattleQueue();

            battleQueue.OnQueueEnd += () =>
            {
                foreach (var enemy in enemies)
                    enemy.Enabled = true;
            };
        }
        
        public void StartAction(int index, EActionType action)
        {
            if (action == EActionType.Spare)
            {
                battleQueue.Add(new QueueAction(() =>
                {
                    EnemyAttack(index);
                    Thread.Sleep(600);
                }, true));

                battleQueue.Execute();

                return;
            }

            var action1 = new QueueAction(() =>
            {
                PlayerAttack(index);
                Thread.Sleep(1000);
            }, true);

            battleQueue.Add(action1);

            battleQueue.Add(new QueueAction(() =>
            {
                if (enemyHealthBars[index].Progress <= 0)
                    return;

                EnemyAttack(index);
                Thread.Sleep(600);
            }, true));

            battleQueue.Execute();
            
        }

        private void PlayerAttack(int index)
        {
            
            enemyHealthBars[index].Progress -= (int)((AccountManager.CurrentUser.Attack / (float)enemy.Health)*100);

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

        private void EnemyAttack(int index)
        {
            OnAnimation?.Invoke(EAnimationTarget.Enemy, index);

            //if (!AccountManager.CurrentUser.ShouldHit)
            //    return;
            var damage = enemy.Attack;
            damage = AccountManager.CurrentUser.GetDamage(damage);

            AccountManager.CurrentUser.Health -= damage;

            if (AccountManager.CurrentUser.Health <= 0)
                BattleEnd?.Invoke(BattleCore.EBattleEndType.Lost);
        }
    }
}