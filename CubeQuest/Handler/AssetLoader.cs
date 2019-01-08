using Android.Content.Res;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using CubeQuest.Account.Interface;
using System.Collections.Generic;

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

			AssetLoaderConverter.Create();
        }
        
        /// <summary>
        /// Get enemy bitmap from cache or assets
        /// </summary>
        public static Bitmap GetEnemyBitmap(IEnemy enemy) => 
            GetBitmapFromPath($"enemy/{enemy.Icon}.webp");
        
        public static Bitmap GetCompanionBitmap(ICompanion companion) =>
            GetBitmapFromPath($"companion/{companion.Icon}.webp", 0.5f);

        private static Bitmap GetAnimationFrameBitmap(string name, int frame) =>
	        GetBitmapFromPath($"animations/{name}/{frame}.webp");

        /// <summary>
        /// Create an animated bitmap
        /// </summary>
        public static Drawable GetAnimatedDrawable(Resources res, string name, int frames, int duration, bool loop)
        {
	        var animation = new AnimationDrawable();
			var bitmapFrames = new List<Bitmap>(frames);
			
			for (var i = 0; i < frames; i++)
				bitmapFrames.Add(GetAnimationFrameBitmap(name, i));

			// TODO: Load drawable better, like from cache
			bitmapFrames.ForEach(f => animation.AddFrame(new BitmapDrawable(res, f), duration));

			animation.OneShot = !loop;
			animation.Start();
			return animation;
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

	public static class AssetLoaderConverter
	{
		/// <summary>
		/// Dictionary with hash of bitmap and bitmap descriptor
		/// </summary>
		private static Dictionary<int, BitmapDescriptor> bitmapDescriptors;

		public static void Create() => 
			bitmapDescriptors = new Dictionary<int, BitmapDescriptor>(8);

		public static BitmapDescriptor ToBitmapDescriptor(this Bitmap bitmap)
		{
			var bitmapHash = bitmap.GetHashCode();

			if (bitmapDescriptors.ContainsKey(bitmapHash))
				return bitmapDescriptors[bitmapHash];

			var descriptor = BitmapDescriptorFactory.FromBitmap(bitmap);
			bitmapDescriptors.Add(bitmap.GetHashCode(), descriptor);
			return descriptor;
		}
	}
}