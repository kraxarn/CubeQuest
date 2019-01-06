using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeQuest.WorldGen
{
	internal class ChunkHandler
    {
        //(Math.Pow(x1-x2,2)+Math.Pow(y1-y2,2)) < (d*d);

        public List<PointSpot> WorldPoints { get; private set; }
        public List<Chunk> WorldChunks { get; private set; }

        public ChunkHandler()
        {
            WorldPoints = new List<PointSpot>();
            WorldChunks = new List<Chunk>();
        }

        private List<PointSpot> LoadChunk(int x, int y)
        {
            return WorldGen.GenerateArea(x * Vars.ChunkXWidth, (x + 1) * Vars.ChunkXWidth, y * Vars.ChunkYHeight, (y + 1) * Vars.ChunkYHeight);
        }

        public void UpdateCoord(double lat, double lon)
        {
            int chunkXCoord = MapCoordToLonLat.ConvertLatToChunkX(lat);
            int chunkYCoord = MapCoordToLonLat.ConvertLonToChunkY(lon);


            int xDis;
            int yDis;
            int arrayIndex;

            for (int i = 0; i < 9; i++)
            {
                xDis = -1 + (i % 3);
                yDis = -1 + (i / 3);
                arrayIndex = 3 * yDis + xDis + 4; //3 * (y + 1) + (x + 1)
                int x = chunkXCoord + xDis;
                int y = chunkYCoord + yDis;
                if (!WorldChunks.Any(c => c.X == x && c.Y == y))
                {
                    WorldChunks.Add(new Chunk(x, y));
                }
            }

            for (int i = WorldChunks.Count - 1; i >= 0; i--)
            {
                if (Math.Abs(chunkXCoord - WorldChunks[i].X) > 1 || Math.Abs(chunkYCoord - WorldChunks[i].Y) > 1)
                {
                    WorldChunks[i].Unload();
                    WorldChunks.Remove(WorldChunks[i]);
                }
            }
        }
    }
}
