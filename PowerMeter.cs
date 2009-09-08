using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class PowerMeter
    {
        public delegate void PowerUpdatedHandler(float db);

        public event PowerUpdatedHandler PowerUpdated;

        public Buffer<float> InputBuffer;

        public PowerMeter(int bufferSize)
        {
            InputBuffer = new Buffer<float>(bufferSize);
            InputBuffer.BufferFilled += new Buffer<float>.BufferFilledHandler(InputBuffer_BufferFilled);
        }

        void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            // TODO: DC removal?

            float invSize = 1.0f / bufSize;

            float dc = 0;
            for (int i = 0; i < bufSize; ++i)
                dc += buffer[i] * invSize;

            float power = 0.0f;
            for (int i = 0; i < bufSize; ++i)
            {
                float x = buffer[i] - dc;
                power += x * x * invSize;
            }

            float db = 10.0f * (float)Math.Log10(power);

            if (PowerUpdated != null)
                PowerUpdated(db);
        }
    }
}
