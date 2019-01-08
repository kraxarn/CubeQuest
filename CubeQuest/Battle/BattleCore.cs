using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CubeQuest.Account;
using CubeQuest.Account.Interface;
using CubeQuest.Handler;
using CubeQuest.Layout;

namespace CubeQuest.Battle
{
    public class BattleCore
    {
        /// <summary>
        /// Event for when battle ends, used by <see cref="End"/>
        /// </summary>
        public delegate void EndEvent(EBattleEndType battleEndType);

        public enum EBattleEndType
        {
            Won,
            Lost,
            Ran
        };

        /// <summary>
        /// When the battle ends
        /// </summary>
        public event EndEvent End;

        /// <summary>
        /// Current selected index, you probably want to use <see cref="SelectedEnemyIndex"/>
        /// </summary>
        private int selectedEnemyIndex;

        /// <summary>
        /// Overlay used for animations for each enemy
        /// </summary>
        private readonly ImageView[] enemyOverlays;

        /// <summary>
        /// Health bar for each enemy
        /// </summary>
        private readonly ProgressBar[] enemyHealthBars;

        private readonly ProgressBar playerHealthBar;

        /// <summary>
        /// Context, probably <see cref="GameActivity"/>
        /// </summary>
        private readonly Context context;

        /// <summary>
        /// Battle view
        /// </summary>
        private readonly View mainView;

        /// <summary>
        /// Animation used for flashing health bars
        /// </summary>
        private readonly Animation flashAnimation;

        private readonly Animation shakeAnimation;

        private readonly Animation attackAnimation;

        public BattleCore(Activity context, View view, IEnemy enemy)
        {
            // Start battle music
            MusicManager.Play(MusicManager.EMusicTrack.Battle);

            // Set context
            this.context = context;

            // Set view
            mainView = view;

            // Companions
            var companions = new List<ImageView>();

            companions.Add(mainView.FindViewById<ImageView>(Resource.Id.image_battle_companion_0));
            companions.Add(mainView.FindViewById<ImageView>(Resource.Id.image_battle_companion_1));
            companions.Add(mainView.FindViewById<ImageView>(Resource.Id.image_battle_companion_2));

            // Animations
            flashAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.flash);
            attackAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.attack);
            shakeAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.shake);

            // Load enemy sprite(s)
            var enemySprite = AssetLoader.GetEnemyBitmap(enemy);

            // Enemy image buttons
            var enemyButtons = EnemyButtons.ToArray();

            // Enemy health bars
            enemyHealthBars = EnemyHealthBars.ToArray();

            // Player health bar
            playerHealthBar = view.FindViewById<ProgressBar>(Resource.Id.progress_battle_health);

            playerHealthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

            // Battle handler
            var battleHandler = new BattleHandler(enemyButtons, enemyHealthBars, playerHealthBar);

            // Set bitmaps of enemy buttons
            enemyButtons.SetBitmaps(enemySprite);

            // Start shake animation when pressing on the first enemy
            //CreateTestShakeAnimation(enemyButtons[0]);

            // Enemy overlays
            enemyOverlays = EnemyImages;

            // Set first enemy as default selected
            SelectedEnemyIndex = 0;

            ResetEnemies();

            // Load 'selected enemy' frames
			var selectedAnimations = new Drawable[enemyOverlays.Length];

			// Create 3 copies for each enemy
			for (var i = 0; i < selectedAnimations.Length; i++)
				selectedAnimations[i] = AssetLoader.GetAnimatedDrawable(context.Resources, "selected", 2, 400, true);

            // Load image animators
            EnemyOverlayDrawable = selectedAnimations;
            

            // When clicking attack
            view.FindViewById<Button>(Resource.Id.button_battle_attack).Click += OnAttackClickEvent;

            void OnAttackClickEvent(object sender, EventArgs arg)
            {
                if (enemyHealthBars[selectedEnemyIndex].Progress <= 0)
                {
                    return;
                }
                ButtonsController(mainView, false);
                battleHandler.StartAction(selectedEnemyIndex, BattleHandler.EActionType.Attack);
            }

            battleHandler.BattleEnd += won =>
            {
                switch (won)
                {
                    case EBattleEndType.Won:
                        End?.Invoke(EBattleEndType.Won);
                        break;
                    case EBattleEndType.Lost:
                        End?.Invoke(EBattleEndType.Lost);
                        break;
                    case EBattleEndType.Ran:
                        End?.Invoke(EBattleEndType.Ran);
                        break;
                };
                view.FindViewById<Button>(Resource.Id.button_battle_attack).Click -= OnAttackClickEvent;
                view.FindViewById<Button>(Resource.Id.button_battle_run).Click -= OnRunClickEvent;
                view.FindViewById<Button>(Resource.Id.button_battle_spare).Click -= OnSpareClickEvent;
            };
            
            // Player attacks animation in battle
            battleHandler.OnAnimation += (target, index) =>
            {

                foreach (var enemyButton in enemyButtons)
                    enemyButton.Enabled = false;

                switch (target)
                {
                    case BattleHandler.EAnimationTarget.Player:
                        companions[index].StartAnimation(attackAnimation);
                        enemyButtons[index].StartAnimation(shakeAnimation);
                        break;

                    case BattleHandler.EAnimationTarget.Enemy:
                        enemyButtons[index].StartAnimation(attackAnimation);
                        companions[index].StartAnimation(shakeAnimation);
                        companions[index].Animation.AnimationEnd += (sender, args) => ButtonsController(mainView, true);
                        break;
                }

            };

            // When clicking 'run'
            view.FindViewById<Button>(Resource.Id.button_battle_run).Click += OnRunClickEvent;

            void OnRunClickEvent(object sender, EventArgs arg)
            {
                // Invoke end event
                //End?.Invoke(EBattleEndType.Ran);

                battleHandler.RanAway();
            }

            // When clicking 'win'
            view.FindViewById<Button>(Resource.Id.button_battle_spare).Click += OnSpareClickEvent;

            void OnSpareClickEvent(object sender, EventArgs arg)
            {
                var enemyTotalHealth = enemyHealthBars.Average(e => e.Progress);
                var enemyLevel = enemy.Level;
                var playerLevel = AccountManager.CurrentUser.Level;
                var success = playerLevel > enemyLevel ? 0.2f : -0.2f;

                success += enemyTotalHealth < 0.25f ? 0.8f : enemyTotalHealth < 0.5f ? 0.4f : 0f;

                if (success >= new Random().NextDouble())
                {
                    End?.Invoke(EBattleEndType.Won);
                }
                else
                {
                    ButtonsController(mainView, false);
                    battleHandler.StartAction(selectedEnemyIndex, BattleHandler.EActionType.Spare);
                }
                
            }

            // Create events when clicking on enemies
            CreateEnemyEvents(enemyButtons);

            // When a enemy dies the button can no longer be clicked.
            battleHandler.OnEnemyKilled += index => enemyButtons[index].Enabled = enemyButtons[index].Clickable = false;

            // Set companion images
            view.FindViewById<ImageView>(Resource.Id.image_battle_companion_0).SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[0]));
            view.FindViewById<ImageView>(Resource.Id.image_battle_companion_1).SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[1]));
            view.FindViewById<ImageView>(Resource.Id.image_battle_companion_2).SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[2]));

            AccountManager.CurrentUser.OnHealthChange += health =>
                playerHealthBar.Progress = AccountManager.CurrentUser.HealthPercentage;
        }

        private void ButtonsController(View view, bool turnOn)
        {
            view.FindViewById<Button>(Resource.Id.button_battle_attack).Enabled = turnOn;
            view.FindViewById<Button>(Resource.Id.button_battle_run).Enabled = turnOn;
            view.FindViewById<Button>(Resource.Id.button_battle_spare).Enabled = turnOn;
            foreach (var enemy in EnemyButtons)
            {
                enemy.Enabled = turnOn;
            }

        }

        private void ResetEnemies()
        {
            foreach (var enemy in enemyHealthBars)
            {
                enemy.Progress = 100;
            }

            foreach (var enemy in EnemyButtons)
            {
                enemy.Clickable = enemy.Enabled = true;
            }
        }

        async void StartTimer(ImageButton[] enemyButtons)
        {
            await Task.Delay(5000);
            enemyButtons[selectedEnemyIndex]
                .StartAnimation(AnimationUtils.LoadAnimation(context, Resource.Animation.attack));
        }

        private void PlayAnimation(View view)
        {
            view.StartAnimation(attackAnimation);
        }

        /// <summary>
        /// Selected enemy
        /// </summary>
        public int SelectedEnemyIndex
        {
            get => selectedEnemyIndex;

            private set
            {
                // Reset all overlay images
                foreach (var image in enemyOverlays)
                    image.Visibility = ViewStates.Invisible;

                // Reset all health bar animations
                foreach (var enemy in enemyHealthBars)
                    enemy.ClearAnimation();

                // Show correct overlay image
                enemyOverlays[value].Visibility = ViewStates.Visible;

                // Set correct health bar animation
                enemyHealthBars[value].StartAnimation(flashAnimation);

                selectedEnemyIndex = value;
            }
        }

        /// <summary>
        /// Sets a view to use the shake animation on press, for testing only
        /// </summary>
        /// <param name="view"></param>
        private void CreateTestShakeAnimation(View view) =>
            view.Click += (sender, args) =>
                view.StartAnimation(AnimationUtils.LoadAnimation(context, Resource.Animation.shake));

        /// <summary>
        /// ID of all enemy buttons
        /// </summary>
        private static IEnumerable<int> EnemyButtonIds =>
            new[]
            {
                Resource.Id.button_battle_enemy0,
                Resource.Id.button_battle_enemy1,
                Resource.Id.button_battle_enemy2
            };

        /// <summary>
        /// All enemy image buttons
        /// </summary>
        private IEnumerable<ImageButton> EnemyButtons =>
            EnemyButtonIds.Select(enemy => mainView.FindViewById<ImageButton>(enemy));

        /// <summary>
        /// ID of all enemy health bars
        /// </summary>
        private static IEnumerable<int> EnemyHealthBarIds =>
            new[]
            {
                Resource.Id.battle_health_enemy0,
                Resource.Id.battle_health_enemy1,
                Resource.Id.battle_health_enemy2
            };

        /// <summary>
        /// All enemy health bars
        /// </summary>
        private IEnumerable<ProgressBar> EnemyHealthBars =>
            EnemyHealthBarIds.Select(enemy => mainView.FindViewById<ProgressBar>(enemy));

        /// <summary>
        /// Get enemy image overlays from <see cref="mainView"/>
        /// </summary>
        private ImageView[] EnemyImages => new[]
        {
            mainView.FindViewById<ImageView>(Resource.Id.image_battle_enemy0),
            mainView.FindViewById<ImageView>(Resource.Id.image_battle_enemy1),
            mainView.FindViewById<ImageView>(Resource.Id.image_battle_enemy2)
        };

        private Drawable[] EnemyOverlayDrawable
        {
	        set
	        {
		        for (var i = 0; i < enemyOverlays.Length; i++)
			        enemyOverlays[i].SetImageDrawable(value[i]);
			}
        }

        /// <summary>
        /// Set <see cref="SelectedEnemyIndex"/> depending on what enemy is pressed
        /// </summary>
        private void CreateEnemyEvents(IReadOnlyList<ImageButton> imageButtons)
        {
            imageButtons[0].Click += (sender, args) => SelectedEnemyIndex = 0;
            imageButtons[1].Click += (sender, args) => SelectedEnemyIndex = 1;
            imageButtons[2].Click += (sender, args) => SelectedEnemyIndex = 2;
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// Set bitmaps for all specified image buttons
        /// </summary>
        public static void SetBitmaps(this IEnumerable<ImageButton> buttons, Bitmap bitmap)
        {
            foreach (var button in buttons)
                button.SetImageBitmap(bitmap);
        }
	}
}