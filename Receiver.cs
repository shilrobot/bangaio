using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BangaiO
{
    public class ReceiverSettings
    {
        // TODO: Other stuff here.
        public float SamplingFrequency = 44100.0f;
        public float CarrierFrequency = 1022.7f;
        public InputPin<float> PhaseOutput;
        public InputPin<PointF> EyeDiagramOutput;
    }
    
    public enum TransferState
    {
        InProgress,
        Completed,
        Failed
    }

    public delegate void TransferStateChangedHandler(object sender, TransferState newState);

    public interface ITransfer
    {
        int SizeInBytes { get; }
        int BytesTransferred { get; }
        TransferState State { get; }
        event TransferStateChangedHandler StateChanged;
        byte[] GetBytes();
    }

    public delegate void TransferStartedHandler(object sender, ITransfer t);

    public class Receiver
    {
        public event TransferStartedHandler TransferStarted;


        private ReceiverSettings settings;
        private FrontEnd frontEnd;

        public Receiver(ReceiverSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (settings.SamplingFrequency <= 0.0f)
                throw new ArgumentOutOfRangeException("settings", "Settings request an invalid sampling frequency. Sampling frequency must be positive.");
            if (settings.CarrierFrequency <= 0.0f)
                throw new ArgumentOutOfRangeException("settings", "Settings request an invalid carrier frequency. Carrier frequency must be positive.");
            this.settings = settings;
            this.frontEnd = new FrontEnd(settings.SamplingFrequency, settings.CarrierFrequency);

        }

        public void ProcessData(float[] data, int offset, int count)
        {
        }

        // flushes remaining data through the pipeline
        public void Finish()
        {
        }
    }
}
