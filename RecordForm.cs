using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX.Multimedia;
using SlimDX.DirectSound;
using System.IO;

namespace BangaiO
{

    public partial class RecordForm : Form
    {
        public class DeviceItem
        {
            public DeviceInformation DeviceInformation;

            public DeviceItem(DeviceInformation info)
            {
                this.DeviceInformation = info;
            }

            public override string ToString()
            {
                return DeviceInformation.Description;
            }
        }


        private WaveCapture waveCap;
        private PowerMeter powerMeter;
        private FrontEnd frontEnd;
        private Decider2 decider1;
        private Decider2 decider2;
        private Buffer<float> phases = new Buffer<float>(5);

        private bool recording = false;

        public RecordForm()
        {
            InitializeComponent();
            foreach (DeviceInformation info in DirectSoundCapture.GetDevices())
                deviceCombo.Items.Add(new DeviceItem(info));

            deviceCombo.SelectedIndex = 0;

            dbLabel.Text = "";
        }

        private void StopRecording()
        {
            if (!recording)
                return;
            waveCap.Stop();
            waveCap.Dispose();
            waveCap = null;
            recording = false;

            decider1.Finish();
            decider2.Finish();

            phasePlot2.Clear();
            eyePlot1.Clear();
            //probabilityPlot1.Clear();
            deviceCombo.Enabled = true;
            vuMeter1.Percent = 0;
            dbLabel.Text = "";
        }

        private void StartRecording()
        {
            if (recording)
                return;
            Guid guid = ((DeviceItem)deviceCombo.SelectedItem).DeviceInformation.DriverGuid;

            const int Fs = 44100;

            waveCap = new WaveCapture(guid, Fs, 32 * 1024, 50);

            frontEnd = new FrontEnd(Fs, 1022.7);
            //frontEnd.OutputBuffer = signalDetector1.InputBuffer;
            frontEnd.PhaseOutputBuffer = new Buffer<float>(1);
            frontEnd.PhaseOutputBuffer.BufferFilled += new Buffer<float>.BufferFilledHandler(PhaseOutputBuffer_BufferFilled);
            frontEnd.EyeOutputBuffer = eyePlot1.InputBuffer;

            powerMeter = new PowerMeter(Util.CalcBufferSize(0.1f, Fs));

            Splitter<float> splitter = new Splitter<float>(1024);
            splitter.OutputBuffers.Add(frontEnd.InputBuffer);
            splitter.OutputBuffers.Add(powerMeter.InputBuffer);

            //signalDetector1.OutputBuffer = probabilityPlot1.InputBuffer;
            //signalDetector1.OutputBuffer = histogram1.InputBuffer;

            UnitOffset<float> offset = new UnitOffset<float>(256);
            BitCombiner bc1 = new BitCombiner(256);
            BitCombiner bc2 = new BitCombiner(256);

            Splitter<float> splitter2 = new Splitter<float>(256);
            frontEnd.OutputBuffer = splitter2.InputBuffer;

            //signalDetector1.OutputBuffer = splitter2.InputBuffer;
            splitter2.OutputBuffers.Add(bc1.InputBuffer);
            splitter2.OutputBuffers.Add(offset.InputBuffer);
            offset.OutputBuffer = bc2.InputBuffer;
            //bc1.OutputBuffer = histogram1.InputBuffer;
            //bc2.OutputBuffer = histogram2.InputBuffer;

            decider1 = new Decider2(1);
            decider2 = new Decider2(2);

            bc1.OutputBuffer = decider1.InputBuffer;
            bc2.OutputBuffer = decider2.InputBuffer;

            waveCap.OutputBuffer = splitter.InputBuffer;

            powerMeter.PowerUpdated += new PowerMeter.PowerUpdatedHandler(powerMeter_PowerUpdated);


            signalDetector1.Reset();
            phasePlot2.Clear();
            phasePlot2.Reset();
            eyePlot1.Clear();
            //probabilityPlot1.Clear();
            //probabilityPlot1.Reset();

            waveCap.Start();
            deviceCombo.Enabled = false;

            recording = true;
        }

        void PhaseOutputBuffer_BufferFilled(float[] buffer, int bufSize)
        {
            for (int i = 0; i < bufSize; ++i)
                phasePlot2.AddPhase(buffer[i]);
        }

        void powerMeter_PowerUpdated(float db)
        {
            if (!recording)
                return;
            dbLabel.Text = String.Format("{0:0} dB", db);
            vuMeter1.Color = Color.FromArgb(0, 255, 0);
            vuMeter1.Percent = (db + 60.0f) / 60.0f;
        }

        private void recordBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (recordBtn.Checked)
                StartRecording();
            else
                StopRecording();
        }

        private void signalDetector1_Click(object sender, EventArgs e)
        {

        }

    }
}
