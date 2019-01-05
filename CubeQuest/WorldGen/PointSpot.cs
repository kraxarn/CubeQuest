using Android.Gms.Maps.Model;

namespace CubeQuest.WorldGen
{
	internal class PointSpot
    {
        public PointSpot(int x, int y, double count = 0)
        {
            X = x;
            Y = y;
            Count = count;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public double Count { get; set; }

        public double Lat => MapCoordToLonLat.CalcLat(X);
        public double Lon => MapCoordToLonLat.CalcLon(Y);

        public LatLng ToLatLng() => 
	        new LatLng(Lat, Lon);

        public override string ToString() => 
	        $"X:{X} - Y:{Y} -- Lat: {Lat} - Lon:{Lon}";
    }
}
