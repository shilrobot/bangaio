using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class BitCombiner
     {
        public Buffer<float> InputBuffer;
        public Buffer<float> OutputBuffer;

        private bool first = true;

        public BitCombiner(int bufferSize)
        {
            if (bufferSize % 2 != 0)
                throw new ArgumentException("BufferSize must be divisible by two", "bufferSize");
            InputBuffer = new Buffer<float>(bufferSize);
            InputBuffer.BufferFilled += new Buffer<float>.BufferFilledHandler(InputBuffer_BufferFilled);
        }

        void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            for (int i = 0; i < bufSize; i+=2)
                OutputBuffer.Write(buffer[i] + buffer[i + 1]);
        }
    }
}
