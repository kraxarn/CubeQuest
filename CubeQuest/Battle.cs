using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using CubeQuest.Account.Interface;

namespace CubeQuest
{
    public class Battle
    {
        public delegate void EndEvent();

        public event EndEvent End;

        public Battle(View view, AssetManager assets, IItem item)
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

            view.FindViewById<Button>(Resource.Id.button_battle_run).Click += (sender, args) => End?.Invoke();
        }
    }
}