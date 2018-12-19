using System.Collections.Generic;
using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Games.MultiPlayer.RealTime;
using Android.Graphics;
using Android.Views;
using Android.Views.Accessibility;
using Android.Views.Animations;
using Android.Widget;
using CubeQuest.Account.Interface;

namespace CubeQuest
{
    public class Battle
    {
        public delegate void EndEvent();

        public event EndEvent End;

        private Android.Resource.Animation animateShake;

        private int selectedIndex;

        private ImageView[] images;

        private ProgressBar[] enemyHealthBars;

        public Battle(Context context, View view, AssetManager assets, IItem item)
        {

            var enemy = BitmapFactory.DecodeStream(assets.Open($"enemy/{item.Icon}.webp"));

            var progressBar = GetEnemyProgressbarResourceArray();         
            var enemies = GetEnemyButtonResourceArray();
            var imageButtons = GetEnemyButtonsArray(view, enemies);

            SetEnemyProgressbarArray(view, progressBar);
            SetUpBitmapsForEnemies(view, enemies, enemy);
            SetShakeAnimation(context, imageButtons[0]);
            SetEnemyImageViewsArray(view);

            InitAnimations(context);

            var frames = new[]
            {
                BitmapFactory.DecodeStream(assets.Open("animations/selected/0.webp")),
                BitmapFactory.DecodeStream(assets.Open("animations/selected/1.webp"))
            };

            var anims = GetImageAnimatorArray(frames);

            view.FindViewById<Button>(Resource.Id.button_battle_run).Click += (sender, args) =>
            {
                foreach (var anim in anims)
                    anim.Stop();

                End?.Invoke();
            };

            SetAnimationClickEvents(imageButtons, context);



        }

        private void SetHighlightedEnemy(int index)
        {
            foreach (var image in images)
            {
                image.Visibility = ViewStates.Invisible;
            }

            images[index].Visibility = ViewStates.Visible;

            selectedIndex = index;
        }



        private void SetShakeAnimation(Context context, ImageButton button)
        {
            var animShake = AnimationUtils.LoadAnimation(context, Resource.Animation.shake);

            button.Click += (sender, args) =>
            {
                button.StartAnimation(animShake);
            };
        }

        private void SetFlashingAnimation(Context context, int index)
        {
            var animFlash = AnimationUtils.LoadAnimation(context, Resource.Animation.flash);

            foreach (var enemy in enemyHealthBars)
            {
                enemy.ClearAnimation();
            }

            enemyHealthBars[index].StartAnimation(animFlash);
        }

        private void InitAnimations(Context context)
        {

            var animFlash = AnimationUtils.LoadAnimation(context, Resource.Animation.flash);
            foreach (var image in images)
                image.Visibility = ViewStates.Invisible;

            images[0].Visibility = ViewStates.Visible;

            foreach (var enemy in enemyHealthBars)
            {
                enemy.ClearAnimation();
            }

            enemyHealthBars[0].StartAnimation(animFlash);
        }

        private int[] GetEnemyButtonResourceArray()
        {
            var enemies = new[]
            {
                Resource.Id.button_battle_enemy0,
                Resource.Id.button_battle_enemy1,
                Resource.Id.button_battle_enemy2,
                Resource.Id.button_battle_enemy3,
                Resource.Id.button_battle_enemy4
            };

            return enemies;
        }

        private int[] GetEnemyProgressbarResourceArray()
        {
            var progressBar = new[]
            {
                Resource.Id.battle_health_enemy0,
                Resource.Id.battle_health_enemy1,
                Resource.Id.battle_health_enemy2,
                Resource.Id.battle_health_enemy3,
                Resource.Id.battle_health_enemy4,

            };

            return progressBar;
        }

        private ImageButton[] GetEnemyButtonsArray(View view, IReadOnlyList<int> enemies)
        {
            var imageButtons = new[]
            {
                view.FindViewById<ImageButton>(enemies[0]),
                view.FindViewById<ImageButton>(enemies[1]),
                view.FindViewById<ImageButton>(enemies[2]),
                view.FindViewById<ImageButton>(enemies[3]),
                view.FindViewById<ImageButton>(enemies[4])
            };

            return imageButtons;
        }

        private void SetEnemyProgressbarArray(View view, IReadOnlyList<int> progressBar)
        {
            enemyHealthBars = new[]
            {
                view.FindViewById<ProgressBar>(progressBar[0]),
                view.FindViewById<ProgressBar>(progressBar[1]),
                view.FindViewById<ProgressBar>(progressBar[2]),
                view.FindViewById<ProgressBar>(progressBar[3]),
                view.FindViewById<ProgressBar>(progressBar[4]),
            };
        }

        private void SetUpBitmapsForEnemies(View view, IReadOnlyList<int> enemies, Bitmap enemy)
        {
            foreach (var e in enemies)
                view.FindViewById<ImageButton>(e).SetImageBitmap(enemy);
        }

        private void SetEnemyImageViewsArray(View view)
        {
            images = new[]
            {
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy0),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy1),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy2),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy3),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy4)
            };
            
        }

        private ImageAnimator[] GetImageAnimatorArray(Bitmap[] frames)
        {
            var anims = new[]
            {
                new ImageAnimator(images[0], frames, 400),
                new ImageAnimator(images[1], frames, 400),
                new ImageAnimator(images[2], frames, 400),
                new ImageAnimator(images[3], frames, 400),
                new ImageAnimator(images[4], frames, 400)
            };

            return anims;
        }

        private void SetAnimationClickEvents(ImageButton[] imageButtons, Context context)
        {
            imageButtons[0].Click += (sender, args) => SetHighlightedEnemy(0);
            imageButtons[1].Click += (sender, args) => SetHighlightedEnemy(1);
            imageButtons[2].Click += (sender, args) => SetHighlightedEnemy(2);
            imageButtons[3].Click += (sender, args) => SetHighlightedEnemy(3);
            imageButtons[4].Click += (sender, args) => SetHighlightedEnemy(4);

            imageButtons[0].Click += (sender, args) => SetFlashingAnimation(context, 0);
            imageButtons[1].Click += (sender, args) => SetFlashingAnimation(context, 1);
            imageButtons[2].Click += (sender, args) => SetFlashingAnimation(context, 2);
            imageButtons[3].Click += (sender, args) => SetFlashingAnimation(context, 3);
            imageButtons[4].Click += (sender, args) => SetFlashingAnimation(context, 4);
        }
    }
}


//var progressBar = new[]
//{
//    Resource.Id.battle_health_enemy0,
//    Resource.Id.battle_health_enemy1,
//    Resource.Id.battle_health_enemy2,
//    Resource.Id.battle_health_enemy3,
//    Resource.Id.battle_health_enemy4,

//};

//var enemies = new[]
//{
//    Resource.Id.button_battle_enemy0,
//    Resource.Id.button_battle_enemy1,
//    Resource.Id.button_battle_enemy2,
//    Resource.Id.button_battle_enemy3,
//    Resource.Id.button_battle_enemy4
//};

//var imageButtons = new[]
//{
//    view.FindViewById<ImageButton>(enemies[0]),
//    view.FindViewById<ImageButton>(enemies[1]),
//    view.FindViewById<ImageButton>(enemies[2]),
//    view.FindViewById<ImageButton>(enemies[3]),
//    view.FindViewById<ImageButton>(enemies[4])
//};
//images = new[]
//{
//    view.FindViewById<ImageView>(Resource.Id.image_battle_enemy0),
//    view.FindViewById<ImageView>(Resource.Id.image_battle_enemy1),
//    view.FindViewById<ImageView>(Resource.Id.image_battle_enemy2),
//    view.FindViewById<ImageView>(Resource.Id.image_battle_enemy3),
//    view.FindViewById<ImageView>(Resource.Id.image_battle_enemy4)
//};
