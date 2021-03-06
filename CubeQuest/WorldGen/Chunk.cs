﻿using Android.Gms.Maps.Model;
using CubeQuest.Account;
using CubeQuest.Account.Interface;
using CubeQuest.Handler;
using CubeQuest.MonsterGen;
using System.Collections.Generic;

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

        public List<Marker> Markers { get; }

        public int X;
        public int Y;

        public void Load()
        {
            // Save player level for later
            var playerLevel = AccountManager.CurrentUser.Level;

            List<PointSpot> points = WorldGen.LoadChunk(X, Y);
            for (int i = 0; i < points.Count; i++)
            {
                LatLng coord = points[i].ToLatLng();
                if (!MapHandler.Visited.ContainsKey(coord.GetHashCode()))
                {
                    double val = points[i].Value;
                    IEnemy enemy = MonsterFactory.CreateMonster(val);

                    enemy.Level += (int) playerLevel;
                    if (enemy.Level < 1)
                        enemy.Level = 1;

					var m = MapHandler.AddMarker(coord, enemy.Name, AssetLoader.GetEnemyBitmap(enemy).ToBitmapDescriptor());
					m.Tag = new EnemyTag(enemy.GetType(), enemy.Level);
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

        public override int GetHashCode() => 
	        Markers.GetHashCode();

        public override bool Equals(object obj) => 
	        obj is Chunk chunk && chunk.X == X && chunk.Y == Y;
    }
}
