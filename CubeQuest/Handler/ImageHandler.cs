using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CubeQuest.Handler
{
	public class ImageHandler
    {
        private static Dictionary<ImageName, BitmapDescriptor> images = new Dictionary<ImageName, BitmapDescriptor>()
       {
        { ImageName.ALIEN_BEIGE, BitmapDescriptorFactory.FromAsset("enemy/alien_beige.webp")},
        { ImageName.ALIEN_BLUE, BitmapDescriptorFactory.FromAsset("enemy/alien_blue.webp")},
        { ImageName.ALIEN_GREEN, BitmapDescriptorFactory.FromAsset("enemy/alien_green.webp")},
        { ImageName.ALIEN_PINK, BitmapDescriptorFactory.FromAsset("enemy/alien_pink.webp")},
        { ImageName.ALIEN_YELLOW, BitmapDescriptorFactory.FromAsset("enemy/alien_yellow.webp")}
    };

        public enum ImageName { ALIEN_BEIGE, ALIEN_BLUE,ALIEN_GREEN,ALIEN_PINK,ALIEN_YELLOW}
        public static BitmapDescriptor GetImage(ImageName name) => images[name];
    }
}