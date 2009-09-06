using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BangaiO
{
    public partial class EyePlot : Plot
    {
        private Pen grayPen =new Pen(Color.FromArgb(128, 128, 128));
        private Pen darkGrayPen =new Pen(Color.FromArgb(80, 80, 80));
        private Pen greenPen =new Pen(Color.FromArgb(0, 255, 0));

        public Buffer<PointF> InputBuffer = new Buffer<PointF>(4096);

        public EyePlot()
        {
            InitializeComponent();

            LeftBound = 0;
            RightBound = (float)(2 * Math.PI);
            TopBound = 2.5f * 1.2f;
            BottomBound = -2.5f * 1.2f;

            InputBuffer.BufferFilled += new Buffer<PointF>.BufferFilledHandler(InputBuffer_BufferFilled);

            this.AutoInvalidate = false;
        }

        void InputBuffer_BufferFilled(PointF[] buffer, int bufSize)
        {
            ClearGrid();
            float lastY = 0;
            float lastX = 999.0f;

            float maxY = 0;
            for (int i = 0; i < bufSize; ++i)
                maxY = Math.Max(Math.Abs(buffer[i].Y), maxY);

            float scaleY = 2.5f / maxY;

            for (int i = 0; i < bufSize; ++i)
            {
                if (buffer[i].X > lastX)
                    DrawLine(lastX, lastY, buffer[i].X, buffer[i].Y*scaleY);

                lastX = buffer[i].X;
                lastY = buffer[i].Y * scaleY;
            }
            Invalidate();
        }

        public void ClearGrid()
        {
            Clear(Color.Black);

            Color darkGray = Color.FromArgb(80,80,80);
            Color gray = Color.FromArgb(128,128,128);


            DrawLine(darkGray, 0, 1, 10, 1);
            DrawLine(darkGray, 0, -1, 10, -1);
            DrawLine(darkGray, 0, 2.5f, 10, 2.5f);
            DrawLine(darkGray, 0, -2.5f, 10, -2.5f);

            DrawLine(gray, 0, 0, 10, 0);
            DrawLine(gray, (float)Math.PI * 2 * 0.25f, -10, (float)Math.PI * 2 * 0.25f, 10);
            DrawLine(gray, (float)Math.PI * 2 * 0.5f, -10, (float)Math.PI * 2 * 0.5f, 10);
            DrawLine(gray, (float)Math.PI * 2 * 0.75f, -10, (float)Math.PI * 2 * 0.75f, 10);
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            DrawLine(greenPen, x1, y1, x2, y2);
        }
    }
}
