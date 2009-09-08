using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    public class Mux
    {
        public Buffer<bool> InputBuffer1 = new Buffer<bool>(128);
        public Buffer<bool> InputBuffer2 = new Buffer<bool>(128);
        public Buffer<bool> OutputBuffer;
        private int selection = -1;

        public Mux()
        {
            InputBuffer1.BufferFilled += new Buffer<bool>.BufferFilledHandler(InputBuffer1_BufferFilled);
            InputBuffer2.BufferFilled += new Buffer<bool>.BufferFilledHandler(InputBuffer2_BufferFilled);
        }

        void InputBuffer1_BufferFilled(bool[] buffer, int bufSize)
        {
            if (selection == 1)
                OutputBuffer.Write(buffer, bufSize);
        }

        void InputBuffer2_BufferFilled(bool[] buffer, int bufSize)
        {
            if (selection == 2)
                OutputBuffer.Write(buffer, bufSize);
        }

        public void Select(int i)
        {
            if(selection == -1)
                selection = i;
        }


    }
}
