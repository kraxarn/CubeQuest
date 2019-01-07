using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Collections.Generic;

namespace CubeQuest.WorldGen
{
	static class MapHandler
    {
        public static GoogleMap Map;

        public static Dictionary<int, LatLng> Visited;

        public static void Init(GoogleMap map)
        {
            Map = map;
            if(Visited == null)
            {
                Visited = new Dictionary<int, LatLng>();
            }
        }

        public static Marker AddMarker(LatLng latLng, string title, BitmapDescriptor icon) =>
	        Map.AddMarker(new MarkerOptions()
		        .SetPosition(latLng)
		        .SetTitle(title)
		        .Anchor(0.5f, 0.5f)
		        .SetIcon(icon));
    }
}