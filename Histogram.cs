using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BangaiO
{
    public partial class Histogram : Plot
    {
        public const int BucketCount = 64;
        public Buffer<float> InputBuffer = new Buffer<float>(1024);
        private int[] buckets = new int[BucketCount];

        public Histogram()
        {
            InitializeComponent();
            InputBuffer.BufferFilled += new Buffer<float>.BufferFilledHandler(InputBuffer_BufferFilled);
            LeftBound = 0;
            RightBound = BucketCount*1.2f;
            BottomBound = 0;
            TopBound = 1;
        }

        void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            float min = buffer[0];
            float max = buffer[0];
            for (int i = 0; i < bufSize; ++i)
            {
                min = Math.Min(buffer[i], min);
                max = Math.Max(buffer[i], max);
            }

            for (int i = 0; i < BucketCount; ++i)
                buckets[i] = 0;

            int highestBucket = 0;

            for (int i = 0; i < bufSize; ++i)
            {
                float x = buffer[i];
                x = x / max;
                x *= BucketCount;
                int whichBucket = (int)x;
                if (whichBucket >= BucketCount)
                    whichBucket = BucketCount - 1;
                if (whichBucket < 0)
                    whichBucket = 0;
                buckets[whichBucket]++;

                highestBucket = Math.Max(highestBucket, buckets[whichBucket]);
            }

            TopBound = highestBucket;

            DrawLine(Color.FromArgb(0, 255, 0), -1, 0, 0, buckets[0]);
            Clear();
            for (int i = 0; i < BucketCount; ++i)
            {
                int nextHeight = (i == BucketCount - 1) ? 0 : buckets[i + 1];
                DrawLine(Color.FromArgb(0,255,0), i, buckets[i], i + 1, nextHeight);
            }
        }
    }
}
