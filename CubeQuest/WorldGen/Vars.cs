namespace CubeQuest.WorldGen
{
	internal class Vars
    {
        public static int LonMin = -90;
        public static int LonMax = 90;
        public static int LatMin = -180;
        public static int LatMax = 180;

        public static int ChunkXWidth = 50;
        public static int ChunkYHeight = 50;
        public static double ChunkWidth = 0.004;
        public static double ChunkHeight = 0.004;

        public static int LonRange => LonMax - LonMin;
        public static int LatRange => LatMax - LatMin;
        public static double XRange => ChunkXWidth * (LonRange / ChunkWidth);
        public static double YRange => ChunkYHeight * (LatRange / ChunkHeight);
    }
}
