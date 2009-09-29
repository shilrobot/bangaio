using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public sealed class InputPin<T>
    {
        public delegate void BufferFilledHandler(T[] buffer, int bufSize);

        public event BufferFilledHandler BufferFilled;

        private T[] buffer;
        private int bufferPos = 0;

        public InputPin(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("Buffer size must be a positive integer");
            buffer = new T[size];
        }

        public InputPin() : this(1)
        {
        }

        public void Write(T[] data, int offset, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                buffer[bufferPos++] = data[i+offset];
                if (bufferPos == buffer.Length)
                {
                    if (BufferFilled != null)
                        BufferFilled(buffer, buffer.Length);
                    bufferPos = 0;
                }
            }
        }

    }
}
