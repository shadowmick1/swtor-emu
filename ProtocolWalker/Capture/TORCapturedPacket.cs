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
using PacketDotNet;
using System.Windows;
using System.Xml.Serialization;

namespace ProtocolWalker.Capture
{
    public class TORCapturedPacket
    {
        TORPacketType _type;
        public TORPacketType Type
        {
            get { return _type; }
        }

        byte[] _data;
        public byte[] Data
        {
            get { return _data; }
        }

        int _id;
        public int ID
        {
            get { return _id; }
        }
        CaptureSession _session;
        public CaptureSession Session
        {
            get { return _session; }
        }
        PacketDirection _direction;
        public PacketDirection Direction
        {
            get { return _direction; }
        }

        public int Length
        {
            get { return _data.Length; }
        }

        bool serverPacket;

        public string Name = "unknown";        

        public TORCapturedPacket(int id, TORPacketType type, byte[] data, CaptureSession session, PacketDirection direction)
        {
            _id = id;
            _type = type;
            _data = data;
            _session = session;
            _direction = direction;
        }
    }

    public enum TORPacketType
    {
        Proxy,
        Shard
    }

    public enum PacketDirection
    {
        Client2Server,
        Server2Client
    }

}
