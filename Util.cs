using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    static class Util
    {
        public static int CalcBufferSize(float seconds, int fs)
        {
            return (int)Math.Ceiling(seconds * fs);
        }

        public static float GaussianMax(float sigma)
        {
            return 1.0f / (sigma * (float)Math.Sqrt(2 * Math.PI));
        }

        public static float GaussianPdf(float x, float mu, float sigma)
        {
            // NOTE: Divide by zero risk
            return (float)(1 / (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x - mu) * (x - mu) / (2 * sigma * sigma)));
        }
    }
}
