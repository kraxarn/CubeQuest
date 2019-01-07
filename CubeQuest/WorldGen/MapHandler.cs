using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Collections.Generic;

namespace CubeQuest.WorldGen
{
	static class MapHandler
    {
        public static GoogleMap Map;

        public static HashSet<LatLng> Visited;

        public static Marker AddMarker(LatLng latLng, string title, BitmapDescriptor icon) =>
	        Map.AddMarker(new MarkerOptions()
		        .SetPosition(latLng)
		        .SetTitle(title)
		        .Anchor(0.5f, 0.5f)
		        .SetIcon(icon));
    }
}