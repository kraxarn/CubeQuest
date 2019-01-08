using System;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using CubeQuest.Account;
using CubeQuest.Account.Interface;
using System.Collections.Generic;
using Android.Graphics.Drawables;

namespace CubeQuest.Handler
{
    public static class AssetLoader
    {
        /// <summary>
        /// Width of device
        /// </summary>
        private static int deviceWidth;

        private static AssetManager assets;

        /// <summary>
        /// Size of bitmaps
        /// </summary>
        private static int BitmapSize => (int) (deviceWidth / 5.625);

        /// <summary>
        /// Cache with loaded bitmaps
        /// </summary>
        private static Dictionary<string, Bitmap> bitmaps;
        
        /// <summary>
        /// Create asset loader
        /// </summary>
        public static void Create(int displayWidth, AssetManager assetManager)
        {
            deviceWidth = displayWidth;
            assets = assetManager;
            bitmaps = new Dictionary<string, Bitmap>();
        }
        
        /// <summary>
        /// Get enemy bitmap from cache or assets
        /// </summary>
        public static Bitmap GetEnemyBitmap(IEnemy enemy) => 
            GetBitmapFromPath($"enemy/{enemy.Icon}.webp");
        
        public static Bitmap GetCompanionBitmap(ICompanion companion) =>
            GetBitmapFromPath($"companion/{companion.Icon}.webp", 0.5f);

        [Obsolete("GetAnimationDrawable")]
        public static Bitmap[] GetAnimationBitmaps(string name, int frames)
        {
            var b = new Bitmap[frames];

            for (var i = 0; i < frames; i++)
                b[i] = GetBitmapFromPath($"animations/{name}/{i}.webp");

            return b;
        }

        private static Bitmap GetBitmapFromPath(string path, float sizeModifier = 1f)
        {
            if (bitmaps.ContainsKey(path))
                return bitmaps[path];

            var bitmap = ScaleBitmap(DecodeBitmap(path), (int) (BitmapSize * sizeModifier));
            bitmaps.Add(path, bitmap);
            return bitmap;
        }

        private static Bitmap DecodeBitmap(string path) =>
            Build.VERSION.SdkInt >= BuildVersionCodes.P 
                ? ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(assets, path)) 
                : BitmapFactory.DecodeStream(assets.Open(path));

        private static Bitmap ScaleBitmap(Bitmap bitmap, int size) => 
            Bitmap.CreateScaledBitmap(bitmap, size, size, false);
    }
}