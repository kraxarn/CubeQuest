using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Widget;

namespace CubeQuest.Handler
{
    public class ImageAnimator
    {
	    public delegate void DoneEvent();

		/// <summary>
		/// When a non-repeating animation finishes
		/// </summary>
	    public event DoneEvent Done;
		
		private bool running;

		private IReadOnlyList<Bitmap> bitmaps;

		private int current;

		private int delay;

		private readonly ImageView image;

		/// <summary>
		/// Animate an <see cref="ImageView"/>
		/// </summary>
		/// <param name="image"><see cref="ImageView"/> to be animated</param>
		/// <param name="bitmaps">All bitmaps in the animation</param>
		/// <param name="delay">Delay in milliseconds</param>
		public ImageAnimator(ImageView image, IReadOnlyList<Bitmap> bitmaps, int delay)
		{
			this.bitmaps = bitmaps;
			this.delay   = delay;
			this.image   = image;

            running = true;
            current = 0;

            Task.Run(() => Loop());
        }

		/// <summary>
		/// Start a new animation on the same image
		/// </summary>
		public void New(IReadOnlyList<Bitmap> newBitmaps, int newDelay)
		{
			bitmaps = newBitmaps;
			delay   = newDelay;

			current = 0;

			if (!running)
				Task.Run(() => Loop());
		}

		private void Loop()
		{
			while (running)
			{
				if (current == bitmaps.Count)
				{
					Done?.Invoke();
					current = 0;
				}

				image.SetImageBitmap(bitmaps[current]);
				current++;
				Thread.Sleep(delay);
			}
		}

		/// <summary>
		/// Stop the animation at the current frame
		/// </summary>
        public void Stop() => running = false;
    }
}