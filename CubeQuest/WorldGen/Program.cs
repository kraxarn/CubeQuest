using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World_generation
{
    class Program
    {

        static void Main(string[] args)
        {

            MapImitator mi = new MapImitator();

            /*
            ChunkHandler ch = new ChunkHandler();
            ch.UpdateCoord(lat: 180, lon: 90);
            Console.WriteLine(ch.WorldPoints.Count);
            foreach (PointSpot p in ch.WorldPoints)
            {
                Console.WriteLine(p.ToString());
            }
            Console.ReadLine();
            */
        }
            /*
            WorldGen wg = new WorldGen();
            List<PointSpot> ps = wg.GenerateArea(0, 100, 0, 100);
            */
    }
}
