using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    // Handles extracting the payload from a transmission, at the byte level.
    // The bytes transmitted are divided into 5 regions:
    // 1. SYNC.
    //      This is not handled here, but rather in Decider & ConvertToBytes. Looks like AA AA AA ... AA 0A
    //      if converted to bytes.
    // 2. HEADER.
    //      byte 0: File type (0x02 for map, 0x03 for replay)
    //      byte 1: Repeated file type
    //      byte 2-3: Little-endian, 16-bit payload size.
    //      byte 4-5: Little-endian, 16-bit payload size. (repeated)
    // 3. PAYLOAD DATA.
    //      Structure depends on file type; always contains the number of bytes specified in the HEADER region.
    // 4. CHECKSUM.
    //      bytes 0-4: Little-endian, 32-bit checksum.
    //                 The checksum is the arithmetic sum of all bytes in the PAYLOAD DATA region.
    // 5. ZEROS.
    //      The output is padded with zeros for a while, before the signal ceases entirely.
    //      We reset when the CHECKSUM region is reached, so it's not really handled here (or anywhere.)
    class Progress
    {
        private enum State
        {
            Header,
            Payload,
            Checksum,
            Error,
            //Done
        }

        public event Action ResetSignal;

        private State state = State.Header;

        private int byteIndex;
        private int size1;
        private int size2;
        private int size;
        private int computedChecksum = 0;
        private int theirChecksum = 0;

        public InputPin<byte> Input = new InputPin<byte>();

        int count = 0;

        public Progress()
        {
            Input.BufferFilled += new InputPin<byte>.BufferFilledHandler(InputBuffer_BufferFilled);
            Reset();
        }

        private void Reset()
        {
            byteIndex = 0;
            state = State.Header;
            size1 = 0;
            size2 = 0;
            size = 0;
            computedChecksum = 0;
            theirChecksum = 0;
            count = 0;
        }

        void InputBuffer_BufferFilled(byte[] buffer, int bufSize)
        {
            for (int i = 0; i < bufSize; ++i)
                ReceiveByte(buffer[i]);

            if (state == State.Payload && count == 128)
            {
                Console.WriteLine("{0}/{1} - {2:0.0}%", byteIndex, size, 100.0f * byteIndex / (float)size);
                count = 0;
            }
            ++count;
        }

        void Completed(bool error)
        {
            // RESET
            Reset();

            if (ResetSignal != null)
                ResetSignal();
        }

        void ReceiveByte(byte b)
        {
            if (state == State.Header)
            {
                if (byteIndex == 0 || byteIndex == 1)
                {
                    // TODO: Allow 0x03 for replays
                    if (b != 0x02)
                        Completed(true);
                    else
                        ++byteIndex;
                }
                else if (byteIndex == 2)
                {
                    size1 = b;
                    ++byteIndex;
                }
                else if (byteIndex == 3)
                {
                    size1 |= b << 8;
                    ++byteIndex;
                }
                else if (byteIndex == 4)
                {
                    size2 = b;
                    ++byteIndex;
                }
                else if (byteIndex == 5)
                {
                    size2 |= b << 8;
                    if (size2 != size1)
                    {
                        Console.WriteLine("ERROR: Sizes don't match!");
                        Completed(true);
                    }
                    else
                        size = size1;

                    state = State.Payload;
                    byteIndex = 0;
                }
            }
            else if (state == State.Payload)
            {
                computedChecksum += b;
                //computedChecksum &= 0xFFFF;

                byteIndex++;
                if (byteIndex == size)
                {
                    state = State.Checksum;
                    byteIndex = 0;
                }
            }
            else if (state == State.Checksum)
            {
                if (byteIndex == 0)
                {
                    theirChecksum = b;
                    ++byteIndex;
                }
                else if (byteIndex == 1)
                {
                    theirChecksum |= b<<8;
                    ++byteIndex;
                }
                else if (byteIndex == 2)
                {
                    theirChecksum |= b<<16;
                    ++byteIndex;
                }
                else
                {
                    theirChecksum |= b << 24;
                    Console.WriteLine("Computed Checksum: {0:X8}", computedChecksum);
                    Console.WriteLine("Their Checksum: {0:X8}", theirChecksum);
                    if(theirChecksum != computedChecksum)
                        Console.WriteLine("CHECKSUM FAIL");
                    else
                        Console.WriteLine("CHECKSUM OK :)");


                    Completed(theirChecksum != computedChecksum);
                }
            }
        }
    }
}
