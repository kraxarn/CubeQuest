namespace CubeQuest.WorldGen
{
	internal static class MapCoordinateToLonLat
    {
        public static double CalcLon(double x) => 
	        x / Vars.ChunkXWidth * Vars.ChunkWidth;

        public static double CalcLat(double y) =>
	        y / Vars.ChunkYHeight * Vars.ChunkHeight;
		
        public static int ConvertLatToChunkX(double lat) => 
	        (int) (lat / Vars.ChunkWidth);

        public static int ConvertLonToChunkY(double lon) => 
	        (int) (lon / Vars.ChunkHeight);
    }
}