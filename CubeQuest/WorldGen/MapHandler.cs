using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CubeQuest.WorldGen
{
    class MapHandler
    {
        public static GoogleMap Map;
        public static List<LatLng> Visited;
        public static Marker AddMarker(LatLng latLng, String title, BitmapDescriptor icon)
        {
            return Map.AddMarker(new MarkerOptions()
                .SetPosition(latLng)
                .SetTitle(title)
                .Anchor(0.5f, 0.5f)
                .SetIcon(icon));
        }
    }
}