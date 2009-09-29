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
        private Decider decider1;
        private Decider decider2;
        private InputPin<float> phases = new InputPin<float>(5);

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

            powerMeter = new PowerMeter(Util.CalcBufferSize(0.1f, Fs));
            powerMeter.PowerUpdated += new PowerMeter.PowerUpdatedHandler(powerMeter_PowerUpdated);

            waveCap = new WaveCapture(guid, Fs, 32 * 1024, 50);
            frontEnd = new FrontEnd(Fs, 1022.76);
            UnitOffset<float> offset = new UnitOffset<float>();
            BitCombiner combiner1 = new BitCombiner();
            BitCombiner combiner2 = new BitCombiner();
            decider1 = new Decider(1);
            decider2 = new Decider(2);
            FirstOneWins<bool> firstOneWins = new FirstOneWins<bool>();
            ConvertToBytes convertToBytes = new ConvertToBytes();
            Progress progress = new Progress();

            // set up capture outputs
            waveCap.Output.Connect(frontEnd.Input);
            waveCap.Output.Connect(powerMeter.Input);

            // set up front-end outputs
            InputPin<float> phasePin = new InputPin<float>();
            frontEnd.PhaseOutput.Connect(phasePin);
            phasePin.BufferFilled += new InputPin<float>.BufferFilledHandler(PhaseOutputBuffer_BufferFilled);
            frontEnd.EyeOutput.Connect(eyePlot1.Input);
            frontEnd.Output.Connect(combiner1.Input);
            frontEnd.Output.Connect(offset.Input);
            offset.Output.Connect(combiner2.Input);

            // Set up combiner outputs
            combiner1.Output.Connect(decider1.Input);
            combiner2.Output.Connect(decider2.Input);

            // Set up decider outputs
            decider1.Output.Connect(firstOneWins.Input1);
            decider2.Output.Connect(firstOneWins.Input2);

            firstOneWins.Output.Connect(convertToBytes.Input);

            convertToBytes.Output.Connect(progress.Input);

            // TODO: Look into reset signal crap...

            phasePlot2.Clear();
            phasePlot2.Reset();
            eyePlot1.Clear();

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

    }
}
