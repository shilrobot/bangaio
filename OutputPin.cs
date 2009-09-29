using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BangaiO
{
    // Allows multiple inputs connected to one output, or zero inputs connected to one output -- simplifies code.
    public class OutputPin<T>
    {
        private List<InputPin<T>> buffers = new List<InputPin<T>>();
        private T[] oneElem = new T[1];

        public void Connect(InputPin<T> buf)
        {
            if (buf == null)
                throw new ArgumentNullException();
            buffers.Add(buf);
        }

        public void Disconnect(InputPin<T> buf)
        {
            buffers.Remove(buf);
        }

        public void Write(T t)
        {
            oneElem[0] = t;
            Write(oneElem, 0, 1);
        }

        public void Write(T[] data)
        {
            Write(data, 0, data.Length);
        }

        public void Write(T[] data, int count)
        {
            Write(data, 0, count);
        }

        public void Write(T[] data, int offset, int count)
        {
            foreach (InputPin<T> buf in buffers)
                buf.Write(data, offset, count);
        }
    }
}
