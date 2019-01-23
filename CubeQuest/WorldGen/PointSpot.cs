using Android.Gms.Maps.Model;

namespace CubeQuest.WorldGen
{
	internal class PointSpot
    {
        public PointSpot(int x, int y, double value = 0)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public double Value { get; set; }

        public double Lat => MapCoordinateToLonLat.CalcLat(X);
        public double Lon => MapCoordinateToLonLat.CalcLon(Y);

        public LatLng ToLatLng() => 
	        new LatLng(Lat, Lon);

        public override string ToString() => 
	        $"X:{X} - Y:{Y} -- Lat: {Lat} - Lon:{Lon}";
    }
}
