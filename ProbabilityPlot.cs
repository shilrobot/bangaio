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
    public partial class ProbabilityPlot : Plot
    {
        
        public InputPin<float> InputBuffer = new InputPin<float>(64*2);
        //private StreamWriter sw = new StreamWriter("bits.txt");

        private bool locked = false;
        private bool offByOne = false;
        private bool firstClear = false;

        private Statistics statsA = new Statistics();
        private Statistics statsB = new Statistics();
        private Statistics statsC = new Statistics();
        private Statistics statsD = new Statistics();

        public ProbabilityPlot()
        {
            InitializeComponent();
            InputBuffer.BufferFilled += new InputPin<float>.BufferFilledHandler(InputBuffer_BufferFilled);
            AutoInvalidate = false;
        }

        public void Reset()
        {
            locked = true;
            offByOne = false;
            firstClear = false;
            statsA.Clear();
            statsB.Clear();
            statsC.Clear();
            statsD.Clear();
        }

        void DrawGaussian(Color col, float mean, float stddev, float scale)
        {
            int points = this.ClientSize.Width >> 1;
            for (int i = 0; i < points; ++i)
            {
                float x1 = scale * (i / (float)points);
                float y1 = Util.GaussianPdf(x1, mean, stddev);
                float x2 = scale * ((i + 1) / (float)points);
                float y2 = Util.GaussianPdf(x2, mean, stddev);
                this.DrawLine(col, x1, y1, x2, y2);
            }
        }

        void InputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            /*
            statsA.Clear();
            statsB.Clear();
            statsC.Clear();
            statsD.Clear();


            for (int i = 0; i < bufSize-1; i += 2)
            {
                statsA.AddSample((buffer[i] + buffer[i+1])*0.5f);
            }

            for (int i = 0; i < bufSize; i+=2)
            {
                if (i % 4 <= 1)
                    statsA.AddSample(buffer[i]);
                else
                    statsB.AddSample(buffer[i]);

                if ((i + 1) % 4 <= 1)
                    statsC.AddSample(buffer[i]);
                else
                    statsD.AddSample(buffer[i]);
            }

            float maxX = 1e-5f;
            maxX = Math.Max(statsA.Max, statsB.Max);
            maxX = Math.Max(maxX, statsC.Max);
            maxX = Math.Max(maxX, statsD.Max);

            LeftBound = 0;
            RightBound = maxX*1.5f;
            BottomBound = 0;
            TopBound = Math.Max(Util.GaussianMax(statsA.StandardDeviation), Util.GaussianMax(statsB.StandardDeviation));
            TopBound = Math.Max(TopBound, Util.GaussianMax(statsC.StandardDeviation));
            TopBound = Math.Max(TopBound, Util.GaussianMax(statsD.StandardDeviation));

            if (TopBound < 0.001f)
                TopBound = 0.001f;
            if (Single.IsInfinity(TopBound))
                TopBound = 1.0f;


            Clear();
            Invalidate();
            DrawGaussian(offByOne ? Color.Blue : Color.Red, statsA.Mean, statsA.StandardDeviation, RightBound);
            DrawGaussian(offByOne ? Color.Blue : Color.Red, statsB.Mean, statsB.StandardDeviation, RightBound);
            DrawGaussian(offByOne ? Color.Red : Color.Blue, statsC.Mean, statsC.StandardDeviation, RightBound);
            DrawGaussian(offByOne ? Color.Red : Color.Blue, statsD.Mean, statsD.StandardDeviation, RightBound);

            sw.Flush();
             * */
        }


    }
}
