using System.Collections.Generic;
using Android.Gms.Maps.Model;
using Android.Util;

namespace CubeQuest.WorldGen
{
	internal class Chunk
    {
        public Chunk(int x, int y)
        {
            X = x;
            Y = y;
            Markers = new List<Marker>();
            Load();
        }

        public List<Marker> Markers { get; private set; }

        public int X;
        public int Y;

        public void Load()
        {
            List<PointSpot> points = WorldGen.LoadChunk(X, Y);
            for (int i = 0; i < points.Count; i++)
            {
                Markers.Add(MapHandler.AddMarker(points[i].ToLatLng(), "Title", BitmapDescriptorFactory.FromAsset("enemy/snake.webp")));
            }
        }

        public void Unload()
        {
            for (int i = Markers.Count - 1; i >= 0; i--)
            {
                Markers[i].Remove();
            }
        }

        public void HideMatches(List<LatLng> interaktedList)
        {
            for (int i = Markers.Count - 1; i >= 0; i--)
            {
                for (int j = interaktedList.Count - 1; j >= 0; j--)
                {
                    if (Markers[i].Position.Equals(interaktedList[j]))
                    {
                        Markers[i].Visible = false;
                    }
                }
            }
            //Loop all point look for match then hide
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is Chunk) && ((obj as Chunk).X == this.X && (obj as Chunk).Y == this.Y);
        }
    }
}
