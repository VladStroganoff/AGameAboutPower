using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class ByteBuffer : IDisposable
    {
        private List<byte> Buffer;
        private byte[] ReadBuffer;
        private int ReadPos;
        private bool BufferUpdated = false;

        public ByteBuffer()
        {
            Buffer = new List<byte>();
            ReadPos = 0;
        }

        public int GetReadPosition()
        {
            return ReadPos;
        }

        public byte[] ToArray()
        {
            return Buffer.ToArray();
        }

        public int Count()
        {
            return Buffer.Count;
        }

        public int Length()
        {
            return Count() - ReadPos;
        }

        public void Clear()
        {
            Buffer.Clear();
            ReadPos = 0;
        }

        public void WriteByte(byte input)
        {
            Buffer.Add(input);
            BufferUpdated = true;
        }
        public void WriteBytes(byte[] input)
        {
            Buffer.AddRange(input);
            BufferUpdated = true;
        }
        public void WriteShort(short input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }
        public void WriteInt(int input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }
        public void WriteLong(long input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }
        public void WriteFloat(float input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }
        public void WriteBool(bool input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input));
            BufferUpdated = true;
        }
        public void WriteString(string input)
        {
            Buffer.AddRange(BitConverter.GetBytes(input.Length));
            Buffer.AddRange(Encoding.ASCII.GetBytes(input));
            BufferUpdated = true;
        }

        public byte ReadByte(bool peek = true)
        {
            if (Buffer.Count > ReadPos)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                byte value = ReadBuffer[ReadPos];

                if (peek && Buffer.Count > ReadPos)
                {
                    ReadPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'BYTE'");
            }
        }
        public byte[] ReadByteArray(int Length, bool peek = true)
        {
            if (Buffer.Count > ReadPos)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                byte[] value = Buffer.GetRange(ReadPos, Length).ToArray();

                if (peek && Buffer.Count > ReadPos)
                {
                    ReadPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'BYTE[]'");
            }
        }
        public short ReadShort(bool peek = true)
        {
            if (Buffer.Count > ReadPos)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                short value = BitConverter.ToInt16(ReadBuffer, ReadPos);

                if (peek && Buffer.Count > ReadPos)
                {
                    ReadPos += 2;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'SHORT'");
            }
        }
        public int ReadInt(bool peek = true)
        {
            if (Buffer.Count > ReadPos)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                int value = BitConverter.ToInt32(ReadBuffer, ReadPos);

                if (peek && Buffer.Count > ReadPos)
                {
                    ReadPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'INT'");
            }
        }
        public long ReadLong(bool peek = true)
        {
            if (Buffer.Count > ReadPos)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                long value = BitConverter.ToInt64(ReadBuffer, ReadPos);

                if (peek && Buffer.Count > ReadPos)
                {
                    ReadPos += 8;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'LONG'");
            }
        }
        public float ReadFloat(bool peek = true)
        {
            if (Buffer.Count > ReadPos)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                float value = BitConverter.ToSingle(ReadBuffer, ReadPos);

                if (peek && Buffer.Count > ReadPos)
                {
                    ReadPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'FLOAT'");
            }
        }
        public bool ReadBool(bool peek = true)
        {
            if (Buffer.Count > ReadPos)
            {
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = false;
                }

                bool value = BitConverter.ToBoolean(ReadBuffer, ReadPos);

                if (peek && Buffer.Count > ReadPos)
                {
                    ReadPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'BOOL'");
            }
        }
        public string ReadString(bool peek = true)
        {

            try
            {
                int length = ReadInt(true);
                if (BufferUpdated)
                {
                    ReadBuffer = Buffer.ToArray();
                    BufferUpdated = true;
                }

                string value = Encoding.ASCII.GetString(ReadBuffer, ReadPos, length);

                if (peek && Buffer.Count > ReadPos)
                {
                    if (value.Length > 0)
                        ReadPos += length;
                }

                return value;
            }
            catch (Exception)
            {
                throw new Exception("You are not trying to read out a 'STRING'");
            }
        }

        private bool disposeValue;
        protected virtual void Dispose(bool sidposing)
        {
            if (!sidposing)
            {
                if (sidposing)
                {
                    Buffer.Clear();
                    ReadPos = 0;
                }
                disposeValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
