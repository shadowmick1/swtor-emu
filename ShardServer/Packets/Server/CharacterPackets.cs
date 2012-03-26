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
using ShardServer.TORBusiness.Model;
using Commons;
using ShardServer.TORBusiness.Data;
using Commons.Networking;

namespace ShardServer.Packets.Server
{
    public class SMsg_CharacterListResponse : IServerPacket
    {
        uint _id;
        public SMsg_CharacterListResponse(uint objid)
        {
            _id = objid;
        }

        void WriteCharacterInfo(ByteBuffer packet, Character cha, byte add)
        {
            packet.WriteBytes(new byte[] { (byte)(0x86 + add), 0x00, 0x00, 0x40 }); // Dynamic
            packet.WriteUInt(0x4e7511aa); // Static
            packet.WriteUInt(0x899);
            packet.WriteByte(0x80);

            packet.WriteBytes(new byte[] { 
                0xe8, 0x7e, 0x20, 0x9b, // E8 7E 20 9B
                (byte)(0x86 + add), 0x00, 0x00, 0x40, // 4F 00 00 40
                0xe8, 0x7e, 0x20, 0x9b, // E8 7E 20 9B
                (byte)(0x86 + add), 0x00, 0x00, 0x40, // 4F 00 00 40
            });

            packet.WriteBytes(new byte[] { 0x05, 0x00, 0x02 });

            long detailsLengthPlaceholder = packet.Position;
            packet.WriteInt(0); // placeholder for details length

            packet.WriteBytes(new byte[] { 
                0x14, 0x14, 0xCF, 0x40, 
                0x00, 0x00, 0x04, 0xE0, 
                0xD7, 0x68, 0xB5, 0x03,
                0x01, 0xCC, 0x07, 0xD8, 
                0x87, 0xD6, 0x02, 0x03, 
                0x00, 0xCC, 0x29, 0x40, 
                0x56, 0x77, 0x5B, 0x03,
                0x01, 0xC4, 0x30, 0x1A, 
                0x0D, 0x75, 0xB9, 0x15, 
                0xCD, 0x0B,
            }); // static          

            packet.WriteBytes(new byte[] { 0xcc, 0xbd, 0x33, 0x9c, 0xdd }); // unk dynamic (CC BD 33 9C DD)

            packet.WriteBytes(new byte[] { 
                0xCC, 0x07, 0x18, 0xC2, 
                0x55, 0x7C, 0x09, 0x04, 
                0x04, 0xCF, 0x40, 0x00, 
                0x00, 0x30, 0x5C, 0x21, 
                0xB1, 0x10, 0x02 }); // static

            packet.WriteBytes(new byte[] {
            //0x95, 0x01, 0x02,
            //0xC9, 0x02, 0x8A, 0x01, 0x02, 
            //0xC9, 0x02, 0x8C, 0x01, 0x02, 0x9C, // C9 02 FC 01 02 8A 01 02 C9 02 F5 01 02 C9 03 08 
            0xC9, 0x02, 0xFC, 0x01, 0x02, 0x8A, 0x01, 0x02,
            0xC9, 0x02, 0xF5, 0x01, 0x02, 0xC9, 0x03, 0x08,
            
            0xCB, 0x91, 0xFC, 0x05, 
            0xB8, 0x02, 
            0x06, 0xCC, 0x27, 0x32, 0x2B, // static
            0x0D, 0xFD, 0x02, 0xCA, // static
            0x76, // static

            0xc4, 0x7f, // dynamic
            
            0xCB, 0x50, 0x5D, 0x6C, 0xC6, // static
            
            0x04, 0x33, 0x83, 0xc5, // static
            0x44, // static
            0x01, // static
            0x04, 0x9a, 0xdd, 0xdc, // static
            0x45, 
            0xC4, 0x25, 0x42, 0x9A, // Static
            0x25, 0xBA, 0x02, 0xCF }); // Static

            packet.WriteUInt(cha.APP1);
            packet.WriteUInt(cha.APP2);
            packet.WriteUInt(cha.APP3);
            packet.WriteUShort(cha.APP4);
            packet.WriteByte(0xCF);

            /*packet.WriteBytes(new byte[] {
                0x56, 0xF6, 0x33, 0xF2, //0E B2 4F F8
                0xDD, 0xDE, 0x74, 0x77, //51 1E 46 3C
                0xCB, 0x6C, 0x9C, 0xB1, //CB 6C 9C B1
                0xA7, 0x01, 0xCF,
                //0xE0, 0x00, 0xFD, 0x35, 0xB1, 0x0B, 0x17, 0x60 //9B E1 60
            });*/ // appearance ? from char creation

            //packet.WriteBytes(BitConverter.GetBytes((ulong)CharacterClass.JediConsular));
            packet.WriteULongRev((ulong)cha.Class);

            packet.WriteBytes(new byte[] { 0xCC, 0x25, 0xEB, 0x85, 
            0x46, 0x42, 0x03, 0x01, // 0 = 1
            0xC4, 0x25, 0xD0, 0x6C, 0xA1, 
                0x13, 0x07, 
            });

            // Inventory
            ulong[] ids = new ulong[] { 0xe000e32eacf6fe1c, 0xe00096830962E510, 0xE0002D97FFB1222D };
            packet.WriteByte(1);
            packet.WriteByte((byte)ids.Count());
            packet.WriteByte((byte)ids.Count());
            packet.WriteByte(1);
            packet.WriteByte(0xcf);

            int ctr = 0;
            foreach (ulong id in ids)
            {
                packet.WriteULongRev(id);
                if (ctr == 0) packet.WriteByte(0x02);
                else if (ctr == 1) packet.WriteByte(0x03);
                else packet.WriteShort(0x211);
                packet.WriteByte(0xcf);
                ctr++;
            }
            // End Inventory

            packet.WriteBytes(new byte[] { 
                0x15, 0xE2, 
                0x38, // 2D
                0x19, 0xB0, 0x9F, 
                0xDB, 0x62, // C8 B1
                0xCC, 0x02, 0xEE, 0xCD, 0xCF, 0x87, 0x08, 0x00, }); // Static

            // Appearance
            packet.WriteByte(2);
            packet.WriteByte((byte)cha.Appearance.Length);
            packet.WriteByte((byte)cha.Appearance.Length);

            //cha.Appearance.OrderBy(tuple => tuple.Item1);

            // 01 05 07 0A 0B 0C 0E 10
            byte counter = 0;
            for (byte b = 0; b < cha.Appearance.Length; b++)
            {
                Tuple<byte, byte> tuple = cha.Appearance[b];
                packet.WriteByte(0xD2);
                if (tuple.Item1 < 0x0A)
                {
                    packet.WriteByte(0x01);
                    packet.WriteByte((byte)(0x30 + tuple.Item1));
                }
                else
                {
                    packet.WriteByte(0x02);
                    packet.WriteByte(0x31);
                    packet.WriteByte((byte)(0x30 + (tuple.Item1 - 0x0A)));
                }
                packet.WriteByte(0xc9);
                packet.WriteByte(0x04);
                packet.WriteByte(tuple.Item2);
                counter++;
            }
            packet.WriteByte(0xcf);

            // End Appearance

            /*packet.WriteBytes(new byte[] {                
                0x02, 0x08, 0x08, 
                0xD2, 0x01, 0x31, 0xC9, 0x04, 0x5D,
                0xD2, 0x01, 0x35, 0xC9, 0x04, 0x8B, 
                0xD2, 0x01, 0x37, 0xC9, 0x04, 0x3D,
                0xD2, 0x02, 0x31, 0x30, 0xC9, 0x04, 0x51,
                0xD2, 0x02, 0x31, 0x31, 0xC9, 0x04, 0x46,
                0xD2, 0x02, 0x31, 0x32, 0xC9, 0x04, 0x83, 
                0xD2, 0x02, 0x31, 0x34, 0xC9, 0x04, 0x20,
                0xD2, 0x02, 0x31, 0x36, 0xC9, 0x04, 0xA8,
                0xCF
            });*/ // list of something

            packet.WriteBytes(new byte[] {
                0x3F, 0xFF, 0xFF, 0xF5, 
                0x58, 0x76, 0x5E, 0x0F, 0x01, 0xCF, 
                0x40, 0x00, 0x00, (byte)(0x86 + add), 
                0x9b, 0x20, 0x7f, 0xa5, // 9B 20 7F A5
                0x01, 0x06
            });

            // Character Name
            packet.WriteByte((byte)cha.Name.Length);
            packet.WriteBytes(Encoding.UTF8.GetBytes(cha.Name));

            packet.WriteShort(0x202); // Static

            packet.WriteByte(cha.Level); // Character Level

            packet.WriteShort(0x103);

            packet.WriteByte(0xcf); // Separator

            packet.WriteBytes(Utility.Reverse(BitConverter.GetBytes((ulong)cha.AreaSpec))); // Current Map ID

            packet.WriteBytes(new byte[] {
                0xC7, 0x3F, 0xFF, 0xFF, // This is static - probably a separator
                0xD2, 0xA9, 0x55, 0x93, // static
                0x1A, 0x02, 0x01, 0xc7, // static ? or some len/offset data for next char
                0xaf, 0xff, 0xc0, // dynamic unk
            });

            long curPos = packet.Position;
            packet.Position = detailsLengthPlaceholder;

            int value = (int)(curPos - detailsLengthPlaceholder - 8);
            Console.WriteLine("Len = " + value);

            packet.WriteInt((int)(curPos - detailsLengthPlaceholder - 8));
            packet.Position = curPos;
        }

        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(_id);
            packet.WriteInt(5); // character count ?

            packet.WriteUInt(0x4e308655);

            WriteCharacterInfo(packet, new Character() { Name = "Zeus", Level = 99, Class = CharacterClass.SithInquisitor, AreaSpec = MapAreas.PCShip_PhantomX70B }, 0);
            WriteCharacterInfo(packet, new Character() { Name = "LilGreenShit", Level = 99, Class = CharacterClass.JediConsular, AreaSpec = MapAreas.IlumS }, 1);
            WriteCharacterInfo(packet, new Character() { Name = "SuperSayan501", Level = 51, Class = CharacterClass.Trooper, AreaSpec = MapAreas.BelsavisS }, 2);
            WriteCharacterInfo(packet, new Character() { Name = "Tampix", Level = 1, Class = CharacterClass.SithWarrior, AreaSpec = MapAreas.CoruscantR }, 3);
            if (Program.LastCreatedChar != null)
                WriteCharacterInfo(packet, Program.LastCreatedChar, 4);
            else
                WriteCharacterInfo(packet, new Character() { Name = "Bananaaaaa", Level = 8, Class = CharacterClass.BountyHunter, AreaSpec = MapAreas.PCShip_ThunderclapBT7 }, 4);

            string xml = "";
            xml += "<status><userdata><entitlement id=\"30\"    uniqueId=\"539043\"    date_created=\"1311840392836\"    date_consumed =\"\"    ";
            xml += "date_started=\"\" description=\"Early Game Access\" gameInfo=\"\" shard_name=\"he1012\" key=\"\" type=\"G\"/>";

            xml += "<entitlement id=\"39\"    uniqueId=\"15130593\"    date_created=\"1323871237000\"    date_consumed =\"\"    ";
            xml += "date_started=\"\" description=\"Head Start\" gameInfo=\"Created by Head Start Package\" shard_name=\"he1012\" key=\"\" type=\"G\"/>";

            xml += "<entitlement id=\"2013\"    uniqueId=\"19669424\"    date_created=\"1324371113907\"    date_consumed =\"\"    ";
            xml += "date_started=\"\" description=\"SWTOR_COLLECTORS_RETAIL\" gameInfo=\"{&quot;transactionalEmail&quot;: {&quot;templatePrefix&quot;:&quot;PurchaseConfirmation.CR&quot;}}\" shard_name=\"he1012\" key=\"\" type=\"G\"/>";

            xml += "<entitlement id=\"7\"    uniqueId=\"19669425\"    date_created=\"1324371113909\"    date_consumed =\"\"    ";
            xml += "date_started=\"\" description=\"Subcriber Shard Access\" gameInfo=\"\" shard_name=\"he1012\" key=\"\" type=\"G\"/>";

            xml += "<entitlement id=\"40\"    uniqueId=\"22765385\"    date_created=\"1324593971139\"    date_consumed =\"\"    ";
            xml += "date_started=\"\" description=\"Security Key Associated\" gameInfo=\"\" shard_name=\"he1012\" key=\"\" type=\"G\"/>";

            xml += "<entitlement id=\"71\"    uniqueId=\"29277571\"    date_created=\"1327348302784\"    date_consumed =\"\"    ";
            xml += "date_started=\"\" description=\"Early Founder\" gameInfo=\"\" shard_name=\"he1012\" key=\"\" type=\"G\"/></userdata></status>";

            packet.WriteString(xml);
            packet.WriteLong(0);

        }
    }

    public class SMsg_CharacterCreateResponse : IServerPacket
    {
        uint _id;
        string _name;
        byte _responseid;
        public SMsg_CharacterCreateResponse(uint id, string name, byte responseid)
        {
            _id = id;
            _name = name;
            _responseid = responseid;
        }

        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(_id);
            packet.WriteInt(_name.Length);
            packet.WriteBytes(Encoding.UTF8.GetBytes(_name));
            packet.WriteInt(_responseid);
        }
    }

    public class SMsg_CharacterSelectResponse : IServerPacket
    {
        uint _id;
        public SMsg_CharacterSelectResponse(uint objid)
        {
            _id = objid;
        }

        void IServerPacket.WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(_id);
        }
    }

    public class SMsg_CharacterCurrentMap : IServerPacket
    {
        uint _id;
        string _mapname;
        string _mapid;
        public SMsg_CharacterCurrentMap(uint objid, string mapname, string mapid)
        {
            _id = objid;
            _mapname = mapname;
            _mapid = mapid;
        }

        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(_id);
            packet.WriteString(_mapname);
            packet.WriteString(_mapid);
            packet.WriteBytes(new byte[] { 0x4A, 0x23, 0xFD, 0x46, 0x00, 0x00, 0x00, 0x40, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            packet.WriteString("\\world\\areas\\" + _mapid + "\\area.dat");
        }
    }

    public class SMsg_CharacterAreaEnter : IServerPacket
    {
        uint _id;
        public SMsg_CharacterAreaEnter(uint objid)
        {
            _id = objid;
        }

        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(_id);
            packet.WriteUInt(1);
        }
    }

    public class SMsg_CharacterAreaServerSpec : IServerPacket
    {
        uint _id;
        string _mapname;
        string _mapid;
        public SMsg_CharacterAreaServerSpec(uint objid, string mapname, string mapid)
        {
            _id = objid;
            _mapname = mapname;
            _mapid = mapid;
        }

        public void WritePacket(Commons.Networking.AsyncConnection connection, Commons.Networking.ByteBuffer packet)
        {
            packet.WriteUInt(_id);
            packet.WriteString("AreaServer-" + _mapname + "-" + _mapid + "-1-:areaserver");
        }
    }

}
