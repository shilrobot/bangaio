using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BangaiO
{
    // Converts individual bit values to byte values.
    // This involves waiting for the sync period (atlernating 101010101...) to end,
    // and then reading bits LSB first and assembling them into bytes.
    // The end of the sync period looks like this, on the wire (with nibbles grouped):
    //
    // ... 0101 0101 0000 XXXX XXXX ...
    //                    ^
    //                    |
    //                    +-- data starts here

    // FSM Transition table
    //
    // State      |    0           1
    // -----------+-----------------------
    // Initial    | ExpectOne   ExpectZero
    // ExpectZero | ExpectOne   Error
    // ExpectOne  | Zero1       ExpectZero
    // Zero1      | Zero2       Error
    // Zero2      | Active      Error
    // Active     | Active      Active
    // Error      | Error       Error

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
