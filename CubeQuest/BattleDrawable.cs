using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace CubeQuest
{
	public class BattleDrawable : Drawable
	{
		private readonly AnimatedImageDrawable selectedDrawable;

		public BattleDrawable(AssetManager assets)
		{
			// Load images
			selectedDrawable = CreateAnimatedDrawable(assets, "selected");
		}

		private static AnimatedImageDrawable CreateAnimatedDrawable(AssetManager assets, string fileName) => 
			CreateFromStream(assets.Open($"animated/{fileName}.webp"), fileName) as AnimatedImageDrawable;

		public override void Draw(Canvas canvas)
		{
			selectedDrawable.Draw(canvas);
			selectedDrawable.Start();
		}

		public override void SetAlpha(int alpha)
		{
			// Required override
		}

		public override void SetColorFilter(ColorFilter colorFilter)
		{
			// Required override
		}

		public override int Opacity => 
			(int) Format.Opaque;
	}
}