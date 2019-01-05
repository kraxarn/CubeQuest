﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World_generation
{
    class Vars
    {
        public static int LonMin = -90;
        public static int LonMax = 90;
        public static int LatMin = -180;
        public static int LatMax = 180;

        public static int ChunkXWidth = 50;
        public static int ChunkYHeight = 50;
        public static double ChunkWidth = 0.02;
        public static double ChunkHeight = 0.02;

        public static int LonRange => LonMax - LonMin;
        public static int LatRange => LatMax - LatMin;
        public static double XRange => ChunkXWidth * (LonRange / ChunkWidth);
        public static double YRange => ChunkYHeight * (LatRange / ChunkHeight);
    }
}