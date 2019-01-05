using Android.Gms.Maps.Model;

namespace CubeQuest.WorldGen
{
	internal class Test
    {
        public static void GenerateTestMarker(LatLng coord) => 
	        MapHandler.AddMarker(coord, "Title", BitmapDescriptorFactory.FromAsset("enemy/snake.webp"));
    }
}