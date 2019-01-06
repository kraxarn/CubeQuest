using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeQuest.WorldGen
{
	internal class WorldGen
    {
        const double multiplier = 0.1;
        static int[,] Dirr = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        public WorldGen()
        {
        }

        public static List<PointSpot> GenerateArea(int xMin, int xMax, int yMin, int yMax)
        {
            char[] lastRow = new char[xMax - xMin];
            List<PointSpot> returnPoints = new List<PointSpot>();
            List<PointSpot> pointsToAdd = new List<PointSpot>();
            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    double val = Perling(x, y);
                    if (val <= 1)
                        if (lastRow[x - xMin] == ' ' && (x - xMin - 1 < 0 || lastRow[x - xMin - 1] == ' '))
                        {
                            PointSpot localMinSpot = GetLocalMin(x, y, val);
                            if (!(localMinSpot.X < xMin || localMinSpot.X >= xMax || localMinSpot.Y < yMin || localMinSpot.Y >= yMax))
                            {
                                localMinSpot.Value = val;
                                pointsToAdd.Add(localMinSpot);
                                returnPoints.Add(localMinSpot);
                            }
                        }

                    lastRow[x - xMin] = val <= 1 ? 'X' : ' ';
                    PointSpot poi = pointsToAdd.Find(p => p.X == x && p.Y == y);
                    if (poi != null)
                        pointsToAdd.Remove(poi);
                }
            }

            List<int> indexToRemove = new List<int>();

            for (int i = returnPoints.Count - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (returnPoints[i].X == returnPoints[j].X && returnPoints[i].Y == returnPoints[j].Y)
                    {
                        if (i != j)
                        {
                            indexToRemove.Add(i);
                        }
                    }
                }
            }
            indexToRemove = indexToRemove.Distinct().ToList();
            for (int i = 0; i < indexToRemove.Count; i++)
            {
                returnPoints.RemoveAt(indexToRemove[i]);
            }

            return returnPoints.Distinct().ToList();
        }

        static PointSpot GetLocalMin(int x, int y, double min)
        {
            for (int i = 0; i < 4; i++)
            {
                double val = Perling(x + Dirr[i, 0], y + Dirr[i, 1]);
                if (min > val)
                {
                    return GetLocalMin(x + Dirr[i, 0], y + Dirr[i, 1], val);
                }
            }
            return new PointSpot(x, y, min);
        }

        public static List<PointSpot> LoadChunk(int x, int y)
        {
            return GenerateArea(x * Vars.ChunkXWidth, (x + 1) * Vars.ChunkXWidth, y * Vars.ChunkYHeight, (y + 1) * Vars.ChunkYHeight);
        }

        static double Perling(int x, int y)
        {
            return Perlin.OctavePerlin(x * multiplier, y * multiplier, 0, 2, 2);
        }
    }
}
