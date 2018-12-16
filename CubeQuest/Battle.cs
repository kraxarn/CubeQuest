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

            foreach (var e in enemies)
                view.FindViewById<ImageButton>(e).SetImageBitmap(enemy);

            SetAnimation(context, view.FindViewById<ImageButton>(enemies[0]));

            /*
            var image = view.FindViewById<ImageView>(Resource.Id.image_battle_enemy0);

            var anim = new ImageAnimator(image, new []
            {
                BitmapFactory.DecodeStream(assets.Open("animations/selected/0.webp")),
                BitmapFactory.DecodeStream(assets.Open("animations/selected/1.webp"))
            }, 400);

            view.FindViewById<Button>(Resource.Id.button_battle_run).Click += (sender, args) =>
            {
                anim.Stop();
                End?.Invoke();
            };
            */
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