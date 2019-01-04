using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World_generation
{
    class PointSpot
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

        public LatLng ToLatLng()
        {
            return new LatLng(Lat, Lon);
        }

        public override string ToString()
        {
            return "X:" + X + " - Y:" + Y + " -- Lat: " + Lat + " - Lon:" + Lon;
        }
    }
}
