using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BangaiO
{
    // TODO: Break this into a separate detector & UI element
    public partial class SignalDetector : Control
    {
        private enum SignalState
        {
            NotDetected,
            Detected,
            Fading
        }

        const int WindowSize = 32;
        const int FailedWindowsBeforeCarrierLost = 7; // each window is ~ 16 msec atm, given that they're 1/2 bits

        public Buffer<float> InputBuffer = new Buffer<float>(WindowSize);
        public Buffer<float> OutputBuffer;
        private SignalState state = SignalState.NotDetected;
        private int fadeCount;
        private bool firstDetectedFrame = false;
        private StreamWriter sw = new StreamWriter("detected_data.txt");

        public SignalDetector()
        {
            InitializeComponent();
            InputBuffer.BufferFilled += new Buffer<float>.BufferFilledHandler(InputBuffer_BufferFilled);
        }

        public void Reset()
        {
            InputBuffer.Clear();
            state = SignalState.NotDetected;
            fadeCount = 0;
            firstDetectedFrame = false;
            Invalidate();
            sw.Close();
            sw = new StreamWriter("signal_detector.txt");
        }

        void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            float sum = 0, ss = 0;
            for (int i = 0; i < bufSize; ++i)
            {
                float x= buffer[i];
                sum += x;
                ss += (x * x);
            }

            float mean = sum / (float)bufSize;
            float expectedX2 = ss / (float)bufSize;
            float variance = expectedX2 - mean * mean;
            float stdev = (float)Math.Sqrt(variance);

            // Expected std. dev. of the sample mean in this window
            float stdevSampleMean = stdev / (float)Math.Sqrt(WindowSize);

            bool detected = mean > (stdevSampleMean * 3.0f);

            SignalState newState = state;

            if(state == SignalState.NotDetected)
            {
                if (detected)
                {
                    EnterState(SignalState.Detected);
                    firstDetectedFrame = true;
                }
            }
            else if(state == SignalState.Detected)
            {
                if(!detected)
                {
                    fadeCount = FailedWindowsBeforeCarrierLost;
                    EnterState(SignalState.Fading);
                }
            }
            else if(state == SignalState.Fading)
            {
                if(detected)
                    EnterState(SignalState.Detected);
                else
                {
                    fadeCount--;
                    if(fadeCount == 0)
                        EnterState(SignalState.NotDetected);
                }
            }

            for (int i = 0; i < bufSize; ++i)
            {
                if (state == SignalState.NotDetected)
                    sw.WriteLine("{0},0", buffer[i]);
                else
                    sw.WriteLine("0,{0}", buffer[i]);
            }

            if (state != SignalState.NotDetected)
            {
                /*for (int i = 0; i < bufSize; ++i)
                    sw.WriteLine("{0}\n", buffer[i]);*/
                if(OutputBuffer != null)
                    OutputBuffer.Write(buffer, bufSize);
            }

            if (state == SignalState.Detected)
                firstDetectedFrame = false;
        }

        private void EnterState(SignalState newState)
        {
            if (newState == state)
                return;
            state = newState;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (state == SignalState.Detected)
                pe.Graphics.Clear(Color.FromArgb(0, 255, 0));
            else if(state == SignalState.Fading)
                pe.Graphics.Clear(Color.Yellow);
            else if (state == SignalState.NotDetected)
                pe.Graphics.Clear(Color.Red);

            base.OnPaint(pe);
        }
    }
}
