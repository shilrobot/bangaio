using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class Splitter<T>
    {
        public Buffer<T> InputBuffer;
        public List<Buffer<T>> OutputBuffers = new List<Buffer<T>>();

        public Splitter(int bufSize)
        {
            InputBuffer = new Buffer<T>(bufSize);
            InputBuffer.BufferFilled += new Buffer<T>.BufferFilledHandler(InputBuffer_BufferFilled);
        }

        void InputBuffer_BufferFilled(T[] buffer, int bufSize)
        {
            foreach (Buffer<T> outputBuf in OutputBuffers)
                outputBuf.Write(buffer, bufSize);
        }
    }
}
