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
    public partial class PhasePlot : Plot
    {
        private int i = 0;

        public PhasePlot()
        {
            InitializeComponent();
            LeftBound = 0;
            TopBound = (float)(Math.PI);
            BottomBound = -(float)(Math.PI);
        }

        public void Reset()
        {
            i = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
            RightBound = ClientSize.Width; 
        }

        public void AddPhase(float phase)
        {
            DrawPoint(Color.FromArgb(0,255,0), i, phase);
            i++;
            if (i >= ClientSize.Width)
            {
                Clear();
                i = 0;
            }
        }
    }
}
