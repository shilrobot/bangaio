using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    /*
    public class Splitter<T>
    {
        public InputPin<T> InputBuffer;
        public List<InputPin<T>> OutputBuffers = new List<InputPin<T>>();

        public Splitter(int bufSize)
        {
            InputBuffer = new InputPin<T>(bufSize);
            InputBuffer.BufferFilled += new InputPin<T>.BufferFilledHandler(InputBuffer_BufferFilled);
        }

        void InputBuffer_BufferFilled(T[] buffer, int bufSize)
        {
            foreach (InputPin<T> outputBuf in OutputBuffers)
                outputBuf.Write(buffer, bufSize);
        }
    }
     * */
}
