using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BangaiO
{
    public class Decider
    {
        public Buffer<float> InputBuffer = new Buffer<float>(256);
        public Buffer<bool> OutputBuffer;

        public int TrainingSamples = 100;
        private int trainingCounter = 0;
        private Statistics oneStats = new Statistics();
        private Statistics zeroStats = new Statistics();
        private bool alternate = false;
        private List<bool> output = new List<bool>();
        private int idx;
        private StreamWriter sw;

        private Statistics sigPowerStats = new Statistics();
        private Statistics noisePowerStats = new Statistics();

        public Decider(int idx)
        {
            this.idx = idx;
            InputBuffer.BufferFilled += new Buffer<float>.BufferFilledHandler(InputBuffer_BufferFilled);
            sw = new StreamWriter(String.Format("decode{0}.txt", idx));
        }

        public void Finish()
        {
            sw.Close();
        }

        void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            for (int i = 0; i < bufSize; ++i)
            {
                float x = buffer[i];

                if (trainingCounter < TrainingSamples)
                {
                    if (alternate)
                        oneStats.AddSample(x);
                    else
                        zeroStats.AddSample(x);
                    alternate = !alternate;
                    ++trainingCounter;
                    
                    if(trainingCounter == TrainingSamples)
                    {
                        if (zeroStats.Mean > oneStats.Mean)
                        {
                            Statistics temp = zeroStats;
                            zeroStats = oneStats;
                            oneStats = temp;
                        }

                        Console.WriteLine("Decider {0} trained: 0={1},{2}, 1={3},{4}, ratio={5:0.0}",
                                        idx,
                                        zeroStats.Mean, zeroStats.StandardDeviation,
                                        oneStats.Mean, oneStats.StandardDeviation,
                                        oneStats.Mean / zeroStats.Mean);

                    }
                }
                else
                {
                    float thresh = (zeroStats.Mean + oneStats.Mean) * 0.5f;
                    //float probA = statsA.Probability(x);
                    //float probB = statsB.Probability(x);

                    bool value;
                    float refMean;

                    if (x > thresh)
                    {
                        value = true;
                        refMean = oneStats.Mean;
                        oneStats.AddSample(x);
                    }
                    else
                    {
                        value = false;
                        refMean = zeroStats.Mean;
                        zeroStats.AddSample(x);
                    }

                    float sigPower = refMean * refMean;
                    float diff = refMean - x;
                    float noisePower = diff * diff;

                    sigPowerStats.AddSample(sigPower);
                    noisePowerStats.AddSample(noisePower);

                    //output.Add(value);
                    sw.WriteLine("{0},{1}",x,thresh);

                    if (OutputBuffer != null)
                    {
                        OutputBuffer.Write(value);
                    }
                }
            }

            double snr = 10 * Math.Log10(sigPowerStats.Mean / noisePowerStats.Mean);
            Console.WriteLine("{0} SNR: {1:0.0} dB", idx, snr);
        }

    }
}
