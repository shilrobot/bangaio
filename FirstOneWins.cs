using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class FirstOneWins<T>
    {
        public InputPin<T> Input1 = new InputPin<T>();
        public InputPin<T> Input2 = new InputPin<T>();
        public OutputPin<T> Output = new OutputPin<T>();
        private int selection = -1;

        public FirstOneWins()
        {
            Input1.BufferFilled += new InputPin<T>.BufferFilledHandler(Input1_BufferFilled);
            Input2.BufferFilled += new InputPin<T>.BufferFilledHandler(Input2_BufferFilled);
        }

        void Reset()
        {
            selection = -1;
        }

        void Input1_BufferFilled(T[] buffer, int bufSize)
        {
            if (selection < 0 || selection == 1)
            {
                Output.Write(buffer, bufSize);
                selection = 1;
            }
        }

        void Input2_BufferFilled(T[] buffer, int bufSize)
        {
            if (selection < 0 || selection == 2)
            {
                Output.Write(buffer, bufSize);
                selection = 2;
            }
        }

    }
}
