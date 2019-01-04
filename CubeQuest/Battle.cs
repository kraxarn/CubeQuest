using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CubeQuest.Account.Interface;
using System.Collections.Generic;
using System.Linq;
using CubeQuest.Handler;
using CubeQuest.Layout;

namespace CubeQuest
{
    public class Battle
    {
        /// <summary>
        /// Event for when battle ends, used by <see cref="End"/>
        /// </summary>
        public delegate void EndEvent();

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

        public Battle(Context context, View view, AssetManager assets, IItem item)
        {
            // Start battle music
            MusicManager.Play(MusicManager.EMusicTrack.Battle);

            BattleHandler.Battle = this;

            // Set context
            this.context = context;

            // Set view
            mainView = view;

            // Companions
            var companions = new List<ImageView>();

            companions.Add(mainView.FindViewById<ImageView>(Resource.Id.image_battle_companion_0));

            // Flashing health bar animation
            flashAnimation = AnimationUtils.LoadAnimation(context, Resource.Animation.flash);

            // Load enemy sprite(s)
            var enemySprite = BitmapFactory.DecodeStream(assets.Open($"enemy/{item.Icon}.webp"));

            // Enemy image buttons
            var enemyButtons = EnemyButtons.ToArray();
            
            // Enemy health bars
            enemyHealthBars = EnemyHealthBars.ToArray();

            playerHealthBar = view.FindViewById<ProgressBar>(Resource.Id.progress_battle_health);

            // Set bitmaps of enemy buttons
            enemyButtons.SetBitmaps(enemySprite);

            // Start shake animation when pressing on the first enemy
            CreateTestShakeAnimation(enemyButtons[0]);

            // Enemy overlays
            enemyOverlays = EnemyImages;

            // Set first enemy as default selected
            SelectedEnemyIndex = 0;
            
            // Load 'selected enemy' frames
            var selectedFrames = LoadAnimation(assets, "selected", 2);

			// Load slash frames when attacking enemy
            var slashFrames = LoadAnimation(assets, "slash", 5);

            // Load image animators
            var anims = GetImageAnimators(selectedFrames);

			// When clicking 'attack'
			view.FindViewById<Button>(Resource.Id.button_battle_attack).Click += (sender, args) =>
			{
				anims[SelectedEnemyIndex].New(slashFrames, 75);

				void SwitchBack()
				{
					anims[SelectedEnemyIndex].New(selectedFrames, 400);
					anims[SelectedEnemyIndex].Done -= SwitchBack;
				}

				anims[SelectedEnemyIndex].Done += SwitchBack;

                companions[0].StartAnimation(AnimationUtils.LoadAnimation(context, Resource.Animation.attack));
                BattleHandler.PlayerAttack(5);
                enemyButtons[selectedEnemyIndex]
                    .StartAnimation(AnimationUtils.LoadAnimation(context, Resource.Animation.attack));
            };

            // When clicking 'run'
            view.FindViewById<Button>(Resource.Id.button_battle_run).Click += (sender, args) =>
            {
                // Unload all animations
                foreach (var anim in anims)
                    anim.Stop();

                // Invoke end event
                End?.Invoke();
            };

            // Create events when clicking on enemies
            CreateEnemyEvents(enemyButtons);

			// Replace cube placeholders with test images
			view.FindViewById<ImageView>(Resource.Id.image_battle_companion_0).SetImageBitmap(BitmapFactory.DecodeStream(assets.Open("companion/bear.webp")));
			view.FindViewById<ImageView>(Resource.Id.image_battle_companion_1).SetImageBitmap(BitmapFactory.DecodeStream(assets.Open("companion/buffalo.webp")));
			view.FindViewById<ImageView>(Resource.Id.image_battle_companion_2).SetImageBitmap(BitmapFactory.DecodeStream(assets.Open("companion/chick.webp")));
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
                Resource.Id.button_battle_enemy2,
                Resource.Id.button_battle_enemy3,
                Resource.Id.button_battle_enemy4
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
                Resource.Id.battle_health_enemy2,
                Resource.Id.battle_health_enemy3,
                Resource.Id.battle_health_enemy4,

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
            mainView.FindViewById<ImageView>(Resource.Id.image_battle_enemy2),
            mainView.FindViewById<ImageView>(Resource.Id.image_battle_enemy3),
            mainView.FindViewById<ImageView>(Resource.Id.image_battle_enemy4)
        };
        
        /// <summary>
        /// Get image animators for each overlay image
        /// </summary>
        private ImageAnimator[] GetImageAnimators(IReadOnlyList<Bitmap> frames) =>
            new[]
            {
                new ImageAnimator(enemyOverlays[0], frames, 400),
                new ImageAnimator(enemyOverlays[1], frames, 400),
                new ImageAnimator(enemyOverlays[2], frames, 400),
                new ImageAnimator(enemyOverlays[3], frames, 400),
                new ImageAnimator(enemyOverlays[4], frames, 400)
            };
        
        /// <summary>
        /// Set <see cref="SelectedEnemyIndex"/> depending on what enemy is pressed
        /// </summary>
        private void CreateEnemyEvents(IReadOnlyList<ImageButton> imageButtons)
        {
            imageButtons[0].Click += (sender, args) => SelectedEnemyIndex = 0;
            imageButtons[1].Click += (sender, args) => SelectedEnemyIndex = 1;
            imageButtons[2].Click += (sender, args) => SelectedEnemyIndex = 2;
            imageButtons[3].Click += (sender, args) => SelectedEnemyIndex = 3;
            imageButtons[4].Click += (sender, args) => SelectedEnemyIndex = 4;
        }



        /// <summary>
        /// Loads animation from assets folder
        /// </summary>
        private static Bitmap[] LoadAnimation(AssetManager assets, string name, int frames)
        {
	        var bitmaps = new Bitmap[frames];

	        for (var i = 0; i < frames; i++)
		        bitmaps[i] = BitmapFactory.DecodeStream(assets.Open($"animations/{name}/{i}.webp"));

	        return bitmaps;
        }

        // FUNCTIONS FOR THE BATTLE HANDLER

        public void EnemyLoseLife(int damage)
        {
            enemyHealthBars[selectedEnemyIndex].Progress -= damage;
        }

        public void PlayerLoseLife(int damage)
        {
            playerHealthBar.Progress -= damage;
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