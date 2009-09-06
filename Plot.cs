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
    public partial class Plot : Control
    {
        private bool initialized = false;

        private Bitmap backBuf;
        private Graphics backBufGraphics;
        public float LeftBound = 0.0f;
        public float RightBound = 1.0f;
        public float TopBound = 0.0f;
        public float BottomBound = 1.0f;
        public bool AutoInvalidate = true;

        public Plot()
        {
            InitializeComponent();
        }

        protected virtual void Initialize()
        {
            if (initialized)
                return;
            backBuf = new Bitmap(ClientSize.Width, ClientSize.Height);
            backBufGraphics = Graphics.FromImage(backBuf);
            initialized = true;
            Clear();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
            return;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Initialize();
            pe.Graphics.DrawImage(backBuf, 0, 0);
            base.OnPaint(pe);
        }

        private int ProjectX(float x)
        {
            return (int)Math.Round((x-LeftBound)/(RightBound-LeftBound) * ClientSize.Width);
        }

        private int ProjectY(float y)
        {
            return (int)Math.Round((y - TopBound) / (BottomBound - TopBound) * ClientSize.Height);
        }

        public void DrawPoint(Color col, float x, float y)
        {
            Initialize();
            int xp = ProjectX(x);
            int yp = ProjectY(y);
            backBufGraphics.FillRectangle(new SolidBrush(col), xp, yp, 1, 1);
            if(AutoInvalidate)
                Invalidate(new Rectangle(xp - 1, yp - 1, 3, 3));
        }

        public void DrawLine(Color col, float x1, float y1, float x2, float y2)
        {
            DrawLine(new Pen(col), x1, y1, x2, y2);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            if (Single.IsNaN(y1) || Single.IsNaN(y2))
                return;

            Initialize();
            int xp1 = ProjectX(x1);
            int yp1 = ProjectY(y1);
            int xp2 = ProjectX(x2);
            int yp2 = ProjectY(y2);

            backBufGraphics.DrawLine(pen, xp1, yp1, xp2, yp2);

            if (AutoInvalidate)
            {
                //Rectangle rect = new Rectangle(xp1, yp1);
                //rect
                int minX = xp1 < xp2 ? xp1 : xp2;
                int maxX = xp1 > xp2 ? xp1 : xp2;
                int minY = yp1 < yp2 ? yp1 : yp2;
                int maxY = yp1 > yp2 ? yp1 : yp2;
                Invalidate(new Rectangle(minX - 1, minY - 1, maxX - minX + 2, maxY - minY + 2));
            }
        }

        public void Clear()
        {
            Clear(Color.Black);
        }

        public void Clear(Color col)
        {
            Initialize();
            backBufGraphics.Clear(col);
            Invalidate();
        }
    }
}
