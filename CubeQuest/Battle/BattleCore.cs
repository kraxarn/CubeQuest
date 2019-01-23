using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CubeQuest.Account;
using CubeQuest.Account.Interface;
using CubeQuest.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        }

        private enum EBattleAction
        {
			Attack,
			Spare,
			Run
        }

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

        /// <summary>
        /// Battle view
        /// </summary>
        private readonly View mainView;

        /// <summary>
        /// Animation used for flashing health bars
        /// </summary>
        private readonly Animation flashAnimation;
		
		/// <summary>
		/// Buttons for all companions
		/// </summary>
        private readonly ImageButton[] companionButtons;

		/// <summary>
		/// Buttons for all enemies
		/// </summary>
        private readonly ImageButton[] enemyButtons;

		/// <summary>
		/// Buttons for different actions
		/// </summary>
		private readonly Dictionary<EBattleAction, Button> actionButtons;

		private readonly Activity context;

        public BattleCore(Activity context, View view, IEnemy enemy)
        {
            // Start battle music
            MusicManager.Play(MusicManager.EMusicTrack.Battle);

            // Set view
            mainView = view;

			// Set context
            this.context = context;

            // Companions
            companionButtons = new []
            {
                mainView.FindViewById<ImageButton>(Resource.Id.image_battle_companion_0),
                mainView.FindViewById<ImageButton>(Resource.Id.image_battle_companion_1),
                mainView.FindViewById<ImageButton>(Resource.Id.image_battle_companion_2)
            };

            // Animations
            flashAnimation            = AnimationUtils.LoadAnimation(context, Resource.Animation.flash);
            var enemyAttackAnimation  = AnimationUtils.LoadAnimation(context, Resource.Animation.enemy_attack);
            var playerAttackAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.player_attack);
            var shakeAnimation        = AnimationUtils.LoadAnimation(context, Resource.Animation.shake);

            // Load enemy sprite(s)
            var enemySprite = AssetLoader.GetEnemyBitmap(enemy);

            // Enemy image buttons
            enemyButtons = EnemyButtons.ToArray();

            // Enemy health bars
            enemyHealthBars = EnemyHealthBars.ToArray();

            // Player health bar
            var playerHealthBar = view.FindViewById<ProgressBar>(Resource.Id.progress_battle_health);

            playerHealthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

            // Battle handler
            var battleHandler = new BattleHandler(enemyButtons, enemyHealthBars, enemy);

            // Set bitmaps of enemy buttons
            enemyButtons.SetBitmaps(enemySprite);

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
			enemyOverlays.SetDrawable(selectedAnimations);
            
			// Load action buttons
			actionButtons = new Dictionary<EBattleAction, Button>
			{
				{ EBattleAction.Attack, view.FindViewById<Button>(Resource.Id.button_battle_attack) },
				{ EBattleAction.Spare,  view.FindViewById<Button>(Resource.Id.button_battle_spare)  },
				{ EBattleAction.Run,    view.FindViewById<Button>(Resource.Id.button_battle_run)    }
			};

            // When clicking attack
            actionButtons[EBattleAction.Attack].Click += OnAttackClickEvent;

            void OnAttackClickEvent(object sender, EventArgs arg)
            {
                if (enemyHealthBars[selectedEnemyIndex].Progress <= 0)
	                return;

                ToggleButtons(false);
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
                }

                actionButtons[EBattleAction.Attack].Click -= OnAttackClickEvent;
                actionButtons[EBattleAction.Spare].Click  -= OnSpareClickEvent;
				actionButtons[EBattleAction.Run].Click    -= OnRunClickEvent;
            };
            
            // Player attacks animation in battle
            battleHandler.OnAnimation += (target, index) =>
            {
	            context.RunOnUiThread(() =>
	            {
		            foreach (var enemyButton in enemyButtons)
			            enemyButton.Enabled = false;
	            });

                switch (target)
                {
                    case BattleHandler.EAnimationTarget.Player:
						context.RunOnUiThread(() =>
						{
							companionButtons[index].StartAnimation(playerAttackAnimation);
							enemyButtons[index].StartAnimation(shakeAnimation);
							enemyButtons[index].Animation.AnimationEnd += OnCompanionAnimationEnd;
						});
						break;

                    case BattleHandler.EAnimationTarget.Enemy:
	                    context.RunOnUiThread(() =>
	                    {
							enemyButtons[index].StartAnimation(enemyAttackAnimation);
		                    companionButtons[index].StartAnimation(shakeAnimation);
							companionButtons[index].Animation.AnimationEnd += OnCompanionAnimationEnd;
	                    });
						break;
                }
            };

            // When clicking 'run'
            actionButtons[EBattleAction.Run].Click += OnRunClickEvent;

            void OnRunClickEvent(object sender, EventArgs arg)
            {
                AccountManager.CurrentUser.HealthPercentage -= 2;
                battleHandler.RunAway();
            }

            // When clicking 'spare'
            actionButtons[EBattleAction.Spare].Click += OnSpareClickEvent;
			
			void OnSpareClickEvent(object sender, EventArgs arg)
            {
                var enemyAverageHp = enemyHealthBars.Average(e => e.Progress);
                var enemyLevel = enemy.Level;
                var playerLevel = AccountManager.CurrentUser.Level;
                var success = playerLevel > enemyLevel ? 0.2f : -0.2f;

                success += enemyAverageHp < 25 ? 0.8f : enemyAverageHp < 50 ? 0.4f : 0f;

                if (success >= new Random().NextDouble())
	                End?.Invoke(EBattleEndType.Won);
                else
                {
					// Disable buttons
                    ToggleButtons(false);

					// Show text
                    var spareText = view.FindViewById<TextView>(Resource.Id.progress_battle_spare_message_text_view);
					spareText.Visibility = ViewStates.Visible;

					// Hide after 2 seconds
					Task.Run(() =>
					{
						Thread.Sleep(2000);
						spareText.Visibility = ViewStates.Invisible;
					});

					// Do the rest of the spare stuff (enemy attacking you)
                    battleHandler.StartAction(selectedEnemyIndex, BattleHandler.EActionType.Spare);
                }
            }

            // Create events when clicking on enemies
            CreateEnemyEvents(enemyButtons);

            // When a enemy dies the button can no longer be clicked.
            battleHandler.OnEnemyKilled += index => enemyButtons[index].Enabled = enemyButtons[index].Clickable = false;

            // Set companion images
            for (var i = 0; i < companionButtons.Length; i++)
	            companionButtons[i].SetImageBitmap(AssetLoader.GetCompanionBitmap(AccountManager.CurrentUser.EquippedCompanions[i]));

            AccountManager.CurrentUser.OnHealthChange += health =>
                playerHealthBar.Progress = AccountManager.CurrentUser.HealthPercentage;

			// Run on ui thread stuff
			battleHandler.RunOnUiThread += context.RunOnUiThread;
        }

        private void OnCompanionAnimationEnd(object sender, Animation.AnimationEndEventArgs args)
        {
	        ToggleButtons(true);
			
			companionButtons.Select(c => c.Animation).ClearAnimationEndListeners(OnCompanionAnimationEnd);
			enemyButtons.Select(c     => c.Animation).ClearAnimationEndListeners(OnCompanionAnimationEnd);
		}
		
        private void ToggleButtons(bool enable) =>
	        context.RunOnUiThread(() =>
	        {
		        foreach (var button in actionButtons)
			        button.Value.Enabled = enable;

		        foreach (var enemy in EnemyButtons)
			        enemy.Enabled = enable;
	        });

        private void ResetEnemies()
        {
            foreach (var enemy in enemyHealthBars)
                enemy.Progress = 100;

            foreach (var enemy in EnemyButtons)
                enemy.Clickable = enemy.Enabled = true;
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

    public static class BattleCoreExtensions
    {
        /// <summary>
        /// Set bitmaps for all specified images
        /// </summary>
        public static void SetBitmaps(this IEnumerable<ImageView> buttons, Bitmap bitmap)
        {
            foreach (var button in buttons)
                button.SetImageBitmap(bitmap);
        }

        /// <summary>
        /// Set drawable for all specified images
        /// </summary>
        public static void SetDrawable(this ImageView[] buttons, Drawable[] drawable)
        {
	        for (var i = 0; i < buttons.Length; i++)
		        buttons[i].SetImageDrawable(drawable[i]);
        }

		public static void ClearAnimationEndListeners(this IEnumerable<Animation> animations, EventHandler<Animation.AnimationEndEventArgs> eventHandler)
        {
	        foreach (var animation in animations)
	        {
				if (animation != null)
					animation.AnimationEnd -= eventHandler;
	        }
        }
    }
}