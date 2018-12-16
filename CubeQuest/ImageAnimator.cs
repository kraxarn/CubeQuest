using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Widget;

namespace CubeQuest
{
    public class ImageAnimator
    {
        private bool running;

        /// <summary>
        /// Animate an <see cref="ImageView"/>
        /// </summary>
        /// <param name="image"><see cref="ImageView"/> to be animated</param>
        /// <param name="bitmaps">All bitmaps in the animation</param>
        /// <param name="delay">Delay in milliseconds</param>
        public ImageAnimator(ImageView image, IReadOnlyList<Bitmap> bitmaps, int delay)
        {
            running = true;
            var i = 0;

            Task.Run(() =>
            {
                while (running)
                {
                    if (i == bitmaps.Count)
                        i = 0;

                    image.SetImageBitmap(bitmaps[i]);
                    i++;
                    Thread.Sleep(delay);
                }
            });
        }

        public void Stop() => running = false;
    }
}