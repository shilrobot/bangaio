using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class UnitOffset<T>
    {
        public InputPin<T> Input;
        public OutputPin<T> Output = new OutputPin<T>();

        private bool first = true;

        public UnitOffset()
            : this(1)
        {
        }

        public UnitOffset(int bufferSize)
        {
            Input = new InputPin<T>(bufferSize);
            Input.BufferFilled += new InputPin<T>.BufferFilledHandler(Input_BufferFilled);
        }

        void Input_BufferFilled(T[] buffer, int bufSize)
        {
            for (int i = 0; i < bufSize; ++i)
            {
                if (!first)
                    Output.Write(buffer[i]);
                first = false;
            }
        }
    }
}
