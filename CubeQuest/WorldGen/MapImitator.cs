using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World_generation
{
    class MapImitator
    {
        double lon = 1000;
        double lat = 1000;
        //SOme list of pins
        //Update function
        public MapImitator()
        {
            WorldGen wg = new WorldGen();
            //WorldGen.OldWorldGen();

            ConsoleKeyInfo key;
            ChunkHandler ch = new ChunkHandler();
            while (true)
            {
                Console.Write("\nPress an arrow key: ");
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        lat += 0.02;
                        break;
                    case ConsoleKey.RightArrow:
                        lat -= 0.02;
                        break;
                    case ConsoleKey.UpArrow:
                        lon += 0.02;
                        break;
                    case ConsoleKey.DownArrow:
                        lon -= 0.02;
                        break;
                    default:
                        break;
                }
                ch.UpdateCoord(lat, lon);

                Console.Clear();
                Console.WriteLine("Last move: " + key.Key.ToString());
                ch.Print();
                //Console.WriteLine(ch.WorldPoints.Count);
            }
        }
    }
}
