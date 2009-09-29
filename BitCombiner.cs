using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    // Combines half-bit energies into whole-bit energies. That is, for input values
    //
    //   x1, x2, x3, x4, x5, x6...
    //
    // it outputs
    //
    //   (x1+x2), (x3+x4), (x5+x6) ...
    //
    public class BitCombiner
     {
        public InputPin<float> Input;
        public OutputPin<float> Output = new OutputPin<float>();

        public BitCombiner() : this(2)
        {
        }

        public BitCombiner(int bufferSize)
        {
            if ((bufferSize & 1) != 0)
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
