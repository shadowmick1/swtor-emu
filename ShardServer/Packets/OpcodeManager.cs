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
using System.Xml.Serialization;
using System.IO;

namespace ShardServer.Packets
{
    public class OpcodeManager
    {
        static OpcodeManager _instance;
        public static OpcodeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OpcodeManager();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        OpcodeList _opcodeList;

        public void Initialize()
        {
            Commons.TORLog.Info("Loading Opcodes ...");
            XmlSerializer ser = new XmlSerializer(typeof(OpcodeList));
            _opcodeList = ser.Deserialize(new FileStream("Packets\\Opcodes.xml", FileMode.Open)) as OpcodeList;
        }

        public byte GetOpcode(string packetName)
        {
            PacketOpcodeEntry entry = _opcodeList.Server.Packets.Find(poe => poe.Name == packetName);
            if (entry == null) return 0;
            return entry.Opcode;
        }

        public uint GetPacketID(string packetName)
        {
            PacketOpcodeEntry entry = _opcodeList.Server.Packets.Find(poe => poe.Name == packetName);
            if (entry == null) return 0;
            return entry.ID;
        }

        public string GetPacketName(byte opcode, uint id)
        {
            PacketOpcodeEntry entry = _opcodeList.Client.Packets.Find(poe => poe.ID == id && poe.Opcode == opcode);
            if (entry == null) return "";
            return entry.Name;
        }

    }

    [XmlRoot("Opcodes")]
    public class OpcodeList
    {
        [XmlElement("Client")]
        public OpcodeListCategory Client;
        [XmlElement("Server")]
        public OpcodeListCategory Server;
    }

    public class OpcodeListCategory
    {
        [XmlElement("Packet")]
        public List<PacketOpcodeEntry> Packets;
    }

    public class PacketOpcodeEntry
    {
        [XmlAttribute("ID")]
        public string HexID;

        uint _id = 0;
        public uint ID
        {
            get
            {
                if(_id == 0) _id = uint.Parse(HexID.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                return _id;
            }
        }

        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Opcode")]
        public byte Opcode = 0;
    }

}
