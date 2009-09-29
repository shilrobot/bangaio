using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BangaiO
{
    public class ConvertToBytes
    {
        private enum State
        {
            Initial,
            ExpectZero,
            ExpectOne,
            Zero1,
            Zero2,
            Active,
            Error
        }

        public InputPin<bool> Input = new InputPin<bool>();
        public OutputPin<byte> Output = new OutputPin<byte>();
        private State state = State.Initial;
        private byte currByte = 0;
        private int currBit = 0;
        private FileStream fs;

        public ConvertToBytes()
        {
            Input.BufferFilled += new InputPin<bool>.BufferFilledHandler(Input_BufferFilled);
            //fs = new FileStream("out.bin", FileMode.Create, FileAccess.Write);
        }

        void Input_BufferFilled(bool[] buffer, int bufSize)
        {
            for (int i = 0; i < bufSize; ++i)
                BitReceived(buffer[i]);
        }

        void Reset()
        {
            state = State.Initial;
            currByte = 0;
            currBit = 0;
        }

        private void BitReceived(bool bit)
        {
            /*if(state != State.Active &&
                state != State.Error)
            {
                Console.WriteLine("Bit {0}", bit?"1":"0");
            }*/

            switch(state)
            {
                case State.Initial:
                    if(bit)
                        EnterState(State.ExpectZero);
                    else
                        EnterState(State.ExpectOne);
                    break;

                case State.ExpectZero:
                    if(bit)
                        EnterState(State.Error);
                    else
                        EnterState(State.ExpectOne);
                    break;

                case State.ExpectOne:
                    if(bit)
                        EnterState(State.ExpectZero);
                    else
                        EnterState(State.Zero1);
                    break;

                case State.Zero1:
                    if(bit)
                        EnterState(State.Error);
                    else
                        EnterState(State.Zero2);
                    break;

                case State.Zero2:
                    if(bit)
                        EnterState(State.Error);
                    else
                        EnterState(State.Active);
                    break;

                case State.Active:
                    if (bit)
                        currByte |= (byte)(1 << currBit);
                    ++currBit;
                    if (currBit == 8)
                    {
                        /*Console.WriteLine("{0:X2}", currByte);
                        byte[] bytes = new byte[] { currByte };
                        fs.Write(bytes, 0, 1);
                        fs.Flush();*/
                        Output.Write(currByte);
                        currBit = 0;
                        currByte = 0;
                    }
                    break;

                case State.Error:
                    break;
            }
        }

        private void EnterState(State newState)
        {
            //Console.WriteLine("State: {0} -> {1}", state, newState);
            state = newState;
        }
    }
}
