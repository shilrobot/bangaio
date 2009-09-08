using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BangaiO
{
    public class Decider2
    {
        private enum State
        {
            Waiting,
            Locked
        }

        public Buffer<float> InputBuffer = new Buffer<float>(32);
        private int index;
        private const int TrainingSampleCount = 100;
        private int trainingSamples = 0;

        private State state = State.Waiting;
        private Statistics oneStats = new Statistics();
        private Statistics zeroStats = new Statistics();

        private Statistics signalPower = new Statistics();
        private Statistics noisePower = new Statistics();

        private StreamWriter sw;
        private float bestSNR = 0.0f;

        public Decider2(int index)
        {
            this.index = index;
            InputBuffer.BufferFilled += new Buffer<float>.BufferFilledHandler(InputBuffer_BufferFilled);
            sw = new StreamWriter(String.Format("decider{0}.txt",index));
        }

        public void Finish()
        {
            sw.Close();
        }

        void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            if(state == State.Waiting)
            {
                float invSize = 1.0f / bufSize;
                float mean = 0;
                for (int i = 0; i < bufSize; ++i)
                    mean += buffer[i]*invSize;

                bool lastBit = buffer[0] > mean;
                int flips = 0;

                for (int i = 1; i < bufSize; ++i)
                {
                    bool newBit = buffer[i] > mean;
                    if (newBit != lastBit)
                        ++flips;
                    lastBit = newBit;
                }

                //Console.WriteLine("flips={0}", flips);

                if (flips == (bufSize - 1))
                {
                    Statistics tmpOneStats = new Statistics();
                    Statistics tmpZeroStats = new Statistics();

                    for (int i = 0; i < bufSize; ++i)
                    {
                        if (buffer[i] > mean)
                            tmpOneStats.AddSample(buffer[i]);
                        else
                            tmpZeroStats.AddSample(buffer[i]);
                    }

                    // sanity check means
                    // TODO: Work out what the probability of this really is for realistic noise levels
                    if (tmpOneStats.Mean / tmpZeroStats.Mean > 1.5f)
                    {
                        state = State.Locked;

                        Console.WriteLine("Decider {0} locked on!", index);

                        for (int i = 0; i < bufSize; ++i)
                        {
                            Bit(buffer[i], mean);
                        }
                    }
                }
            }
            else if (state == State.Locked)
            {

                for (int i = 0; i < bufSize; ++i)
                {
                    float thresh = 0.5f * (oneStats.Mean + zeroStats.Mean);
                    Bit(buffer[i], thresh);
                }

                float SNR = (float)(10 * Math.Log10(signalPower.Mean / noisePower.Mean));
                if (SNR > bestSNR*1.1)
                {
                    Console.WriteLine("({0}) SNR: {1:0.0} dB", index, SNR);
                    bestSNR = SNR;
                }
            }
        }

        private void Bit(float x, float thresh)
        {
            bool bit = x > thresh;

            float signal, noise;

            if (bit)
            {
                signal = oneStats.Mean;
                oneStats.AddSample(x);
            }
            else
            {
                signal = zeroStats.Mean;
                zeroStats.AddSample(x);
            }

            noise = x - signal;
            signalPower.AddSample(signal * signal);
            noisePower.AddSample(noise * noise);

            sw.WriteLine("{0},{1}", x, thresh);
            sw.Flush();
        }


    }
}
