using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Multimedia;
using SlimDX.DirectSound;
using System.Windows.Forms;
using System.IO;

namespace BangaiO
{
    public class WaveCapture : IDisposable
    {
        private enum State
        {
            Initial,
            Recording,
            Done
        }

        public Buffer<float> OutputBuffer;

        private DirectSoundCapture capture;
        private CaptureBuffer captureBuffer;

        private State state = State.Initial;
        private Timer timer;
        private int lastReadPos = 0;
        private int whichByte = 0;
        private byte[] buf;
        private byte lowByte = 0;
        private bool disposed = false;
        
        // bufferSize is a SAMPLE COUNT
        // NOTE: we always capture 16 bits/sample
        public WaveCapture(Guid deviceGuid, int Fs, int bufferSize, int timerInterval)
        {
            CaptureBufferDescription desc = new CaptureBufferDescription();
            desc.BufferBytes = bufferSize * 2;
            desc.ControlEffects = false;
            desc.WaveMapped = true;
            desc.Format = new WaveFormat();
            desc.Format.FormatTag = SlimDX.WaveFormatTag.Pcm;
            desc.Format.SamplesPerSecond = Fs;
            desc.Format.Channels = 1;
            desc.Format.BitsPerSample = 16;
            desc.Format.BlockAlignment = 2;
            desc.Format.AverageBytesPerSecond = Fs * 2;

            buf = new byte[bufferSize * 2];

            capture = new DirectSoundCapture(deviceGuid);
            captureBuffer = new CaptureBuffer(capture, desc);

            timer = new Timer();
            timer.Interval = timerInterval;
            timer.Tick += new EventHandler(timer_Tick);
        }

        ~WaveCapture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (captureBuffer != null)
                captureBuffer.Dispose();
            if (capture != null)
                capture.Dispose();

            if (disposing)
                GC.SuppressFinalize(this);

            disposed = true;
        }

        private void CaptureRange(int start, int end)
        {
            int count = end - start;
            captureBuffer.Read(buf, 0, count, start, false);

            const float invMax = 1.0f / 32768.0f;

            for (int i = 0; i < count; ++i)
            {
                if(whichByte == 0)
                {
                    lowByte = buf[i];
                    whichByte = 1;
                }
                else
                {
                    byte hiByte = buf[i];
                    short signed16 = (short)((hiByte << 8) | lowByte);
                    OutputBuffer.Write(signed16*invMax);
                    whichByte = 0;
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (state != State.Recording)
                return;

            int currReadPos = captureBuffer.CurrentReadPosition;
            if (currReadPos > lastReadPos)
            {
                CaptureRange(lastReadPos, currReadPos);
            }
            else if (currReadPos < lastReadPos)
            {
                CaptureRange(lastReadPos, captureBuffer.SizeInBytes);
                CaptureRange(0, currReadPos);
            }

            lastReadPos = currReadPos;
        }

        public void Start()
        {
            if (state != State.Initial)
                throw new InvalidOperationException("Cannot start recording if not in Initial state");
            timer.Start();
            captureBuffer.Start(true);
            state = State.Recording;
        }

        public void Stop()
        {
            if(state != State.Recording)
                throw new InvalidOperationException("Cannot stop recording if not in Recording state");
            // TODO: Read the rest?
            captureBuffer.Stop();
            timer.Stop();
            state = State.Done;
        }
    }
}
