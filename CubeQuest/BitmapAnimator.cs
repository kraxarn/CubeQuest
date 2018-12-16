using System.Collections.Generic;
using Android.Graphics;

namespace CubeQuest
{
    public class BitmapAnimator
    {
        /// <summary>
        /// Current frame
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                if (frames >= delay)
                {
                    current++;
                    frames = 0;

                    if (current > bitmaps.Count)
                        current = 0;
                }

                frames++;
                return bitmaps[current];
            }
        }

        private readonly IReadOnlyList<Bitmap> bitmaps;

        /// <summary>
        /// Total number of frames since last bitmap change
        /// </summary>
        private int frames;

        /// <summary>
        /// Frames of delay until bitmap change
        /// </summary>
        private readonly int delay;

        /// <summary>
        /// Current frame/index returned
        /// </summary>
        private int current;

        /// <summary>
        /// Animate a bitmap
        /// </summary>
        /// <param name="bitmaps">All bitmaps in the animation</param>
        /// <param name="delay">Delay in frames between each frame</param>
        public BitmapAnimator(IReadOnlyList<Bitmap> bitmaps, int delay)
        {
            this.bitmaps = bitmaps;
            this.delay = delay;

            current = 0;
            frames = 0;
        }
    }
}