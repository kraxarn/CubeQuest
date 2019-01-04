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

namespace CubeQuest.WorldGen
{
    class Test
    {
        public static void generateTestMarker(LatLng coord)
        {
            MapHandler.AddMarker(coord, "Title", BitmapDescriptorFactory.FromAsset("enemy/snake.webp"));
        }
    }
}