/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Commons.Networking
{
    /// <summary>
    /// Custom implementation of MemoryStream for writing LE/BE
    /// </summary>
    public class ByteBuffer : MemoryStream
    {
        public readonly ByteOrder ByteOrder;

        public ByteBuffer(ByteOrder order) : base()
        {
            this.ByteOrder = order;
        }

        public ByteBuffer(ByteOrder order, byte[] buffer) : base(buffer)
        {
            this.ByteOrder = order;
        }

        public ByteBuffer(ByteOrder order, byte[] buffer, int offset, int length) : base(buffer, offset, length)
        {
            this.ByteOrder = order;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] result = new byte[length];
            Read(result, 0, length);
            return result;
        }

        public short ReadShort()
        {
            byte[] values = ReadBytes(2);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(values);
            return BitConverter.ToInt16(values, 0);
        }

        public ushort ReadUShort()
        {
            byte[] values = ReadBytes(2);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(values);
            return BitConverter.ToUInt16(values, 0);
        }

        public int ReadInt()
        {
            byte[] values = ReadBytes(4);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(values);
            return BitConverter.ToInt32(values, 0);
        }

        public uint ReadUInt()
        {
            byte[] values = ReadBytes(4);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(values);
            return BitConverter.ToUInt32(values, 0);
        }

        public uint ReadObjectIDRev()
        {
            byte[] values1 = ReadBytes(2);
            byte[] values2 = ReadBytes(2);
            byte[] result = new byte[4];
            Array.Copy(values2, 0, result, 0, 2);
            Array.Copy(values1, 0, result, 2, 2);
            return BitConverter.ToUInt32(result, 0);
        }

        public string ReadString()
        {
            int length = ReadInt();
            string result = "";
            for (int i = 0; i < length; i++)
            {
                result += (char)ReadByte();
            }
            return result.Replace("\x0", "");
        }

        public void WriteBytes(byte[] data)
        {
            Write(data, 0, data.Length);
        }

        public void WriteShort(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(bytes);
            WriteBytes(bytes);
        }

        public void WriteUShort(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(bytes);
            WriteBytes(bytes);
        }

        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(bytes);
            WriteBytes(bytes);
        }

        public void WriteUInt(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(bytes);
            WriteBytes(bytes);
        }

        public void WriteLong(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (ByteOrder == Networking.ByteOrder.BigEndian) Array.Reverse(bytes);
            WriteBytes(bytes);
        }

        public void WriteULongRev(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            WriteBytes(bytes);
        }

        public void WriteString(string value)
        {
            WriteInt(value.Length+1);
            foreach (char c in value)
            {
                WriteByte((byte)c);
            }
            WriteByte(0);
        }

    }

    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }

}
