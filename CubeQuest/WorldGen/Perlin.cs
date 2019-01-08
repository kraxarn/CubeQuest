
using System;
using System.Runtime.CompilerServices;

namespace CubeQuest.WorldGen
{
	public static class Perlin
	{

		public static double OctavePerlin(double x, double y, double z, int octaves, double persistence)
		{
			double total = 0;
			double frequency = 1;
			double amplitude = 1;
			for (int i = 0; i < octaves; i++)
			{
				total += perlin(x * frequency, y * frequency, z * frequency) * amplitude;

				amplitude *= persistence;
				frequency *= 2;
			}

			return total;
		}

        /*
		private static readonly int[] permutation =
		{
			56, 100, 32, 169, 80, 114, 37, 38, 226, 69, 224, 3, 94, 253, 29, 149, 109, 13, 231, 28, 143, 15, 150, 160, 108, 55, 154, 118, 245, 36, 20, 199, 236, 163, 58, 195, 148, 213, 161, 131, 5, 99, 142, 196, 52, 215, 237, 50, 152, 68, 180, 197, 85, 186, 113, 6, 63, 21, 218, 81, 23, 79, 212, 210, 175, 78, 238, 127, 10, 168, 98, 140, 16, 12, 223, 17, 130, 254, 174, 91, 240, 171, 19, 222, 139, 198, 255, 39, 201, 26, 84, 225, 103, 173, 153, 95, 214, 252, 96, 111, 209, 71, 135, 47, 251, 194, 247, 54, 2, 241, 235, 73, 11, 30, 105, 138, 46, 176, 220, 145, 167, 230, 8, 157, 89, 250, 242, 24, 137, 115, 83, 206, 82, 147, 162, 132, 129, 188, 40, 221, 216, 66, 155, 134, 34, 93, 62, 124, 205, 97, 192, 25, 166, 92, 104, 59, 106, 117, 14, 189, 42, 33, 158, 217, 57, 227, 49, 200, 18, 193, 0, 48, 185, 9, 65, 70, 229, 133, 122, 232, 187, 208, 72, 119, 246, 211, 165, 172, 233, 41, 35, 181, 177, 234, 243, 121, 170, 151, 136, 244, 88, 76, 86, 141, 228, 207, 31, 43, 107, 27, 239, 249, 146, 1, 51, 156, 179, 182, 44, 7, 125, 164, 202, 120, 203, 67, 112, 191, 61, 248, 123, 219, 144, 87, 190, 45, 74, 53, 116, 126, 4, 110, 159, 90, 184, 22, 60, 102, 204, 183, 128, 101, 178, 77, 75, 64
		};
        */
    private static readonly int[] permutation = { 151,160,137,91,90,15,					// Hash lookup table as defined by Ken Perlin.  This is a randomly
		131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,	// arranged array of all numbers from 0-255 inclusive.
		190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
    };

        public static void UseSeed(int seed)
        {
            Random r = new Random(seed);
            int n = permutation.Length;
            while (n > 1)
            {
                int k = r.Next(n--);
                int temp = permutation[n];
                permutation[n] = permutation[k];
                permutation[k] = temp;
            }
            Permutation();
        }

		private static readonly int[] p;                                                    // Doubled permutation to avoid overflow

		static Perlin()
		{
			p = new int[512];
            Permutation();
		}

        private static void Permutation()
        {
            for (int x = 0; x < 512; x++)
            {
                p[x] = permutation[x % 256];
            }
        }

		public static double perlin(double x, double y, double z)
		{
			int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in
			int yi = (int)y & 255;                              // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
			int zi = (int)z & 255;                              // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
			double xf = x - (int)x;                             // We also fade the location to smooth the result.
			double yf = y - (int)y;
			double zf = z - (int)z;
			double u = fade(xf);
			double v = fade(yf);
			double w = fade(zf);

			int a = p[xi] + yi;                             // This here is Perlin's hash function.  We take our x value (remember,
			int aa = p[a] + zi;                             // between 0 and 255) and get a random value (from our p[] array above) between
			int ab = p[a + 1] + zi;                             // 0 and 255.  We then add y to it and plug that into p[], and add z to that.
			int b = p[xi + 1] + yi;                             // Then, we get another random value by adding 1 to that and putting it into p[]
			int ba = p[b] + zi;                             // and add z to it.  We do the whole thing over again starting with x+1.  Later
			int bb = p[b + 1] + zi;                             // we plug aa, ab, ba, and bb back into p[] along with their +1's to get another set.
			// in the end we have 8 values between 0 and 255 - one for each vertex on the unit cube.
			// These are all interpolated together using u, v, and w below.

			double x1, x2, y1, y2;
			x1 = lerp(grad(p[aa], xf, yf, zf),          // This is where the "magic" happens.  We calculate a new set of p[] values and use that to get
				grad(p[ba], xf - 1, yf, zf),            // our final gradient values.  Then, we interpolate between those gradients with the u value to get
				u);                                     // 4 x-values.  Next, we interpolate between the 4 x-values with v to get 2 y-values.  Finally,
			x2 = lerp(grad(p[ab], xf, yf - 1, zf),          // we interpolate between the y-values to get a z-value.
				grad(p[bb], xf - 1, yf - 1, zf),
				u);                                     // When calculating the p[] values, remember that above, p[a+1] expands to p[xi]+yi+1 -- so you are
			y1 = lerp(x1, x2, v);                               // essentially adding 1 to yi.  Likewise, p[ab+1] expands to p[p[xi]+yi+1]+zi+1] -- so you are adding
			// to zi.  The other 3 parameters are your possible return values (see grad()), which are actually
			x1 = lerp(grad(p[aa + 1], xf, yf, zf - 1),      // the vectors from the edges of the unit cube to the point in the unit cube itself.
				grad(p[ba + 1], xf - 1, yf, zf - 1),
				u);
			x2 = lerp(grad(p[ab + 1], xf, yf - 1, zf - 1),
				grad(p[bb + 1], xf - 1, yf - 1, zf - 1),
				u);
			y2 = lerp(x1, x2, v);

			return (lerp(y1, y2, w) + 1) / 2;                       // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
		}

		public static double grad(int hash, double x, double y, double z)
		{
			int h = hash & 15;                                  // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
			double u = h < 8 /* 0b1000 */ ? x : y;              // If the most signifigant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.

			double v;                                           // In Ken Perlin's original implementation this was another conditional operator (?:).  I
			// expanded it for readability.

			if (h < 4 /* 0b0100 */)                             // If the first and second signifigant bits are 0 set v = y
				v = y;
			else if (h == 12 /* 0b1100 */ || h == 14 /* 0b1110*/)// If the first and second signifigant bits are 1 set v = x
				v = x;
			else                                                // If the first and second signifigant bits are not equal (0/1, 1/0) set v = z
				v = z;

			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v); // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
		}

		public static double fade(double t)
		{
			// Fade function as defined by Ken Perlin.  This eases coordinate values
			// so that they will "ease" towards integral values.  This ends up smoothing
			// the final output.
			return t * t * t * (t * (t * 6 - 15) + 10);         // 6t^5 - 15t^4 + 10t^3
		}

		public static double lerp(double a, double b, double x)
		{
			return a + x * (b - a);
		}
	}
}