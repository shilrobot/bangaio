using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace BangaiO
{
    // Front-end demodulator: turns waveform into a stream of bits.
    // This works by correlating a sine wave with an approximate carrier to small segments of the input
    // audio stream; this tells us the correct phase without needing *exactly* the right frequency.
    // Then, given the carrier, we can count the energy between zero-crossings to get the half-bit energies,
    // which are output.
    public class FrontEnd
    {
        public double Fs { get; set; }
        public double Fc { get; set; } // APPROXIMATE carrier

        // TODO: Make some of these configurable?
        private const int FrameSize = 2084;
        private const int LerpSamples = 512;
        private float[] frame = new float[FrameSize];
        private float[] frameCarrier = new float[FrameSize];
        private int absoluteSample = 0; // todo: hmm, do we lose accuracy here?
        private double lastPhase = 0;

        private float energy = 0;
        private float lastCarrier = 0;

        public InputPin<float> Input = new InputPin<float>(FrameSize);
        public OutputPin<float> Output = new OutputPin<float>();
        public OutputPin<float> PhaseOutput = new OutputPin<float>();
        public OutputPin<PointF> EyeOutput = new OutputPin<PointF>();

        //private StreamWriter sw = new StreamWriter("out.txt");

        public FrontEnd(double Fs, double Fc)
        {
            this.Fs = Fs;
            this.Fc = Fc;
            Input.BufferFilled += new InputPin<float>.BufferFilledHandler(InputBuffer_BufferFilled);
        }

        // TODO: Way to flush the last frame through w/ appended zeros

        // this counts on the buffer being long enough that the average is approximately the DC value
        private void RemoveDC(float[] data, int count)
        {
            float dc = 0;
            float invCount = 1.0f / count;
            for (int i = 0; i < count; ++i)
                dc += data[i] * invCount;
            for (int i = 0; i < count; ++i)
                data[i] -= dc;
        }


        static float peakSNR = 0.0f;

        private double DetectPhase(float[] data, int count, int startSample)
        {
            double corrRe = 0.0f;
            double corrIm = 0.0f;

            for (int i = 0; i < count; ++i)
            {
                float value = data[i];
                double t = (startSample + i) / Fs;
                double a = Math.Cos(2 * Math.PI * Fc * t);
                double b = Math.Sin(2 * Math.PI * Fc * t);
                corrRe += a * value;
                corrIm += b * value;
            }
            
            
            /*float invCount = 1.0f / count;
            // This is just for the positive frequency, so we have to multiply by two
            float voltage = 2.0f * (float)Math.Sqrt(corrRe * corrRe + corrIm * corrIm);
            voltage = voltage * invCount;

            float signalPower = 0.5f * voltage * voltage;

            float power = 0.0f;
            for (int i = 0; i < count; ++i)
                power += invCount * data[i] * data[i];

            float noisePower = power - signalPower;
            float SNR = 10.0f * (float)Math.Log10(power / noisePower);

            if (SNR > peakSNR)
            {
                Console.WriteLine("Peak SNR: {0:0.0} dB", SNR);
                peakSNR = SNR;
            }

            Console.WriteLine("Total: {0:0.0} dB -- Signal: {1:0.0} dB -- Noise: {2:0.0} dB -- SNR",
                                10 * Math.Log10(power),
                                10 * Math.Log10(signalPower),
                                10 * Math.Log10(power-signalPower));*/
            

            return Math.Atan2(corrIm, corrRe);
        }

        private double NormalizePhase(double a)
        {
            a = a % (Math.PI * 2);
            if (a < 0)
                a += Math.PI * 2;
            return a;
        }

        // TODO: Test this further
        private double LerpPhase(double a, double b, double t)
        {
            a = NormalizePhase(a);
            b = NormalizePhase(b);
            double result;
            if (Math.Abs(b - a) < Math.PI)
                result = (1 - t) * a + t * b;
            else
            {
                if (b > Math.PI)
                    b = b - 2 * Math.PI;
                else
                    a = a - 2 * Math.PI;
                result = (1 - t) * a + t * b;
            }
            return NormalizePhase(result);
        }

        private void GenerateCarrier(float[] values, float[] dest, int count, int startSample, int lerpSamples, double oldPhase, double newPhase)
        {
            for (int i = 0; i < count; ++i)
            {
                double t = (startSample + i) / Fs;

                double currPhase;

                if (i < lerpSamples)
                {
                    double t2 = (double)i / (double)lerpSamples;
                    currPhase = LerpPhase(oldPhase, newPhase, t2);
                }
                else
                    currPhase = newPhase;

                double parameter = 2 * Math.PI * Fc * t - currPhase;
                dest[i] = (float)Math.Cos(parameter);

                EyeOutput.Write(new PointF((float)NormalizePhase(parameter + Math.PI*0.5f), values[i]));
            }
        }

        private void ExtractHalfBits(float[] data, float[] carrier, int count)
        {
            float dt = (float)(1/Fs);

            for (int i = 0; i < count; ++i)
            {
                //sw.WriteLine("{0},{1}", data[i], carrier[i]);

                float newEnergy = data[i] * carrier[i] * dt;
                if ((lastCarrier < 0 && carrier[i] >= 0) || (lastCarrier > 0 && carrier[i] <= 0))
                {
                    //sw.WriteLine("{0}", energy);
                    Output.Write(energy);
                    energy = newEnergy;
                }
                else
                    energy += newEnergy;

                lastCarrier = carrier[i];
            }
        }

        private void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            if (bufSize != FrameSize)
                throw new InvalidOperationException("Frame size should equal buffer size");

            for (int i = 0; i < bufSize; ++i)
                frame[i] = buffer[i];

            RemoveDC(frame, FrameSize);

            double newPhase = DetectPhase(frame, FrameSize, absoluteSample);
            PhaseOutput.Write((float)newPhase);

            GenerateCarrier(frame, frameCarrier, FrameSize, absoluteSample, LerpSamples, lastPhase, newPhase);

            ExtractHalfBits(frame, frameCarrier, FrameSize);


            lastPhase = newPhase;
            absoluteSample += FrameSize;
        }
    }
}
