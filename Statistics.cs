using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class Statistics
    {
        private float sum = 0;
        private float sumSquares = 0;
        private float min = 0;
        private float max = 0;
        private int count = 0;

        public int Count { get { return count; } }
        public float Mean
        {
            get
            {
                // TODO: Improve stability? Blah.
                if (count == 0)
                    return 0;
                return sum / count;
            }
        }

        public float StandardDeviation
        {
            get
            {
                if (count == 0)
                    return 0;
                float mean = sum / count;
                float variance = (sumSquares / count) - mean * mean;
                // possible due to FP error
                if (variance < 0)
                    return 0;
                float stdev = (float)Math.Sqrt(variance);
                if (Single.IsNaN(stdev))
                    Console.WriteLine("FUCK");
                return stdev;
            }
        }

        public float Min
        {
            get { return min; }
        }

        public float Max
        {
            get { return max; }
        }

        public void AddSample(float value)
        {
            sum += value;
            sumSquares += value * value;
            ++count;
            min = value < min ? value : min;
            max = value > max ? value : max;
        }

        public void Clear()
        {
            min = 0;
            max = 0;
            sum = 0;
            sumSquares = 0;
            count = 0;
        }

        public float Probability(float x)
        {
            return Util.GaussianPdf(x, Mean, StandardDeviation);
        }
    }
}
