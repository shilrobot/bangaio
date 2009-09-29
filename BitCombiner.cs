using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class BitCombiner
     {
        public InputPin<float> Input;
        public OutputPin<float> Output = new OutputPin<float>();

        private bool first = true;

        public BitCombiner() : this(2)
        {
        }

        public BitCombiner(int bufferSize)
        {
            if (bufferSize % 2 != 0)
                throw new ArgumentException("BufferSize must be divisible by two", "bufferSize");
            Input = new InputPin<float>(bufferSize);
            Input.BufferFilled += new InputPin<float>.BufferFilledHandler(Input_BufferFilled);
        }

        void Input_BufferFilled(float[] buffer, int bufSize)
        {
            for (int i = 0; i < bufSize; i+=2)
                Output.Write(buffer[i] + buffer[i + 1]);
        }
    }
}
