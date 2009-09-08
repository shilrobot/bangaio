using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{

    public sealed class Buffer<T>
    {
        public delegate void BufferFilledHandler(T[] buffer, int bufSize);

        public event BufferFilledHandler BufferFilled;

        private T[] buffer;
        private int bufferPos = 0;

        public Buffer(int size)
        {
            buffer = new T[size];
        }

        private void Dispatch()
        {
            if (BufferFilled != null)
                BufferFilled(buffer, buffer.Length);
            bufferPos = 0;
        }

        public void Write(T data)
        {
            buffer[bufferPos++] = data;
            if (bufferPos == buffer.Length)
                Dispatch();
        }

        public void Write(T[] data, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                buffer[bufferPos++] = data[i];
                if (bufferPos == buffer.Length)
                    Dispatch();
            }
        }

        public void Clear()
        {
            bufferPos = 0;
        }
    }
}
