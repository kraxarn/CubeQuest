using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World_generation
{
    class MapCoordToLonLat
    {
        public static double CalcLon(double x) => (x / Vars.ChunkXWidth) * Vars.ChunkWidth;
        public static double CalcLat(double y) => (y / Vars.ChunkYHeight) * Vars.ChunkHeight;


        public static int ConvertLatToChunkX(double lat) => (int)(lat / Vars.ChunkWidth);
        public static int ConvertLonToChunkY(double lon) => (int)(lon / Vars.ChunkHeight);

        private static double Map(double toRange, double fromRange, double mapValue) => (toRange / fromRange) * mapValue;
    }
}
