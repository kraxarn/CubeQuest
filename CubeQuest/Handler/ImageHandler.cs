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
    class ImageHandler
    {
        private static Dictionary<string, BitmapDescriptor> images = new Dictionary<string, BitmapDescriptor>()
       {
        { "snake", BitmapDescriptorFactory.FromAsset("enemy/snake.webp")},
        { "dog", BitmapDescriptorFactory.FromAsset("enemy/snake.webp")},
        { "", BitmapDescriptorFactory.FromAsset("enemy/snake.webp")}
    };
        public static BitmapDescriptor GetImage(string name) => images[name];
    }
}