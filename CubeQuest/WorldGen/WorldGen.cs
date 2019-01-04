using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World_generation
{
    class WorldGen
    {
        const double multiplier = 0.1;
        static int[,] Dirr = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        public WorldGen()
        {
            //GenerateWorld();
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
                                localMinSpot.Count = val;
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


        public static void OldWorldGen()
        {
            int yEnd = 10000;
            int xEnd = 86;
            for (int y = 0; y < yEnd; y++)
            {
                for (int x = 0; x < xEnd; x++)
                {
                    //getChar(Perlin.OctavePerlin(x * multiplier, y * multiplier, 0, 4, 0.5));
                    //Console.Write(GetChar(Perlin.OctavePerlin(x * multiplier, y * multiplier, 0, 1, 1)));
                    Console.Write(GetChar(Perlin.OctavePerlin(x * multiplier, y * multiplier, 0, 4, 0.5)));
                    //Console.Write(GetChar(Perlin.OctavePerlin(x * multiplier, y * multiplier, 0, 2, 2)));
                    if (x < xEnd - 1)
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }

        static void GenerateWorld()
        {
            int xStart = -186;
            int yStart = -2000;
            int yEnd = -1000;
            int xEnd = -100;

            char[] lastRow = new char[xEnd-xStart];
            List<PointSpot> pointsToAdd = new List<PointSpot>();

            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    double val = Perling(x, y);
                    if (val <= 1)
                        if (lastRow[x-xStart] == ' ')
                        {
                            PointSpot pooi = GetLocalMin(x, y, val);
                            pooi.Count = val;
                            pointsToAdd.Add(pooi);
                        }

                    lastRow[x - xStart] = val <= 1 ? 'X' : ' ';
                    PointSpot poi = pointsToAdd.Find(p => p.X == x && p.Y == y);
                    bool excist = poi != null;
                    Console.Write(excist ? "X" : " ");
                    if (excist)
                    {
                        pointsToAdd.Remove(poi);
                    }

                }
                //Console.WriteLine();
            }
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

        static char GetChar(double v)
        {
            Char[] arr = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            int arLen = arr.Length;
            for (int i = 0; i < arLen; i++)
            {
                double val = ((i + 1) / (double)arLen);
                if (v < val)
                {
                    return arr[i];
                }
            }
            return ' ';
        }
    }
}
