using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeQuest.WorldGen
{
	internal class ChunkHandler
    {
        // (Math.Pow(x1-x2,2)+Math.Pow(y1-y2,2)) < (d*d);

        private List<Chunk> WorldChunks { get; }

        public ChunkHandler() => 
	        WorldChunks = new List<Chunk>();

        public void UpdateCoordinate(double lat, double lon)
        {
            var chunkXCoordinate = MapCoordinateToLonLat.ConvertLatToChunkX(lat);
            var chunkYCoordinate = MapCoordinateToLonLat.ConvertLonToChunkY(lon);

            for (var i = 0; i < 9; i++)
            {
                var xDis = -1 + i % 3;
                var yDis = -1 + i / 3;

                var x = chunkXCoordinate + xDis;
                var y = chunkYCoordinate + yDis;

                if (!WorldChunks.Any(c => c.X == x && c.Y == y))
	                WorldChunks.Add(new Chunk(x, y));
            }

            for (var i = WorldChunks.Count - 1; i >= 0; i--)
            {
                if (Math.Abs(chunkXCoordinate - WorldChunks[i].X) > 1 || Math.Abs(chunkYCoordinate - WorldChunks[i].Y) > 1)
                {
                    WorldChunks[i].Unload();
                    WorldChunks.Remove(WorldChunks[i]);
                }
            }
        }
    }
}