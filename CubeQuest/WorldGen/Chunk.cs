
using System.Collections.Generic;
using Android.Gms.Maps.Model;
using Android.Util;
using CubeQuest.Account;
using CubeQuest.Account.Interface;
using CubeQuest.Handler;
using CubeQuest.MonsterGen;

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
                LatLng coord = points[i].ToLatLng();
                if (MapHandler.Visited.Contains(coord))
                {
                    double val = points[i].Value;
                    IEnemy enemy = MonsterFactory.CreateMonster(val);
                    Marker m = MapHandler.AddMarker(coord, enemy.Name, ImageHandler.GetImage(enemy.Image));
                    m.Tag = new EnemyTag(enemy.GetType(), 1);
                    Markers.Add(m);
                }
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
