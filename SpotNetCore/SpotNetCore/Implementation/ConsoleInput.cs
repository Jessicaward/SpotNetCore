using System;
using System.Text;

namespace SpotNetCore.Implementation
{
    public class ConsoleInput
    {
        private StringBuilder Buffer { get; } = new StringBuilder();
        public int CurrentIndex { get; set; }

        public ConsoleInput Append(string str)
        {
            Buffer.Append(str);
            CurrentIndex += str.Length;
            return this;
        }

        public ConsoleInput Append(char c)
        {
            Buffer.Append(c);
            CurrentIndex++;
            return this;
        }

        public ConsoleInput Insert(char c, int index = -1) => Insert(c.ToString(), index);

        public ConsoleInput Insert(String str, int index = -1)
        {
            if (index == -1)
            {
                index = CurrentIndex;
            }

            Buffer.Insert(index, str);
            CurrentIndex = index + str.Length;
            return this;
        }

        public ConsoleInput Clear()
        {
            Buffer.Clear();
            CurrentIndex = 0;
            return this;
        }

        public ConsoleInput Remove(int offset, int count)
        {
            Buffer.Remove(offset, count);
            CurrentIndex = offset - count + 1;
            return this;
        }

        public override string ToString()
        {
            return Buffer.ToString();
        }

        public static implicit operator string(ConsoleInput consoleInput)
        {
            return consoleInput.ToString();
        }

        public int Length => Buffer.Length;
    }
}