using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
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

        private ImageView[] images;

        public Battle(Context context, View view, AssetManager assets, IItem item)
        {
            var enemy = BitmapFactory.DecodeStream(assets.Open($"enemy/{item.Icon}.webp"));
            
            var enemies = new[]
            {
                Resource.Id.button_battle_enemy0,
                Resource.Id.button_battle_enemy1,
                Resource.Id.button_battle_enemy2,
                Resource.Id.button_battle_enemy3,
                Resource.Id.button_battle_enemy4
            };

            var imageButtons = new[]
            {
                view.FindViewById<ImageButton>(enemies[0]),
                view.FindViewById<ImageButton>(enemies[1]),
                view.FindViewById<ImageButton>(enemies[2]),
                view.FindViewById<ImageButton>(enemies[3]),
                view.FindViewById<ImageButton>(enemies[4])
            };

            foreach (var e in enemies)
                view.FindViewById<ImageButton>(e).SetImageBitmap(enemy);

            SetAnimation(context, imageButtons[0]);

            images = new[]
            {
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy0),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy1),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy2),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy3),
                view.FindViewById<ImageView>(Resource.Id.image_battle_enemy4)
            };

            foreach (var image in images)
                image.Visibility = ViewStates.Invisible;

            images[0].Visibility = ViewStates.Visible;

            var frames = new[]
            {
                BitmapFactory.DecodeStream(assets.Open("animations/selected/0.webp")),
                BitmapFactory.DecodeStream(assets.Open("animations/selected/1.webp"))
            };

            var anims = new[]
            {
                new ImageAnimator(images[0], frames, 400),
                new ImageAnimator(images[1], frames, 400),
                new ImageAnimator(images[2], frames, 400),
                new ImageAnimator(images[3], frames, 400),
                new ImageAnimator(images[4], frames, 400)
            };

            view.FindViewById<Button>(Resource.Id.button_battle_run).Click += (sender, args) =>
            {
                foreach (var anim in anims)
                    anim.Stop();

                End?.Invoke();
            };

            imageButtons[0].Click += (sender, args) => SetHighlightedEnemy(0);
            imageButtons[1].Click += (sender, args) => SetHighlightedEnemy(1);
            imageButtons[2].Click += (sender, args) => SetHighlightedEnemy(2);
            imageButtons[3].Click += (sender, args) => SetHighlightedEnemy(3);
            imageButtons[4].Click += (sender, args) => SetHighlightedEnemy(4);
        }

        private void SetHighlightedEnemy(int index)
        {
            foreach (var image in images)
                image.Visibility = ViewStates.Invisible;

            images[index].Visibility = ViewStates.Visible;
        }

        private void SetAnimation(Context context, ImageButton button)
        {
            var animShake = AnimationUtils.LoadAnimation(context, Resource.Animation.shake);

            button.Click += (sender, args) =>
            {
                button.StartAnimation(animShake);
            };
        }
    }
}