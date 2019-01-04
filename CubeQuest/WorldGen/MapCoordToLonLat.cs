using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World_generation
{
    class MapCoordToLonLat
    {
        public static double CalcLon(double x) => ((Map(Vars.LonRange, Vars.XRange, x) + Vars.LonMax) % Vars.LonRange) + Vars.LonMin;
        public static double CalcLat(double y) => ((Map(Vars.LatRange, Vars.YRange, y) + Vars.LatMax) % Vars.LatRange) + Vars.LatMin;


        public static int ConvertLatToChunkX(double lat) => (int)(lat / Vars.ChunkWidth);
        public static int ConvertLonToChunkY(double lon) => (int)(lon / Vars.ChunkHeight);

        private static double Map(double toRange, double fromRange, double mapValue) => (toRange / fromRange) * mapValue;
    }
}
